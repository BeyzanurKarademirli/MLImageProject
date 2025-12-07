using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MLImageProject
{
    /// <summary>
    /// Görüntü ön işleme ve dönüştürme işlemleri için yardımcı sınıf
    /// </summary>
    public class ImageProcessor
    {
        /// <summary>
        /// Görüntüyü gri tonlamaya çevirir
        /// </summary>
        public static Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayscale = new Bitmap(original.Width, original.Height);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    grayscale.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }
            
            return grayscale;
        }

        /// <summary>
        /// Görüntüyü siyah-beyaz (binary) formata çevirir
        /// </summary>
        public static Bitmap ApplyThreshold(Bitmap original, int threshold = 128)
        {
            Bitmap binary = new Bitmap(original.Width, original.Height);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    Color newColor = grayValue > threshold ? Color.White : Color.Black;
                    binary.SetPixel(x, y, newColor);
                }
            }
            
            return binary;
        }

        /// <summary>
        /// Görüntüyü yeniden boyutlandırır
        /// </summary>
        public static Bitmap ResizeImage(Bitmap original, int width, int height)
        {
            Bitmap resized = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, 0, 0, width, height);
            }
            return resized;
        }

        /// <summary>
        /// Görüntü kontrastını artırır
        /// </summary>
        public static Bitmap AdjustContrast(Bitmap original, float contrast)
        {
            Bitmap adjusted = new Bitmap(original.Width, original.Height);
            float factor = (259.0f * (contrast + 255.0f)) / (255.0f * (259.0f - contrast));
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int r = Clamp((int)(factor * (pixel.R - 128) + 128));
                    int g = Clamp((int)(factor * (pixel.G - 128) + 128));
                    int b = Clamp((int)(factor * (pixel.B - 128) + 128));
                    adjusted.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            
            return adjusted;
        }

        /// <summary>
        /// Görüntü parlaklığını ayarlar
        /// </summary>
        public static Bitmap AdjustBrightness(Bitmap original, int brightness)
        {
            Bitmap adjusted = new Bitmap(original.Width, original.Height);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    int r = Clamp(pixel.R + brightness);
                    int g = Clamp(pixel.G + brightness);
                    int b = Clamp(pixel.B + brightness);
                    adjusted.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            
            return adjusted;
        }

        /// <summary>
        /// Görüntüyü tersine çevirir (negatif)
        /// </summary>
        public static Bitmap InvertImage(Bitmap original)
        {
            Bitmap inverted = new Bitmap(original.Width, original.Height);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    Color pixel = original.GetPixel(x, y);
                    inverted.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }
            
            return inverted;
        }

        /// <summary>
        /// Görüntüyü ML modeli için uygun formata dönüştürür
        /// </summary>
        public static float[] ImageToFloatArray(Bitmap image, int targetWidth = 224, int targetHeight = 224)
        {
            // Görüntüyü yeniden boyutlandır
            Bitmap resized = ResizeImage(image, targetWidth, targetHeight);
            
            // Gri tonlamaya çevir
            Bitmap grayscale = ConvertToGrayscale(resized);
            
            // Float array'e dönüştür (normalize edilmiş: 0-1 arası)
            float[] result = new float[targetWidth * targetHeight];
            int index = 0;
            
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    Color pixel = grayscale.GetPixel(x, y);
                    result[index++] = pixel.R / 255.0f;
                }
            }
            
            resized.Dispose();
            grayscale.Dispose();
            
            return result;
        }

        private static int Clamp(int value)
        {
            return Math.Max(0, Math.Min(255, value));
        }

        /// <summary>
        /// Görüntüyü belirtilen açıda döndürür
        /// </summary>
        public static Bitmap RotateImage(Bitmap original, float angle)
        {
            Bitmap rotated = new Bitmap(original.Width, original.Height);
            rotated.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.TranslateTransform(original.Width / 2, original.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-original.Width / 2, -original.Height / 2);
                g.DrawImage(original, new Point(0, 0));
            }

            return rotated;
        }

        /// <summary>
        /// Görüntüyü keskinleştirir
        /// </summary>
        public static Bitmap Sharpen(Bitmap original)
        {
            Bitmap sharpened = new Bitmap(original.Width, original.Height);

            // 3x3 Sharpen kernel
            int[,] kernel = new int[,]
            {
                { -1, -1, -1 },
                { -1,  9, -1 },
                { -1, -1, -1 }
            };

            for (int x = 1; x < original.Width - 1; x++)
            {
                for (int y = 1; y < original.Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            Color pixel = original.GetPixel(x + i, y + j);
                            r += pixel.R * kernel[i + 1, j + 1];
                            g += pixel.G * kernel[i + 1, j + 1];
                            b += pixel.B * kernel[i + 1, j + 1];
                        }
                    }

                    sharpened.SetPixel(x, y, Color.FromArgb(Clamp(r), Clamp(g), Clamp(b)));
                }
            }

            return sharpened;
        }

        /// <summary>
        /// Görüntüyü kırpar
        /// </summary>
        public static Bitmap CropImage(Bitmap original, Rectangle cropArea)
        {
            Bitmap cropped = new Bitmap(cropArea.Width, cropArea.Height);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(original, new Rectangle(0, 0, cropped.Width, cropped.Height),
                                 cropArea,
                                 GraphicsUnit.Pixel);
            }

            return cropped;
        }
    }
}

