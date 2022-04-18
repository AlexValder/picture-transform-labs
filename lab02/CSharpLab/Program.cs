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

        private delegate Bitmap ImageTransform(Bitmap bitmap, GistoCollection gisto = null);

        public static void Main() {
            while (true) {
                PrintMenu();
                var input = Console.ReadLine()?.Trim() ?? "";

                switch (input) {
                    case "1":
                        ShowSimpleHistogram();
                        break;
                    case "2":
                        Transform(Equalize);
                        break;
                    case "3":
                        Transform(ByFunction);
                        break;
                    case "0":
                        return;
                    default:
                        Console.Write($"Unknown input: {input}");
                        break;
                }

                Console.ReadKey();
            }
        }

        private static void ShowSimpleHistogram(string path = null) {
            try {
                string filename;
                if (path == null) {
                    Console.Write("Enter file name: ");
                    filename = Console.ReadLine();
                } else {
                    filename = path;
                }

                using var image = (Bitmap)Image.FromFile(filename!);
                ShowSimpleHistogram(image, filename);
            } catch (Exception e) {
                Console.WriteLine($"ERROR {e}: {e.Message}");
            }
        }

        private static void ShowSimpleHistogram(Bitmap image, string title = null) {
            try {
                var       data = CreateHistogram(image);
                using var form = new GistoForm(title ?? "Gistogram", data);
                form.ShowDialog();
            } catch (Exception e) {
                Console.WriteLine($"ERROR {e}: {e.Message}");
            }
        }

        private static void Transform(ImageTransform action) {
            try {
                Console.Write("Enter input image path: ");
                var input = Console.ReadLine();
                Console.Write("Enter output image path: ");
                var output = Console.ReadLine();

                using var inImage  = (Bitmap)Image.FromFile(input!);
                using var outImage = action(inImage);
                SaveImage(outImage, output);
                Console.WriteLine("Done!");
            } catch (Exception e) {
                Console.WriteLine($"ERROR {e}: {e.Message}");
            }
        }

        private static void PrintMenu() {
            Console.Clear();
            Console.Write(
                "MENU\n\n" +
                "1. Show gistogram.\n" +
                "2. Equalization.\n" +
                "3. Variant 9 function.\n" +
                "0. Exit.\n"
            );
            Console.Write("> ");
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
            Console.Write("Please enter lower limit: ");
            var lower = int.Parse(Console.ReadLine()!);
            Console.Write("Please enter upper limit: ");
            var upper = int.Parse(Console.ReadLine()!);

            foreach (var pair in input) {
                result[pair.Key] = VariantFunction(pair.Key, lower: lower, upper: upper);
            }

            return result;

            static int VariantFunction(int x, int lower, int upper) {
                return Convert.ToInt32(GetGradient(x, lower, upper) * 255);

                static float GetGradient(int x, int lower, int upper) {
                    if (x < lower) {
                        return 0.5f;
                    }

                    if (x < upper) {
                        return (x - lower) * 1f / (upper - lower);
                    }

                    return 0.5f;
                }
            }
        }
    }
}
