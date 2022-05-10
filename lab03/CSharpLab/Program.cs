using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CSharpLab {
    /// <summary>
    /// [[[ Variant #6 ]]]
    /// Фільтр серединної точки (з можливістю обрати розмір фільтру).
    /// </summary>
    internal static class Program {
        public static void Main() {
            while (true) {
                PrintMenu();
                var input = Console.ReadLine()?.Trim() ?? "";

                switch (input) {
                    case "1":
                        HandleConversion();
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

        private static void HandleConversion() {
            try {
                Console.Write("Enter input file name: ");
                var input = Console.ReadLine();
                Console.Write("Enter output file name: ");
                var output = Console.ReadLine();
                Console.Write("Enter filter size (SIZExSIZE): ");
                var size = Convert.ToInt32(Console.ReadLine());

                ApplyFilter(size, input, output);
            } catch (Exception ex) {
                Console.WriteLine($"FAILED to apply filter: {ex}: {ex.Message}");
            }
        }

        private static void PrintMenu() {
            Console.Clear();
            Console.Write(
                "1. Convert the image.\n" +
                "0. Exit.\n"
            );
            Console.Write("> ");
        }

        private static void ApplyFilter(int size, string input, string output) {
            var iter = 0;
            try {
                using var image = (Bitmap)Image.FromFile(input);
                using var blurred = (Bitmap)image.Clone();
                Console.WriteLine($"IMAGE RESOLUTION: {image.Width}x{image.Height}");

                for (var i = 0; i < image.Width; ++i) {
                    for (var j = 0; j < image.Height; ++j) {
                        int left, right, top, bottom;
                        if (size % 2 == 0) {
                            left   = Math.Max(i - size / 2, 0);
                            right  = Math.Min(i + size / 2, image.Width);
                            top    = Math.Max(j - size / 2, 0);
                            bottom = Math.Min(j + size / 2, image.Height);
                        } else {
                            left   = Math.Max(i - size / 2 + 1, 0);
                            right  = Math.Min(i + size / 2 + 1, image.Width);
                            top    = Math.Max(j - size / 2 + 1, 0);
                            bottom = Math.Min(j + size / 2 + 1, image.Height);
                        }

                        var rect = new Rectangle(
                            x: left, y: top,
                            width: right - left, height: bottom - top
                        );

                        if (rect.Width != 0 && rect.Height != 0) {
                            var array = new Color[rect.Width * rect.Height];

                            FillArray(image, array, rect);
                            SetColorByRed(blurred, array, i, j);
                        }

                        iter += 1;
                    }
                }

                Save(blurred, output);
                Console.WriteLine("Done!");
            } catch (Exception ex) {
                Console.WriteLine($"FAILED to apply filter (iter {iter}): {ex}: {ex.Message}");
            }
        }

        private static void SetColorByRed(Bitmap image, Color[] array, int i, int j) {
            var max = array.Max(GetBrightness);
            var min = array.Min(GetBrightness);
            var res = (byte)((max + min) / 2);

            var @new = Color.FromArgb(res, res, res);

            image.SetPixel(i, j, @new);
        }

        private static int GetBrightness(Color color) =>
            Convert.ToInt32(
                (color.R / 255f * 0.3 + color.G / 255f * 0.59 + color.B / 255f * 0.11) * 255
            );

        private static void FillArray(Bitmap bitmap, IList<Color> array, Rectangle rect) {
            var index = 0;
            for (var i = rect.Left; i < rect.Right; ++i)
            for (var j = rect.Top; j < rect.Bottom; ++j) {
                if (i >= 0 && j >= 0) {
                    array[index++] = bitmap.GetPixel(i, j);
                }
            }
        }

        private static void Save(Image image, string output) {
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(output)!);
                using var stream = File.Create(output);
                image.Save(stream, ImageFormat.Png);
            } catch (Exception ex) {
                Console.WriteLine($"FAILED to save image. {ex}: {ex.Message}");
            }
        }
    }
}
