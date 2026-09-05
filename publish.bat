@echo off
setlocal enabledelayedexpansion

echo =================================================================
echo  EBUninstaller Pro - Automated Publishing & Release Builder
echo =================================================================

set CONFIG=Release
set PLATFORM=AnyCPU
set REPO_ROOT=%~dp0
set SOLUTION=%REPO_ROOT%source\BulkCrapUninstaller.sln
set BUILD_DIR=%REPO_ROOT%build
set BIN_DIR=%REPO_ROOT%bin\%CONFIG%\%PLATFORM%

:: 1. Clean previous builds
if exist "%BUILD_DIR%" rmdir /s /q "%BUILD_DIR%"
mkdir "%BUILD_DIR%\portable"
mkdir "%BUILD_DIR%\installer"

:: 2. Build via dotnet CLI
echo.
echo [1/3] Compiling EBUninstaller Pro (.NET 8)...
where dotnet >nul 2>nul
if %errorlevel% equ 0 (
    dotnet build "%SOLUTION%" --configuration %CONFIG% -p:Platform=%PLATFORM% -p:Version="7.0.0"
    if %errorlevel% neq 0 (
        echo [ERROR] Dotnet build failed!
        pause
        exit /b 1
    )
) else (
    echo [WARNING] dotnet CLI not found in PATH.
)

:: 3. Create Portable Package
echo.
echo [2/3] Creating Portable ZIP Archive...
powershell -NoProfile -Command "if (Test-Path '%BIN_DIR%') { Compress-Archive -Path '%BIN_DIR%\*' -DestinationPath '%BUILD_DIR%\portable\EBUninstaller_Pro_Portable.zip' -Force; Write-Host ' -> Portable archive created successfully.' } else { Write-Host ' -> Bin directory not found.' }"

:: 4. Build Inno Setup Installer
echo.
echo [3/3] Compiling Inno Setup Installer...
set ISCC="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if exist %ISCC% (
    %ISCC% "%REPO_ROOT%installer\EBUninstallSetup.iss"
    echo  -> Installer compiled to %BUILD_DIR%\installer\
) else (
    echo  -> Inno Setup compiler (ISCC.exe) not found in standard path. Skipping installer compile.
)

echo.
echo =================================================================
echo  EBUninstaller Pro Build Complete!
echo =================================================================
pause
exit /b 0
