@echo off
REM ML Matematik Görüntü İşleme Projesi - Build ve Run Script
REM Batch script

echo ========================================
echo ML Matematik Görüntü İşleme Projesi
echo ========================================
echo.

REM Visual Studio 2022 Developer Command Prompt'ta çalıştırılmalı
REM veya MSBuild yolunu manuel olarak ayarlayın

REM MSBuild ile projeyi derle
echo Proje derleniyor...
msbuild MLImageProject.sln /p:Configuration=Debug /p:Platform="Any CPU" /t:Rebuild /v:minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Derleme basarisiz!
    pause
    exit /b 1
)

echo.
echo Derleme basarili!
echo.

REM Exe dosyasını çalıştır
if exist "MLImageProject\bin\Debug\MLImageProject.exe" (
    echo Uygulama baslatiliyor...
    echo.
    start "" "MLImageProject\bin\Debug\MLImageProject.exe"
) else (
    echo Exe dosyasi bulunamadi!
    pause
    exit /b 1
)

pause

