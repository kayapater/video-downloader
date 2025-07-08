@echo off
chcp 65001 >nul
title Video İndirici - Basit Kurulum

echo.
echo ╔══════════════════════════════════════╗
echo ║        Video İndirici Kurulum        ║
echo ╚══════════════════════════════════════╝
echo.

net session >nul 2>&1
if %errorLevel% == 0 (
    echo ✅ Yönetici yetkisi mevcut
) else (
    echo ❌ Bu kurulum için yönetici yetkisi gerekli!
    echo    Lütfen "Yönetici olarak çalıştır" seçeneğini kullanın.
    pause
    exit /b 1
)

echo.
echo 📁 Kurulum klasörü oluşturuluyor...
set INSTALL_DIR=C:\Program Files (x86)\Video İndirici
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

echo.
echo 📋 Dosyalar kopyalanıyor...

xcopy /E /Y "publish\*" "%INSTALL_DIR%\" >nul
if %errorlevel% neq 0 (
    echo ❌ Dosyalar kopyalanamadı!
    pause
    exit /b 1
)

copy /Y "icon.ico" "%INSTALL_DIR%\" >nul
copy /Y "logo.png" "%INSTALL_DIR%\" >nul

echo ✅ Dosyalar başarıyla kopyalandı

echo.
echo 🔗 Kısayollar oluşturuluyor...

set START_MENU=%ProgramData%\Microsoft\Windows\Start Menu\Programs
if not exist "%START_MENU%\Video İndirici" mkdir "%START_MENU%\Video İndirici"

powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%START_MENU%\Video İndirici\Video İndirici.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\VideoDownloader.exe'; $Shortcut.Save()" >nul
echo.
choice /C YN /M "Masaüstüne kısayol oluşturulsun mu? (Y/N)"
if %errorlevel%==1 (
    powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%USERPROFILE%\Desktop\Video İndirici.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\VideoDownloader.exe'; $Shortcut.Save()" >nul
    echo ✅ Masaüstü kısayolu oluşturuldu
)

echo.
echo 📝 Windows kayıt defterine ekleniyor...

reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "DisplayName" /t REG_SZ /d "Video İndirici" /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "UninstallString" /t REG_SZ /d "\"%INSTALL_DIR%\kaldır.bat\"" /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "DisplayIcon" /t REG_SZ /d "\"%INSTALL_DIR%\VideoDownloader.exe\"" /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "Publisher" /t REG_SZ /d "kayapater" /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "DisplayVersion" /t REG_SZ /d "1.0.0" /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "Comments" /t REG_SZ /d "YouTube, Twitter ve Instagram video indirme aracı - kayapater" /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "NoModify" /t REG_DWORD /d 1 /f >nul
reg add "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /v "NoRepair" /t REG_DWORD /d 1 /f >nul

echo.
echo 🗑️ Kaldırma script'i oluşturuluyor...

(
echo @echo off
echo chcp 65001 ^>nul
echo title Video İndirici - Kaldırma
echo.
echo ╔══════════════════════════════════════╗
echo ║       Video İndirici Kaldırma        ║
echo ╚══════════════════════════════════════╝
echo.
echo Uygulama kaldırılıyor...
echo.
echo taskkill /F /IM VideoDownloader.exe 2^>nul
echo.
echo echo 📁 Dosyalar siliniyor...
echo del /Q "%INSTALL_DIR%\*.*" 2^>nul
echo rmdir /Q "%INSTALL_DIR%" 2^>nul
echo.
echo echo 🔗 Kısayollar siliniyor...
echo del /Q "%START_MENU%\Video İndirici\Video İndirici.lnk" 2^>nul
echo rmdir /Q "%START_MENU%\Video İndirici" 2^>nul
echo del /Q "%USERPROFILE%\Desktop\Video İndirici.lnk" 2^>nul
echo.
echo echo 📝 Kayıt defteri temizleniyor...
echo reg delete "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\VideoDownloader" /f 2^>nul
echo.
echo echo ✅ Video İndirici başarıyla kaldırıldı!
echo pause
) > "%INSTALL_DIR%\kaldır.bat"

echo ✅ Kayıt defteri girişleri oluşturuldu

echo.
echo ╔══════════════════════════════════════╗
echo ║           Kurulum Tamamlandı!        ║
echo ╚══════════════════════════════════════╝
echo.
echo ✅ Video İndirici başarıyla kuruldu
echo 📁 Kurulum yeri: %INSTALL_DIR%
echo 🔗 Başlat Menüsü: Video İndirici
echo 🗑️ Kaldırma: Programlar ve Özellikler'den
echo.

choice /C YN /M "Video İndirici'yi şimdi çalıştırmak istiyor musunuz? (Y/N)"
if %errorlevel%==1 (
    start "" "%INSTALL_DIR%\VideoDownloader.exe"
)

echo.
echo Kurulum tamamlandı! Bu pencereyi kapatabilirsiniz.
pause 
