using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Tesseract;

namespace MLImageProject
{
    /// <summary>
    /// Matematik denklemi tanıma ve işleme servisi
    /// </summary>
    public class MLMathService
    {
        private Dictionary<string, string> symbolMap;

        public MLMathService()
        {
            InitializeSymbolMap();
        }

        private void InitializeSymbolMap()
        {
            // Basit sembol eşleştirme (gerçek ML modeli yerine)
            symbolMap = new Dictionary<string, string>
            {
                { "0", "0" }, { "1", "1" }, { "2", "2" }, { "3", "3" }, { "4", "4" },
                { "5", "5" }, { "6", "6" }, { "7", "7" }, { "8", "8" }, { "9", "9" },
                { "+", "+" }, { "-", "-" }, { "*", "×" }, { "/", "÷" }, { "=", "=" },
                { "x", "x" }, { "y", "y" }, { "(", "(" }, { ")", ")" }
            };
        }

        /// <summary>
        /// Görüntüden matematik denklemini tanır ve çıkarır
        /// </summary>
        /// <param name="image">İşlenecek görüntü</param>
        /// <param name="language">OCR dili (eng, tur, vb.)</param>
        /// <param name="uiLanguage">UI dili (eng, tur) - hata mesajları için</param>
        public MathEquationResult RecognizeEquation(Bitmap image, string language = "eng", string uiLanguage = "eng")
        {
            var result = new MathEquationResult
            {
                OriginalImage = image,
                ProcessedImage = ImageProcessor.ConvertToGrayscale(image),
                Confidence = 0.0f,
                ProcessingTime = DateTime.Now
            };

            // Görüntüyü ön işleme (daha yumuşak işleme)
            Bitmap processed = ImageProcessor.ConvertToGrayscale(image);
            processed = ImageProcessor.AdjustContrast(processed, 30); // Daha yumuşak kontrast
            processed = ImageProcessor.ApplyThreshold(processed, 140); // Biraz daha yüksek threshold

            result.ProcessedImage = processed;

            // OCR ile metin tanıma (dil parametresi ile) - gerçek güven skorunu al
            var ocrResult = PerformOCR(processed, language, uiLanguage);
            result.RecognizedText = ocrResult.Text;
            result.Confidence = ocrResult.Confidence;

            // Matematik ifadesini parse et
            result.ParsedEquation = ParseMathExpression(ocrResult.Text);
            
            // Sonucu hesapla (eğer mümkünse)
            // Sonucu hesapla (eğer mümkünse)
            if (!result.ParsedEquation.Contains("Tanıma başarısız") && !result.ParsedEquation.Contains("OCR Error") && !result.ParsedEquation.Contains("Recognition failed"))
            {
                try
                {
                    result.CalculatedResult = EvaluateExpression(result.ParsedEquation);
                    result.IsValid = result.CalculatedResult.HasValue;
                }
                catch
                {
                    result.IsValid = false;
                }
            }
            else
            {
                result.IsValid = false;
            }

            return result;
        }

        /// <summary>
        /// OCR sonucu (metin ve güven skoru)
        /// </summary>
        private class OCRResult
        {
            public string Text { get; set; }
            public float Confidence { get; set; }
        }

