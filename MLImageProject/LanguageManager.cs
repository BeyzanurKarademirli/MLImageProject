using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MLImageProject
{
    /// <summary>
    /// Uygulama dili yönetimi
    /// </summary>
    public static class LanguageManager
    {
        public enum Language
        {
            Turkish,
            English
        }

        private static Language currentLanguage = Language.Turkish;
        private static Dictionary<string, Dictionary<Language, string>> translations;

        static LanguageManager()
        {
            InitializeTranslations();
        }

        private static void InitializeTranslations()
        {
            translations = new Dictionary<string, Dictionary<Language, string>>
            {
                // Form başlığı
                ["FormTitle"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "ML Matematik Görüntü İşleme Projesi" },
                    { Language.English, "ML Math Image Processing Project" }
                },

                // Butonlar
                ["BtnLoadImage"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "📷 Görüntü Yükle" },
                    { Language.English, "📷 Load Image" }
                },
                ["BtnProcess"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "🔄 İşle" },
                    { Language.English, "🔄 Process" }
                },
                ["BtnRecognize"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "🤖 ML Tanıma" },
                    { Language.English, "🤖 ML Recognition" }
                },
                ["BtnSaveImage"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "💾 Görüntü Kaydet" },
                    { Language.English, "💾 Save Image" }
                },
                ["BtnSaveResult"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "📄 Sonuç Kaydet" },
                    { Language.English, "📄 Save Result" }
                },

                // Görüntü işleme butonları
                ["BtnGrayscale"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Gri Tonlama" },
                    { Language.English, "Grayscale" }
                },
                ["BtnThreshold"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Eşik Değeri" },
                    { Language.English, "Threshold" }
                },
                ["BtnAdjustContrast"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Kontrast" },
                    { Language.English, "Contrast" }
                },
                ["BtnInvert"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Ters Çevir" },
                    { Language.English, "Invert" }
                },

                // Etiketler
                ["LblOriginal"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Orijinal Görüntü" },
                    { Language.English, "Original Image" }
                },
                ["LblProcessed"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "İşlenmiş Görüntü" },
                    { Language.English, "Processed Image" }
                },
                ["LblResult"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Sonuç:" },
                    { Language.English, "Result:" }
                },
                ["LblContrast"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Kontrast: {0}" },
                    { Language.English, "Contrast: {0}" }
                },
                ["LblThreshold"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Eşik Değeri: {0}" },
                    { Language.English, "Threshold: {0}" }
                },
                ["LblOCRLanguage"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "OCR Dili:" },
                    { Language.English, "OCR Language:" }
                },

                // Grup kutuları
                ["GroupBoxControls"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Görüntü İşleme" },
                    { Language.English, "Image Processing" }
                },

                // Menü
                ["MenuFile"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Dosya" },
                    { Language.English, "File" }
                },
                ["MenuOpen"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Aç..." },
                    { Language.English, "Open..." }
                },
                ["MenuSave"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Kaydet..." },
                    { Language.English, "Save..." }
                },
                ["MenuExit"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Çıkış" },
                    { Language.English, "Exit" }
                },
                ["MenuHelp"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Yardım" },
                    { Language.English, "Help" }
                },
                ["MenuAbout"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Hakkında" },
                    { Language.English, "About" }
                },

                // Durum mesajları
                ["StatusReady"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Hazır - Görüntü yükleyin" },
                    { Language.English, "Ready - Load an image" }
                },
                ["StatusProcessing"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "ML ile matematik denklemi tanınıyor..." },
                    { Language.English, "Recognizing math equation with ML..." }
                },
                ["StatusRecognitionComplete"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Tanıma tamamlandı - Güven: {0:F1}%" },
                    { Language.English, "Recognition complete - Confidence: {0:F1}%" }
                },

                // Sonuç başlıkları
                ["ResultTitle"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "ML MATEMATİK DENKLEMİ TANIMA SONUÇLARI" },
                    { Language.English, "ML MATH EQUATION RECOGNITION RESULTS" }
                },
                ["ResultRecognizedText"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Tanınan Metin:" },
                    { Language.English, "Recognized Text:" }
                },
                ["ResultParsedEquation"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Parse Edilmiş Denklem:" },
                    { Language.English, "Parsed Equation:" }
                },
                ["ResultCalculated"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Hesaplanan Sonuç:" },
                    { Language.English, "Calculated Result:" }
                },
                ["ResultNotCalculated"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Sonuç: Hesaplanamadı" },
                    { Language.English, "Result: Could not be calculated" }
                },
                ["ResultConfidence"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Güven Skoru:" },
                    { Language.English, "Confidence Score:" }
                },
                ["ResultValidity"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "Geçerlilik:" },
                    { Language.English, "Validity:" }
                },
                ["ResultValid"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "✓ Geçerli" },
                    { Language.English, "✓ Valid" }
                },
                ["ResultInvalid"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "✗ Geçersiz" },
                    { Language.English, "✗ Invalid" }
                },
                ["ResultProcessingTime"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "İşlem Zamanı:" },
                    { Language.English, "Processing Time:" }
                },
                ["ResultHint"] = new Dictionary<Language, string>
                {
                    { Language.Turkish, "💡 İpucu: Tanınan metin matematik ifadesi olarak parse edilemedi.\r\n   Görüntü kalitesini artırmayı veya farklı bir dil seçmeyi deneyin." },
                    { Language.English, "💡 Tip: Recognized text could not be parsed as a math expression.\r\n   Try improving image quality or selecting a different language." }
                }
            };
        }

        public static Language CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                currentLanguage = value;
                OnLanguageChanged?.Invoke();
            }
        }

        public static event Action OnLanguageChanged;

        public static string GetText(string key, params object[] args)
        {
            if (translations.ContainsKey(key) && translations[key].ContainsKey(currentLanguage))
            {
                string text = translations[key][currentLanguage];
                if (args.Length > 0)
                {
                    return string.Format(text, args);
                }
                return text;
            }
            return key; // Key bulunamazsa key'i döndür
        }

        public static void SetLanguage(Language lang)
        {
            CurrentLanguage = lang;
        }
    }
}

