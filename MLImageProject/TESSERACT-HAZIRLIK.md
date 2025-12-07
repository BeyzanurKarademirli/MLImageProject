# Tesseract OCR Entegrasyonu - Hazırlık Adımları

## 📦 Adım 1: NuGet Paketini Yükle

Visual Studio'da:
1. Solution Explorer → MLImageProject projesine sağ tık
2. "Manage NuGet Packages" seç
3. "Browse" sekmesinde "Tesseract" ara
4. **Tesseract** paketini yükle (Charles Weld tarafından)
5. Versiyon: **4.3.0** veya üzeri

## 📥 Adım 2: Tesseract Dil Paketlerini İndir

1. https://github.com/tesseract-ocr/tessdata adresine git
2. **eng.traineddata** dosyasını indir (İngilizce için)
3. **tur.traineddata** dosyasını indir (Türkçe için - opsiyonel)
4. Bu dosyaları projenin **bin\Debug** klasörüne **tessdata** adında bir klasör oluşturup oraya koy:
   ```
   MLImageProject\bin\Debug\tessdata\
     ├── eng.traineddata
     └── tur.traineddata (opsiyonel)
   ```

## ✅ Adım 3: Paketi Yükledikten Sonra

Paketi yükledikten sonra bana haber ver, ben kodda gerekli değişiklikleri yapacağım:
- `MLMathService.cs` içinde Tesseract entegrasyonu
- Gerekli using'ler
- OCR metodunu aktif hale getirme

## 🔧 Alternatif: Otomatik Kopyalama

Eğer tessdata dosyalarını projeye dahil etmek istersen:
1. Projeye `tessdata` klasörü ekle
2. `eng.traineddata` dosyasını oraya kopyala
3. Solution Explorer'da dosyaya sağ tık → Properties
4. "Copy to Output Directory" → "Copy always" veya "Copy if newer"

## ⚠️ Önemli Notlar

- Tesseract matematik sembolleri için özel eğitim gerektirebilir
- Basit matematik denklemleri için iyi çalışır
- Karmaşık formüller için MathPix veya özel model daha iyi olabilir

Paketi yükledikten sonra haber ver! 🚀

