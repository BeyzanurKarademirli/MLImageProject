# ML Matematik Görüntü İşleme Projesi

Bu proje, matematik denklemlerini görüntülerden tanımak ve işlemek için makine öğrenmesi teknolojilerini kullanan bir WinForms uygulamasıdır.

## Özellikler

- 📷 **Görüntü Yükleme**: JPG, PNG, BMP formatlarında görüntü yükleme
- 🔄 **Görüntü Ön İşleme**:
  - Gri tonlamaya çevirme
  - Eşik değeri (threshold) uygulama
  - Kontrast ayarlama
  - Görüntüyü tersine çevirme
- 🤖 **ML Tabanlı Tanıma**: Matematik denklemlerini görüntülerden tanıma
- 🧮 **Otomatik Hesaplama**: Tanınan denklemlerin sonuçlarını hesaplama
- 💾 **Kaydetme**: İşlenmiş görüntüleri kaydetme

## Gereksinimler

- Visual Studio 2022
- .NET Framework 4.8
- Windows 10/11

## Kurulum

1. Projeyi Visual Studio 2022'de açın
2. NuGet paketlerini yükleyin:
   - Microsoft.ML (3.0.1)
   - Microsoft.ML.ImageAnalytics (3.0.1)
   - Microsoft.ML.OnnxRuntime (1.18.0)
   - System.Drawing.Common (8.0.0)

3. Projeyi derleyin ve çalıştırın

## Kullanım

1. **Görüntü Yükleme**: "Görüntü Yükle" butonuna tıklayarak bir matematik denklemi içeren görüntü yükleyin
2. **Ön İşleme**: Görüntüyü iyileştirmek için çeşitli işleme seçeneklerini kullanın:
   - Gri tonlama
   - Eşik değeri ayarlama
   - Kontrast ayarlama
3. **ML Tanıma**: "ML Tanıma" butonuna tıklayarak matematik denklemini tanıtın
4. **Sonuçları Görüntüleme**: Tanınan denklem ve hesaplanan sonuçlar sonuç kutusunda görüntülenecektir

## Proje Yapısı

- `Form1.cs` - Ana form ve kullanıcı arayüzü mantığı
- `Form1.Designer.cs` - Form tasarımı
- `ImageProcessor.cs` - Görüntü ön işleme yardımcı sınıfı
- `MLMathService.cs` - ML tabanlı matematik denklemi tanıma servisi
- `Program.cs` - Uygulama giriş noktası

## Notlar

- Bu uygulama bir demo uygulamadır. Gerçek ML modeli entegrasyonu için eğitilmiş bir model dosyası (.onnx veya .mlnet) gereklidir.
- Şu anda basit bir OCR simülasyonu kullanılmaktadır. Gerçek uygulamada ML.NET veya Tesseract OCR gibi kütüphaneler kullanılmalıdır.

## Geliştirme

Gerçek ML modeli entegrasyonu için:

1. Matematik denklemi tanıma için bir ML modeli eğitin (ML.NET Model Builder kullanabilirsiniz)
2. Model dosyasını projeye ekleyin
3. `MLMathService.cs` dosyasındaki `SimulateOCR` metodunu gerçek model çağrısı ile değiştirin

## Lisans

Bu proje eğitim amaçlıdır.

