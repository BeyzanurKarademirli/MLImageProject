# Terminalden Çalıştırma Kılavuzu

## Yöntem 1: PowerShell Script (Önerilen)

```powershell
.\build-and-run.ps1
```

## Yöntem 2: Batch Script

```cmd
build-and-run.bat
```

## Yöntem 3: Manuel Komutlar

### Visual Studio 2022 Developer PowerShell'de:

```powershell
# Projeyi derle
msbuild MLImageProject.sln /p:Configuration=Debug /p:Platform="Any CPU" /t:Rebuild

# Uygulamayı çalıştır
.\MLImageProject\bin\Debug\MLImageProject.exe
```

### Visual Studio 2022 Developer Command Prompt'da:

```cmd
REM Projeyi derle
msbuild MLImageProject.sln /p:Configuration=Debug /p:Platform="Any CPU" /t:Rebuild

REM Uygulamayı çalıştır
MLImageProject\bin\Debug\MLImageProject.exe
```

### Normal PowerShell/CMD'de (MSBuild yolunu belirtin):

```powershell
# MSBuild yolunu bulun (Visual Studio 2022 Community için)
$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"

# Projeyi derle
& $msbuild MLImageProject.sln /p:Configuration=Debug /p:Platform="Any CPU" /t:Rebuild

# Uygulamayı çalıştır
.\MLImageProject\bin\Debug\MLImageProject.exe
```

## Yöntem 4: dotnet CLI (Eğer .NET Framework için yapılandırılmışsa)

```powershell
# NuGet paketlerini restore et
dotnet restore MLImageProject.sln

# Projeyi derle
dotnet build MLImageProject.sln --configuration Debug

# Uygulamayı çalıştır
.\MLImageProject\bin\Debug\MLImageProject.exe
```

## Hızlı Komutlar

### Sadece Derleme:
```powershell
msbuild MLImageProject.sln /p:Configuration=Debug /t:Rebuild
```

### Sadece Çalıştırma (önceden derlenmişse):
```powershell
.\MLImageProject\bin\Debug\MLImageProject.exe
```

### Temizleme ve Yeniden Derleme:
```powershell
msbuild MLImageProject.sln /p:Configuration=Debug /t:Clean,Build
```

## Notlar

- Visual Studio 2022 Developer PowerShell veya Developer Command Prompt kullanmanız önerilir
- İlk çalıştırmada NuGet paketlerinin yüklü olması gerekir
- Eğer MSBuild bulunamazsa, Visual Studio Installer'dan "MSBuild" bileşeninin yüklü olduğundan emin olun