        /// <summary>
        /// Tesseract OCR ile görüntüden metin tanıma (metin ve güven skoru döndürür)
        /// </summary>
        /// <param name="image">İşlenecek görüntü</param>
        /// <param name="language">OCR dili (eng, tur, vb.)</param>
        /// <param name="uiLanguage">UI dili (eng, tur) - hata mesajları için</param>
        private OCRResult PerformOCR(Bitmap image, string language = "eng", string uiLanguage = "eng")
        {
            try
            {
                // Görüntüyü OCR için optimize et
                Bitmap ocrImage = OptimizeImageForOCR(image);
                
                // Tesseract OCR kullan
                string[] possiblePaths = new string[]
                {
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"),
                    System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "tessdata"),
                    @".\tessdata",
                    @".\bin\Debug\tessdata",
                    @".\bin\Release\tessdata",
                    @"..\..\tessdata"
                };

                string tessdataPath = null;
                string debugInfo = "Aranan yollar:\n";
                
                foreach (string path in possiblePaths)
                {
                    debugInfo += $"- {path}\n";
                    if (System.IO.Directory.Exists(path))
                    {
                        string langFile = System.IO.Path.Combine(path, $"{language}.traineddata");
                        debugInfo += $"  Klasör var, dil dosyası kontrol ediliyor: {langFile}\n";
                        if (System.IO.File.Exists(langFile))
                        {
                            tessdataPath = path;
                            debugInfo += $"  ✓ Tessdata bulundu: {path}\n";
                            break;
                        }
                        else
                        {
                            debugInfo += $"  ✗ Dil dosyası yok: {langFile}\n";
                        }
                    }
                    else
                    {
                        debugInfo += $"  ✗ Klasör yok\n";
                    }
                }

                if (tessdataPath == null)
                {
                    string errorMsg = $"Tessdata klasörü bulunamadı!\n\n{debugInfo}\n\n" +
                        $"Lütfen tessdata klasörünü şu konuma koyun:\n" +
                        $"{AppDomain.CurrentDomain.BaseDirectory}\\tessdata\\\n" +
                        $"İçinde {language}.traineddata dosyası olmalı.";
                    throw new System.IO.DirectoryNotFoundException(errorMsg);
                }

                using (var engine = new TesseractEngine(tessdataPath, language, EngineMode.Default))
                {
                    // OCR için görüntü boyutunu kontrol et (çok küçükse büyüt)
                    if (ocrImage.Width < 100 || ocrImage.Height < 100)
                    {
                        ocrImage = ImageProcessor.ResizeImage(ocrImage, 
                            Math.Max(ocrImage.Width * 3, 300), 
                            Math.Max(ocrImage.Height * 3, 300));
                    }

                    // Görüntüyü Tesseract formatına çevir
                    using (var page = engine.Process(ocrImage, PageSegMode.Auto))
                    {
                        string recognizedText = page.GetText();
                        float confidence = page.GetMeanConfidence();
                        
                        // Güven skorunu 0-1 aralığına normalize et (Tesseract 0-100 döndürür)
                        float normalizedConfidence = confidence / 100.0f;
                        
                        // OCR sonuçlarını temizle
                        recognizedText = recognizedText?.Trim() ?? string.Empty;
                        
                        // Boş veya çok düşük güven skorlu sonuçları kontrol et
                        if (string.IsNullOrWhiteSpace(recognizedText) || normalizedConfidence < 0.1f)
                        {
                            string errorMsg = uiLanguage == "eng" 
                                ? $"Recognition failed (Confidence: {normalizedConfidence:P0})\n\n" +
                                  $"Try:\n" +
                                  $"1) Click 'Process' button first to improve image\n" +
                                  $"2) Adjust Threshold slider (try 100-150)\n" +
                                  $"3) Adjust Contrast slider\n" +
                                  $"4) Use a clearer, higher quality image"
                                : $"Tanıma başarısız (Güven: {normalizedConfidence:P0})\n\n" +
                                  $"Deneyin:\n" +
                                  $"1) Önce 'İşle' butonuna tıklayın\n" +
                                  $"2) Eşik Değeri kaydırıcısını ayarlayın (100-150 arası)\n" +
                                  $"3) Kontrast kaydırıcısını ayarlayın\n" +
                                  $"4) Daha net, kaliteli bir görüntü kullanın";
                            
                            return new OCRResult
                            {
                                Text = errorMsg,
                                Confidence = normalizedConfidence
                            };
                        }
                        
                        return new OCRResult
                        {
                            Text = recognizedText,
                            Confidence = normalizedConfidence
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Tesseract hatası - detaylı bilgi ver
                string errorMsg = uiLanguage == "eng"
                    ? $"OCR Error: {ex.Message}\n\n" +
                      $"Check:\n" +
                      $"1) Tessdata folder: bin\\Debug\\tessdata\\\n" +
                      $"2) File exists: {language}.traineddata\n" +
                      $"3) Image is loaded and visible"
                    : $"OCR Hatası: {ex.Message}\n\n" +
                      $"Kontrol edin:\n" +
                      $"1) Tessdata klasörü: bin\\Debug\\tessdata\\\n" +
                      $"2) Dosya var mı: {language}.traineddata\n" +
                      $"3) Görüntü yüklü ve görünür mü";
                
                return new OCRResult
                {
                    Text = errorMsg,
                    Confidence = 0.0f
                };
            }
        }

        /// <summary>
        /// Görüntüyü OCR için optimize eder
        /// </summary>
        private Bitmap OptimizeImageForOCR(Bitmap image)
        {
            // Görüntü zaten işlenmişse olduğu gibi döndür
            // Değilse temel optimizasyonlar yap
            
            // Minimum boyut kontrolü
            if (image.Width < 50 || image.Height < 50)
            {
                image = ImageProcessor.ResizeImage(image, 
                    Math.Max(image.Width * 4, 200), 
                    Math.Max(image.Height * 4, 200));
            }
            
            // Gri tonlama ve kontrast iyileştirme
            Bitmap optimized = ImageProcessor.ConvertToGrayscale(image);
            optimized = ImageProcessor.AdjustContrast(optimized, 20); // Hafif kontrast artışı
            optimized = ImageProcessor.ApplyThreshold(optimized, 128); // Binary threshold
            
            return optimized;
        }

        /// <summary>
        /// Matematik ifadesini parse eder
        /// </summary>
        private string ParseMathExpression(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Hata mesajlarını kontrol et
            if (text.Contains("OCR Hatası") || text.Contains("Tanıma başarısız"))
                return text;

            // Basit temizleme ve formatlama
            text = text.Replace(" ", "").Trim();
            
            // Yaygın OCR hatalarını düzelt
            text = text.Replace("O", "0").Replace("o", "0"); // O harfi yerine 0
            text = text.Replace("l", "1").Replace("I", "1"); // l/I harfi yerine 1
            text = text.Replace("S", "5"); // S harfi yerine 5
            
            // Matematik sembollerini normalize et
            text = text.Replace("×", "*").Replace("÷", "/").Replace("x", "*");
            text = text.Replace("=", " = ").Replace("+", " + ").Replace("-", " - ");
            text = text.Replace("*", " * ").Replace("/", " / ");
            
            // Fazla boşlukları temizle
            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            
            return text.Trim();
        }

        /// <summary>
        /// Matematik ifadesini değerlendirir ve sonucu hesaplar
        /// </summary>
        private double? EvaluateExpression(string expression)
        {
            try
            {
                // Basit matematik ifadelerini değerlendir
                // Sadece temel işlemler: +, -, *, /
                
                if (string.IsNullOrEmpty(expression) || expression.Contains("?"))
                    return null;

                // Eşittir işaretinden önceki ve sonraki kısımları ayır
                if (expression.Contains("="))
                {
                    string[] parts = expression.Split('=');
                    if (parts.Length == 2)
                    {
                        double left = EvaluateSimpleExpression(parts[0]);
                        double right = EvaluateSimpleExpression(parts[1]);
                        return left;
                    }
                }

                return EvaluateSimpleExpression(expression);
            }
            catch
            {
                return null;
            }
        }

        private double EvaluateSimpleExpression(string expression)
        {
            // Çok basit bir expression evaluator
            // Gerçek uygulamada daha gelişmiş bir parser kullanılmalı
            
            expression = expression.Trim();
            
            // Sadece sayılar ve temel operatörler
            if (double.TryParse(expression, out double result))
                return result;

            // Boşlukları kaldır
            expression = expression.Replace(" ", "");

            // Basit işlemler için regex kullan (birden fazla işlem için)
            // Önce çarpma ve bölme
            var multDivPattern = @"(\d+\.?\d*)\s*([*/])\s*(\d+\.?\d*)";
            var match = Regex.Match(expression, multDivPattern);
            while (match.Success)
            {
                double left = double.Parse(match.Groups[1].Value);
                string op = match.Groups[2].Value;
                double right = double.Parse(match.Groups[3].Value);
                double resultValue = op == "*" ? left * right : (right != 0 ? left / right : double.NaN);
                expression = expression.Substring(0, match.Index) + resultValue.ToString() + expression.Substring(match.Index + match.Length);
                match = Regex.Match(expression, multDivPattern);
            }

            // Sonra toplama ve çıkarma
            var addSubPattern = @"(\d+\.?\d*)\s*([+\-])\s*(\d+\.?\d*)";
            match = Regex.Match(expression, addSubPattern);
            if (match.Success)
            {
                double left = double.Parse(match.Groups[1].Value);
                string op = match.Groups[2].Value;
                double right = double.Parse(match.Groups[3].Value);

                switch (op)
                {
                    case "+": return left + right;
                    case "-": return left - right;
                }
            }

            // Tek sayı kaldıysa
            if (double.TryParse(expression, out result))
                return result;

            throw new Exception("İfade değerlendirilemedi");
        }

        /// <summary>
        /// Görüntüden karakter segmentasyonu yapar
        /// </summary>
        public List<Rectangle> SegmentCharacters(Bitmap image)
        {
            List<Rectangle> segments = new List<Rectangle>();
            
            // Basit karakter segmentasyonu
            // Gerçek uygulamada daha gelişmiş algoritmalar kullanılmalı
            
            int width = image.Width;
            int height = image.Height;
            bool inCharacter = false;
            int startX = 0;

            for (int x = 0; x < width; x++)
            {
                bool hasBlackPixel = false;
                for (int y = 0; y < height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.R < 128) // Siyah pixel
                    {
                        hasBlackPixel = true;
                        break;
                    }
                }

                if (hasBlackPixel && !inCharacter)
                {
                    startX = x;
                    inCharacter = true;
                }
                else if (!hasBlackPixel && inCharacter)
                {
                    segments.Add(new Rectangle(startX, 0, x - startX, height));
                    inCharacter = false;
                }
            }

            if (inCharacter)
            {
                segments.Add(new Rectangle(startX, 0, width - startX, height));
            }

            return segments;
        }
    }

    /// <summary>
    /// Matematik denklemi tanıma sonucu
    /// </summary>
    public class MathEquationResult
    {
        public Bitmap OriginalImage { get; set; }
        public Bitmap ProcessedImage { get; set; }
        public string RecognizedText { get; set; }
        public string ParsedEquation { get; set; }
        public double? CalculatedResult { get; set; }
        public float Confidence { get; set; }
        public bool IsValid { get; set; }
        public DateTime ProcessingTime { get; set; }
    }
}

