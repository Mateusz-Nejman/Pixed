@echo off
@break off
@title Build distribution script for Pixed
@cls
echo Build distribution script for Pixed
set VERSION=1.9
set SOLUTION_PATH=%~dp0

set ZIP_PATH=%SOLUTION_PATH%build\Dist\Pixed.%VERSION%.win_x64.zip
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

echo Deploying...
dotnet publish Pixed.Windows/Pixed.Windows.csproj --framework net8.0-windows --runtime win-x64 --output build/Deploy

echo Compressing files...

cd "%SOLUTION_PATH%\build\Deploy"

"%SOLUTION_PATH%\7za.exe" a -x"!win-x64/" "%ZIP_PATH%" -mx9 -tzip

cd "%SOLUTION_PATH%"

echo Creating MSI Installer...
wix build -ext WixToolset.UI.wixext -bindvariable WixUILicenseRtf=EULA.rtf msi.wxs -out build\Pixed.Installer.msi

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