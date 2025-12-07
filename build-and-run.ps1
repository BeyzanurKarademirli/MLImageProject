# ML Matematik Görüntü İşleme Projesi - Build ve Run Script
# PowerShell script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ML Matematik Görüntü İşleme Projesi" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Proje dizinine git
$projectPath = "MLImageProject"
$solutionPath = "MLImageProject.sln"

# MSBuild yolunu bul (Visual Studio 2022)
$msbuildPaths = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuild = $null
foreach ($path in $msbuildPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        break
    }
}

if (-not $msbuild) {
    Write-Host "MSBuild bulunamadı! Visual Studio 2022 Developer Command Prompt kullanın." -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternatif: Visual Studio 2022 Developer PowerShell'i açın ve şu komutu çalıştırın:" -ForegroundColor Yellow
    Write-Host "  msbuild MLImageProject.sln /p:Configuration=Debug" -ForegroundColor Yellow
    Write-Host "  .\MLImageProject\bin\Debug\MLImageProject.exe" -ForegroundColor Yellow
    exit 1
}

Write-Host "MSBuild bulundu: $msbuild" -ForegroundColor Green
Write-Host ""

# Projeyi derle
Write-Host "Proje derleniyor..." -ForegroundColor Yellow
& $msbuild $solutionPath /p:Configuration=Debug /p:Platform="Any CPU" /t:Rebuild /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Derleme başarısız!" -ForegroundColor Red
    exit 1
}

Write-Host "Derleme başarılı!" -ForegroundColor Green
Write-Host ""

# Exe dosyasını çalıştır
$exePath = "MLImageProject\bin\Debug\MLImageProject.exe"

if (Test-Path $exePath) {
    Write-Host "Uygulama başlatılıyor..." -ForegroundColor Yellow
    Write-Host ""
    & $exePath
} else {
    Write-Host "Exe dosyası bulunamadı: $exePath" -ForegroundColor Red
    exit 1
}

