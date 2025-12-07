using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace MLImageProject
{
    public partial class Form1 : Form
    {
        private Bitmap originalImage;
        private Bitmap currentProcessedImage;
        private MLMathService mathService;
        private string currentFilePath;

        public Form1()
        {
            InitializeComponent();
            mathService = new MLMathService();
            
            // TrackBar event handlers
            trackBarContrast.ValueChanged += TrackBarContrast_ValueChanged;
            trackBarThreshold.ValueChanged += TrackBarThreshold_ValueChanged;
            
            // OCR dil seçimi değiştiğinde UI dilini güncelle
            comboOCRLanguage.SelectedIndexChanged += ComboOCRLanguage_SelectedIndexChanged;
            
            // Language manager event
            LanguageManager.OnLanguageChanged += UpdateUI;
            
            // Initialize UI
            UpdateUI();
        }

        private void ComboOCRLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // OCR dili değiştiğinde uygulama dilini de değiştir
            if (comboOCRLanguage.SelectedIndex == 0) // İngilizce
            {
                LanguageManager.SetLanguage(LanguageManager.Language.English);
            }
            else // Türkçe
            {
                LanguageManager.SetLanguage(LanguageManager.Language.Turkish);
            }
        }

        private void UpdateUI()
        {
            // Tüm UI elementlerini güncelle
            this.Text = LanguageManager.GetText("FormTitle");
            btnLoadImage.Text = LanguageManager.GetText("BtnLoadImage");
            btnProcess.Text = LanguageManager.GetText("BtnProcess");
            btnRecognize.Text = LanguageManager.GetText("BtnRecognize");
            btnSaveImage.Text = LanguageManager.GetText("BtnSaveImage");
            btnSaveResult.Text = LanguageManager.GetText("BtnSaveResult");
            
            btnGrayscale.Text = LanguageManager.GetText("BtnGrayscale");
            btnThreshold.Text = LanguageManager.GetText("BtnThreshold");
            btnAdjustContrast.Text = LanguageManager.GetText("BtnAdjustContrast");
            btnInvert.Text = LanguageManager.GetText("BtnInvert");
            
            lblOriginal.Text = LanguageManager.GetText("LblOriginal");
            lblProcessed.Text = LanguageManager.GetText("LblProcessed");
            lblResult.Text = LanguageManager.GetText("LblResult");
            lblOCRLanguage.Text = LanguageManager.GetText("LblOCRLanguage");
            groupBoxControls.Text = LanguageManager.GetText("GroupBoxControls");
            
            fileToolStripMenuItem.Text = LanguageManager.GetText("MenuFile");
            openToolStripMenuItem.Text = LanguageManager.GetText("MenuOpen");
            saveToolStripMenuItem.Text = LanguageManager.GetText("MenuSave");
            exitToolStripMenuItem.Text = LanguageManager.GetText("MenuExit");
            helpToolStripMenuItem.Text = LanguageManager.GetText("MenuHelp");
            aboutToolStripMenuItem.Text = LanguageManager.GetText("MenuAbout");
            
            // TrackBar etiketlerini güncelle
            lblContrast.Text = LanguageManager.GetText("LblContrast", trackBarContrast.Value);
            lblThreshold.Text = LanguageManager.GetText("LblThreshold", trackBarThreshold.Value);
            
            UpdateStatus(LanguageManager.GetText("StatusReady"));
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Görüntü Dosyaları|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tüm Dosyalar|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        currentFilePath = openFileDialog.FileName;

                        // Orijinal görüntüyü yükle
                        originalImage = new Bitmap(openFileDialog.FileName);
                        pictureBoxOriginal.Image = originalImage;
                        currentProcessedImage = new Bitmap(originalImage);
                        pictureBoxProcessed.Image = currentProcessedImage;
                        
                        // Dosya boyutunu al
                        FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                        string fileSize = FormatFileSize(fileInfo.Length);
                        
                        // Bilgileri göster
                        string infoText = $"{originalImage.Width}x{originalImage.Height}, {fileSize}";
                        lblOriginal.Text = $"{LanguageManager.GetText("LblOriginal")} ({infoText})";
                        
                        UpdateStatus($"Görüntü yüklendi: {Path.GetFileName(openFileDialog.FileName)} ({infoText})");
                        txtResult.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Görüntü yüklenirken hata oluştu:\n{ex.Message}", "Hata", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus("Görüntü yükleme hatası");
                    }
                }
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir görüntü yükleyin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateStatus("Görüntü işleniyor...");
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                
                // Varsayılan işleme: gri tonlama + eşik değeri
                Bitmap processed = ImageProcessor.ConvertToGrayscale(originalImage);
                processed = ImageProcessor.ApplyThreshold(processed, trackBarThreshold.Value);
                
                currentProcessedImage = processed;
                pictureBoxProcessed.Image = currentProcessedImage;
                
                UpdateStatus("Görüntü işlendi");
                progressBar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşleme sırasında hata oluştu:\n{ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("İşleme hatası");
                progressBar.Visible = false;
            }
        }

        private void btnGrayscale_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir görüntü yükleyin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateStatus("Gri tonlamaya çevriliyor...");
                currentProcessedImage = ImageProcessor.ConvertToGrayscale(originalImage);
                pictureBoxProcessed.Image = currentProcessedImage;
                UpdateStatus("Gri tonlama uygulandı");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThreshold_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir görüntü yükleyin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateStatus("Eşik değeri uygulanıyor...");
                Bitmap source = currentProcessedImage ?? ImageProcessor.ConvertToGrayscale(originalImage);
                currentProcessedImage = ImageProcessor.ApplyThreshold(source, trackBarThreshold.Value);
                pictureBoxProcessed.Image = currentProcessedImage;
                UpdateStatus($"Eşik değeri uygulandı: {trackBarThreshold.Value}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdjustContrast_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir görüntü yükleyin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateStatus("Kontrast ayarlanıyor...");
                Bitmap source = currentProcessedImage ?? originalImage;
                float contrastValue = trackBarContrast.Value;
                currentProcessedImage = ImageProcessor.AdjustContrast(source, contrastValue);
                pictureBoxProcessed.Image = currentProcessedImage;
                UpdateStatus($"Kontrast ayarlandı: {contrastValue}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInvert_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir görüntü yükleyin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateStatus("Görüntü tersine çevriliyor...");
                Bitmap source = currentProcessedImage ?? originalImage;
                currentProcessedImage = ImageProcessor.InvertImage(source);
                pictureBoxProcessed.Image = currentProcessedImage;
                UpdateStatus("Görüntü tersine çevrildi");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRecognize_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir görüntü yükleyin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UpdateStatus(LanguageManager.GetText("StatusProcessing"));
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                
                // Seçilen dili al (eng veya tur)
                string selectedLanguage = "eng"; // Varsayılan İngilizce
                if (comboOCRLanguage.SelectedIndex == 1)
                {
                    selectedLanguage = "tur"; // Türkçe
                }
                
                // ML servisi ile tanıma yap (dil parametresi ile)
                Bitmap imageToProcess = currentProcessedImage ?? originalImage;
                string uiLang = LanguageManager.CurrentLanguage == LanguageManager.Language.English ? "eng" : "tur";
                MathEquationResult result = mathService.RecognizeEquation(imageToProcess, selectedLanguage, uiLang);
                
                // Sonuçları göster
                DisplayResults(result);
                
                // İşlenmiş görüntüyü göster
                if (result.ProcessedImage != null)
                {
                    pictureBoxProcessed.Image = result.ProcessedImage;
                }
                
                UpdateStatus(LanguageManager.GetText("StatusRecognitionComplete", result.Confidence * 100));
                progressBar.Visible = false;
            }
            catch (Exception ex)
            {
                string errorMsg = LanguageManager.CurrentLanguage == LanguageManager.Language.English 
                    ? $"Error during ML recognition:\n{ex.Message}" 
                    : $"ML tanıma sırasında hata oluştu:\n{ex.Message}";
                string errorTitle = LanguageManager.CurrentLanguage == LanguageManager.Language.English 
                    ? "Error" 
                    : "Hata";
                    
                MessageBox.Show(errorMsg, errorTitle, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus(LanguageManager.CurrentLanguage == LanguageManager.Language.English 
                    ? "ML recognition error" 
                    : "ML tanıma hatası");
                progressBar.Visible = false;
            }
        }

        private void DisplayResults(MathEquationResult result)
        {
            txtResult.Clear();
            txtResult.AppendText("═══════════════════════════════════════════════════════\r\n");
            txtResult.AppendText($"  {LanguageManager.GetText("ResultTitle")}\r\n");
            txtResult.AppendText("═══════════════════════════════════════════════════════\r\n\r\n");
            
            // Görüntü bilgilerini ekle
            if (!string.IsNullOrEmpty(currentFilePath) && originalImage != null)
            {
                FileInfo fileInfo = new FileInfo(currentFilePath);
                string fileSize = FormatFileSize(fileInfo.Length);
                
                txtResult.AppendText($"Dosya Adı: {Path.GetFileName(currentFilePath)}\r\n");
                txtResult.AppendText($"Boyutlar: {originalImage.Width}x{originalImage.Height} piksel\r\n");
                txtResult.AppendText($"Dosya Boyutu: {fileSize}\r\n");
                txtResult.AppendText("═══════════════════════════════════════════════════════\r\n\r\n");
            }

            // Hata mesajlarını filtrele ve sadece geçerli sonuçları göster
            bool isError = result.RecognizedText.Contains("Tanıma başarısız") || 
                          result.RecognizedText.Contains("OCR Error") || 
                          result.RecognizedText.Contains("Recognition failed");

            if (!isError)
            {
                txtResult.AppendText($"{LanguageManager.GetText("ResultRecognizedText")} {result.RecognizedText}\r\n");
                txtResult.AppendText($"{LanguageManager.GetText("ResultParsedEquation")} {result.ParsedEquation}\r\n\r\n");
                
                if (result.CalculatedResult.HasValue)
                {
                    txtResult.AppendText($"{LanguageManager.GetText("ResultCalculated")} {result.CalculatedResult.Value}\r\n");
                }
                else
                {
                    txtResult.AppendText($"{LanguageManager.GetText("ResultNotCalculated")}\r\n");
                }
                
                txtResult.AppendText($"\r\n{LanguageManager.GetText("ResultConfidence")} {(result.Confidence * 100):F1}%\r\n");
                txtResult.AppendText($"{LanguageManager.GetText("ResultValidity")} {(result.IsValid ? LanguageManager.GetText("ResultValid") : LanguageManager.GetText("ResultInvalid"))}\r\n");
            }
            
            txtResult.AppendText($"{LanguageManager.GetText("ResultProcessingTime")} {result.ProcessingTime:HH:mm:ss}\r\n");
            txtResult.AppendText("\r\n═══════════════════════════════════════════════════════\r\n");
        }

        private void TrackBarContrast_ValueChanged(object sender, EventArgs e)
        {
            lblContrast.Text = $"Kontrast: {trackBarContrast.Value}";
        }

        private void TrackBarThreshold_ValueChanged(object sender, EventArgs e)
        {
            lblThreshold.Text = $"Eşik Değeri: {trackBarThreshold.Value}";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnLoadImage_Click(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProcessedImage();
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            SaveProcessedImage();
        }

        private void SaveProcessedImage()
        {
            if (currentProcessedImage == null)
            {
                MessageBox.Show("Kaydedilecek işlenmiş görüntü yok.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Dosyası|*.png|JPEG Dosyası|*.jpg|BMP Dosyası|*.bmp|Tüm Dosyalar|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "İşlenmiş Görüntüyü Kaydet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png;
                        string ext = Path.GetExtension(saveFileDialog.FileName).ToLower();
                        
                        if (ext == ".jpg" || ext == ".jpeg")
                            format = ImageFormat.Jpeg;
                        else if (ext == ".bmp")
                            format = ImageFormat.Bmp;
                        
                        currentProcessedImage.Save(saveFileDialog.FileName, format);
                        UpdateStatus($"Görüntü kaydedildi: {Path.GetFileName(saveFileDialog.FileName)}");
                        MessageBox.Show("Görüntü başarıyla kaydedildi.", "Başarılı", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kaydetme sırasında hata oluştu:\n{ex.Message}", "Hata", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSaveResult_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtResult.Text))
            {
                MessageBox.Show("Kaydedilecek sonuç yok.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Metin Dosyası|*.txt|Tüm Dosyalar|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Sonuçları Kaydet";
                saveFileDialog.DefaultExt = "txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Sonuçları dosyaya kaydet
                        string content = txtResult.Text;
                        content += $"\r\n\r\nKayıt Zamanı: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n";
                        
                        File.WriteAllText(saveFileDialog.FileName, content, System.Text.Encoding.UTF8);
                        UpdateStatus($"Sonuçlar kaydedildi: {Path.GetFileName(saveFileDialog.FileName)}");
                        MessageBox.Show("Sonuçlar başarıyla kaydedildi.", "Başarılı", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kaydetme sırasında hata oluştu:\n{ex.Message}", "Hata", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "ML Matematik Görüntü İşleme Projesi\n\n" +
                "Bu uygulama matematik denklemlerini görüntülerden tanımak ve işlemek için\n" +
                "makine öğrenmesi teknolojilerini kullanır.\n\n" +
                "Özellikler:\n" +
                "• Görüntü yükleme ve önizleme\n" +
                "• Görüntü ön işleme (gri tonlama, eşik değeri, kontrast)\n" +
                "• ML tabanlı matematik denklemi tanıma\n" +
                "• Otomatik sonuç hesaplama\n\n" +
                "Versiyon: 1.0\n" +
                "Geliştirme: Visual Studio 2022, C#, WinForms, ML.NET",
                "Hakkında",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Kaynakları temizle
            if (originalImage != null)
            {
                originalImage.Dispose();
            }
            if (currentProcessedImage != null && currentProcessedImage != originalImage)
            {
                currentProcessedImage.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
