# ML Matematik Görüntü İşleme ve OCR Projesi

Bu proje, görüntülerden matematiksel denklemleri tanımak, işlemek ve çözmek için geliştirilmiş bir C# Windows Forms uygulamasıdır. Makine öğrenmesi (ML) ve Optik Karakter Tanıma (OCR) teknolojilerini kullanarak, el yazısı veya basılı matematiksel ifadeleri dijital metne dönüştürür ve sonuçlarını hesaplar.

## 🚀 Özellikler

### 📷 Görüntü İşleme
Uygulama, OCR başarısını artırmak için gelişmiş görüntü işleme araçları sunar:
*   **Gri Tonlama (Grayscale):** Görüntüyü siyah-beyaz tonlarına dönüştürür.
*   **Eşik Değeri (Thresholding):** Görüntüyü ikili (binary) formata çevirerek gürültüyü azaltır.
*   **Kontrast Ayarı:** Görüntünün netliğini artırmak için kontrast seviyesini değiştirmenizi sağlar.
*   **Ters Çevirme (Invert):** Renkleri tersine çevirir (negatif görüntü).
*   **Döndürme ve Kırpma:** Görüntüyü hizalamak ve gereksiz alanları temizlemek için araçlar.
*   **Keskinleştirme:** Bulanık görüntüleri netleştirir.

### 🤖 OCR ve Matematik Çözücü
*   **Metin Tanıma:** Tesseract OCR motorunu kullanarak görüntüdeki metinleri ve sayıları algılar.
*   **Denklem Ayrıştırma:** Algılanan metni matematiksel bir ifade olarak ayrıştırır (örn. "100-50").
*   **Otomatik Hesaplama:** Geçerli matematiksel ifadeleri otomatik olarak çözer ve sonucu gösterir.

### 📊 Detaylı Sonuç Ekranı
*   **Görüntü Bilgileri:** Yüklenen dosyanın adı, boyutları (piksel) ve dosya boyutu (KB/MB) görüntülenir.
*   **Tanıma Sonuçları:** Algılanan ham metin, ayrıştırılmış denklem ve hesaplanan sonuç.

## 🛠️ Teknolojiler

*   **Dil:** C# (.NET Framework 4.8)
*   **Arayüz:** Windows Forms (WinForms)
*   **OCR Kütüphanesi:** Tesseract
*   **Görüntü İşleme:** System.Drawing, AForge.NET (veya benzeri yerel kütüphaneler)
*   **IDE:** Visual Studio 2022

## 📦 Kurulum ve Kullanım

1.  Bu depoyu (repository) klonlayın:
    ```bash
    git clone https://github.com/kullaniciadi/MLImageProject.git
    ```
2.  Projeyi Visual Studio ile açın (`MLImageProject.sln`).
3.  Gerekli NuGet paketlerinin yüklendiğinden emin olun (Tesseract vb.).
4.  Projeyi derleyin ve çalıştırın.
5.  **Görüntü Yükle** butonuna tıklayarak bir resim seçin.
6.  Gerekirse **İşle** butonlarını kullanarak görüntüyü iyileştirin.
7.  **ML Tanıma** butonuna basarak sonucu görün.

