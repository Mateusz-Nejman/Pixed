@echo off
@break off
@title Build distribution script for Pixed
@cls
echo Build distribution script for Pixed
set VERSION=1.14
set SOLUTION_PATH=%~dp0

set ZIP_PATH=%SOLUTION_PATH%build\Dist\Pixed.%VERSION%.win_x64.zip
set ZIP_LINUX_PATH=%SOLUTION_PATH%build\Dist\Pixed.%VERSION%.linux.zip
set ZIP_EXPORTER_PATH=%SOLUTION_PATH%build\Dist\Pixed.Exporter.%VERSION%.win_x64.zip
set MSI_PATH=%SOLUTION_PATH%build\Dist\Pixed.%VERSION%.win_x64.msi
set MSIX_PATH=%SOLUTION_PATH%build\Dist\Pixed.%VERSION%.win_x64.msixbundle
set MSIX_STORE_PATH=%SOLUTION_PATH%build\Dist\Pixed.%VERSION%.win_x64.msixupload

set MSIX_INPUT_PATH=%SOLUTION_PATH%\build\AppPackages\Pixed.MSStore_%VERSION%.0.0_Test\Pixed.MSStore_%VERSION%.0.0_x64.msixbundle
set MSIX_STORE_INPUT_PATH=%SOLUTION_PATH%\build\AppPackages\Pixed.MSStore_%VERSION%.0.0_x64_bundle.msixupload
set MSI_INPUT_PATH=%SOLUTION_PATH%build\Pixed.Installer.msi

echo Removing old files...
if not exist "%SOLUTION_PATH%build\Dist" (
  mkdir "%SOLUTION_PATH%build\Dist"
)

if exist "%ZIP_PATH%" (
    del "%ZIP_PATH%"
)

if exist "%MSI_PATH%" (
    del "%MSI_PATH%"
)

if exist "%MSIX_PATH%" (
    del "%MSIX_PATH%"
)

if exist "%MSIX_STORE_PATH%" (
    del "%MSIX_STORE_PATH%"
)

if exist "%ZIP_LINUX_PATH%" (
	del "%ZIP_LINUX_PATH%"
)

if exist "%ZIP_EXPORTER_PATH%" (
    del "%ZIP_EXPORTER_PATH%"
)

echo Deploying for Windows...
dotnet publish Pixed.Desktop/Pixed.Desktop.csproj --framework net9.0 --runtime win-x64 --output build/Deploy-win

echo Deploying for Linux...
dotnet publish Pixed.Desktop/Pixed.Desktop.csproj --framework net9.0 --runtime linux-x64 --output build/Deploy-linux

echo Deploying exporter for Windows...
dotnet publish Pixed.Exporter/Pixed.Exporter.csproj --framework net9.0 --runtime win-x64 --output build/Deploy-exporter-win

echo Compressing files for Windows...
cd "%SOLUTION_PATH%\build\Deploy-win"
"%SOLUTION_PATH%\7za.exe" a -x"!win-x64/" "%ZIP_PATH%" -mx9 -tzip

echo Compressing files for Linux...
cd "%SOLUTION_PATH%\build\Deploy-linux"
"%SOLUTION_PATH%\7za.exe" a -x"!linux-x64/" "%ZIP_LINUX_PATH%" -mx9 -tzip

echo Compressing exporter files for Windows...
cd "%SOLUTION_PATH%\build\Deploy-exporter-win"
"%SOLUTION_PATH%\7za.exe" a -x"!win-x64/" "%ZIP_EXPORTER_PATH%" -mx9 -tzip

cd "%SOLUTION_PATH%"

echo Creating MSI Installer...
wix build -ext WixToolset.UI.wixext -bindvariable WixUILicenseRtf=EULA.rtf msi.wxs -arch x64 -out build\Pixed.Installer.msi

echo Copying final files...
if exist "%MSI_INPUT_PATH%" (
    copy "%MSI_INPUT_PATH%" "%MSI_PATH%"
)

if exist "%MSIX_INPUT_PATH%" (
    copy "%MSIX_INPUT_PATH%" "%MSIX_PATH%"
)

if exist "%MSIX_STORE_INPUT_PATH%" (
    copy "%MSIX_STORE_INPUT_PATH%" "%MSIX_STORE_PATH%"
)
exit