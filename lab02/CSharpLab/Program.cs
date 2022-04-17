using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using GistoCollection = System.Collections.Generic.Dictionary<int, int>;

namespace CSharpLab {
    internal static class Program {
        private delegate GistoCollection GistogramTransform(GistoCollection original);

        public static void Main(string[] args) {
            foreach (var arg in args) {
                ShowHistogram(arg);
            }
        }

        private static void ShowHistogram(string file) {
            var resIndex = file.LastIndexOf("/", StringComparison.Ordinal);
            var filename = file.Substring(resIndex + 1);

            using var image = (Bitmap)Image.FromFile(file);
            var       data  = CreateHistogram(image);
            using var form1  = new GistoForm($"{filename} - ORIGINAL", data);
            form1.ShowDialog();

            using var eqImage = Equalize(image, data);
            var       path     = AppDomain.CurrentDomain.BaseDirectory + $"eq\\{filename}";
            SaveImage(eqImage, path);
            using var form2 = new GistoForm($"{filename} - EQUALIZED", CreateHistogram(eqImage));
            form2.ShowDialog();

            using var funImage = ByFunction(image, data);
            var       path2   = AppDomain.CurrentDomain.BaseDirectory + $"fun\\{filename}";
            SaveImage(funImage, path2);
            using var form3 = new GistoForm($"{filename} - VARIANT 9", CreateHistogram(funImage));
            form3.ShowDialog();
        }

        private static void SaveImage(Image bitmap, string path) {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var stream = File.Create(path);
            bitmap.Save(stream, ImageFormat.Png);
        }

        private static Dictionary<int, int> CreateHistogram(Bitmap image) {
            var dict = new Dictionary<int, int>(256);
            for (var i = 0; i < 256; ++i) {
                dict[i] = 0;
            }

            for (var x = 0; x < image.Size.Width; ++x)
            for (var y = 0; y < image.Size.Height; ++y) {
                var pixel = image.GetPixel(x, y);
                dict[CalculateBrightness(pixel)] += 1;
            }

            return dict;
        }

        private static int CalculateBrightness(Color color) =>
            Convert.ToInt32(
                (color.R / 255f * 0.3 + color.G / 255f * 0.59 + color.B / 255f * 0.11) * 255
            );

        private static Bitmap Tranform(Bitmap original, GistogramTransform tranform, GistoCollection gisto = null) {
            gisto ??= CreateHistogram(original);

            var result    = (Bitmap)original.Clone();
            var equalized = tranform(gisto);

            for (var x = 0; x < result.Size.Width; ++x)
            for (var y = 0; y < result.Size.Height; ++y) {
                var pixel      = result.GetPixel(x, y);
                var brightness = equalized[CalculateBrightness(pixel)];
                result.SetPixel(x, y, Color.FromArgb(brightness, brightness, brightness));
            }

            return result;
        }

        private static GistoCollection GistoEqualizer(GistoCollection input) {
            var result = new GistoCollection(input.Count);
            var acc    = 0;
            var n      = input.Values.Sum();
            foreach (var pair in input) {
                acc                 += pair.Value;
                result[pair.Key] =  Convert.ToInt32(acc * 1f / n * 255);
            }

            return result;
        }

        private static Bitmap Equalize(Bitmap original, GistoCollection gisto = null) =>
            Tranform(original, GistoEqualizer, gisto);

        private static Bitmap ByFunction(Bitmap original, GistoCollection gisto = null) =>
            Tranform(original, VariantGisto, gisto);

        private static GistoCollection VariantGisto(GistoCollection input) {
            var result = new GistoCollection(input.Count);
            foreach (var pair in input) {
                result[pair.Key] = VariantFunction(pair.Key);
            }

            return result;

            static int VariantFunction(int x) {
                const int fst = 255 / 3;
                const int snd = 2 * 255 / 3;

                return Convert.ToInt32(
                    x switch {
                        < fst => 0.5f,
                        < snd => (x - fst) * 1f / (snd - fst),
                        _     => 0.5f
                    } * 255
                );
            }
        }
    }
}
