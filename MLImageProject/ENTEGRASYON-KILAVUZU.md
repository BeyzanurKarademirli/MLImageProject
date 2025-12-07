# Proje Entegrasyon Kılavuzu

## 🔧 Ne Entegre Etmen Gerekiyor?

### ✅ Zaten Hazır Olanlar
- Görüntü yükleme ve işleme
- Temel görüntü ön işleme (gri tonlama, eşik, kontrast)
- UI ve kaydetme özellikleri
- Proje yapısı ve temel ML servisi

### ⚠️ Entegre Etmen Gerekenler

## 1. **Gerçek ML Modeli** (ÖNEMLİ!)

Şu anda `MLMathService.cs` içinde **simüle edilmiş** bir OCR var. Gerçek matematik denklemi tanıma için:

### Seçenek A: ML.NET Model Builder Kullan
1. Visual Studio'da: **Tools > ML.NET Model Builder**
2. Senaryo seç: **Image Classification** veya **Object Detection**
3. Veri setini hazırla (matematik denklemi görüntüleri)
4. Modeli eğit
5. Modeli projeye ekle
6. `MLMathService.cs` içindeki `SimulateOCR` metodunu gerçek model çağrısı ile değiştir

### Seçenek B: ONNX Model Kullan
1. Hazır bir ONNX modeli bul veya eğit (örn: TrOCR, MathPix API)
2. Modeli `Models` klasörüne ekle
3. `MLMathService.cs` içinde ONNX Runtime ile yükle:
```csharp
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;

// Model yükleme
var session = new InferenceSession("path/to/model.onnx");
```

### Seçenek C: Tesseract OCR Entegrasyonu
1. NuGet: `Tesseract` paketini yükle
2. Tesseract dil paketlerini indir
3. `MLMathService.cs` içinde Tesseract kullan:
```csharp
using Tesseract;

var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
var page = engine.Process(image);
string text = page.GetText();
```

## 2. **NuGet Paketleri** (İlk Kurulum)

Projeyi açtığında Visual Studio otomatik olarak yükleyebilir, ama manuel yükleme:

```
Microsoft.ML (3.0.1)
Microsoft.ML.ImageAnalytics (3.0.1)
Microsoft.ML.OnnxRuntime (1.18.0)
System.Drawing.Common (8.0.0)
```

**Eğer gerçek OCR kullanacaksan:**
```
Tesseract (4.3.0) - Tesseract OCR için
```

## 3. **Model Dosyaları Klasörü**

Projeye `Models` klasörü ekle ve model dosyalarını oraya koy:
```
MLImageProject/
  ├── Models/
  │   ├── math_recognizer.onnx (veya .mlnet)
  │   └── config.json (varsa)
```

## 4. **Örnek ML Servisi Güncellemesi**

`MLMathService.cs` içinde `SimulateOCR` metodunu şöyle güncelleyebilirsin:

```csharp
private string SimulateOCR(Bitmap image)
{
    // ÖRNEK: Tesseract kullanımı
    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
    {
        using (var page = engine.Process(image))
        {
            return page.GetText();
        }
    }
    
    // VEYA: ONNX Model kullanımı
    // var session = new InferenceSession("Models/math_model.onnx");
    // ... model inference kodu
}
```

## 5. **Veri Seti Hazırlama** (Model Eğitimi İçin)

Eğer kendi modelini eğiteceksen:
- Matematik denklemi görüntüleri topla
- Her görüntüyü etiketle (örn: "2+2=4")
- Veri setini train/validation/test olarak ayır
- ML.NET Model Builder veya Python (PyTorch/TensorFlow) ile eğit

## 6. **Performans İyileştirmeleri** (Opsiyonel)

- **Async/Await**: Uzun süren işlemler için
- **Caching**: İşlenmiş görüntüleri cache'le
- **Batch Processing**: Birden fazla görüntüyü toplu işle
- **GPU Desteği**: ONNX Runtime GPU versiyonu kullan

## 📝 Hızlı Başlangıç Checklist

- [ ] NuGet paketlerini yükle
- [ ] Gerçek ML modeli seç (Tesseract/ONNX/ML.NET)
- [ ] Model dosyasını projeye ekle
- [ ] `MLMathService.cs` içindeki `SimulateOCR` metodunu güncelle
- [ ] Test görüntüleri ile dene
- [ ] Performans ayarlamaları yap

## 🔗 Yararlı Kaynaklar

- **ML.NET**: https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet
- **Tesseract OCR**: https://github.com/charlesw/tesseract
- **ONNX Models**: https://github.com/onnx/models
- **MathPix API**: https://mathpix.com/ (Ücretli ama güçlü)

## ⚠️ Önemli Notlar

1. **Tesseract için**: `tessdata` klasörüne dil paketlerini indirmen gerekir
2. **ONNX için**: Model dosyasının doğru formatta olduğundan emin ol
3. **ML.NET için**: Model Builder ile kolayca model oluşturabilirsin
4. **API kullanımı**: MathPix gibi servisler için API key gerekir

## 🚀 Hızlı Test

Şu anki haliyle proje çalışır ama ML tanıma **simüle edilmiş**. Gerçek tanıma için yukarıdaki adımlardan birini uygula!

