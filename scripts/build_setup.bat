@echo off
REM =================================================================
REM  EBUninstaller Pro - One-Click Release Setup & Package Builder   
REM =================================================================
setlocal enabledelayedexpansion

echo =================================================================
echo  EBUninstaller Pro - Release Setup Builder                      
echo =================================================================

cd /d "%~dp0.."

echo [1/5] Unblocking repository files...
powershell -NoProfile -ExecutionPolicy Bypass -File "scripts\unblock_files.ps1"

echo [2/5] Compiling Solution in Release Mode...
dotnet build source\EBUninstaller.sln -c Release /p:Platform="Any CPU" /p:Version="7.0.0"
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Dotnet compilation failed!
    pause
    exit /b %ERRORLEVEL%
)

echo [3/5] Running Automated Test Suite...
dotnet test source\EBUninstallerTests\EBUninstallerTests.csproj -c Release --no-build --logger "console;verbosity=normal"
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Tests failed!
    pause
    exit /b %ERRORLEVEL%
)

echo [4/5] Creating Portable Distribution Archive...
if not exist "build\portable" mkdir "build\portable"
if not exist "build\installer" mkdir "build\installer"
powershell -NoProfile -ExecutionPolicy Bypass -Command "Compress-Archive -Path 'bin\Release\AnyCPU\*' -DestinationPath 'build\portable\EBUninstaller_Pro_Portable.zip' -Force"

echo [5/5] Compiling Inno Setup Release Installer...
set "ISCC="
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" set "ISCC=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if exist "C:\Program Files\Inno Setup 6\ISCC.exe" set "ISCC=C:\Program Files\Inno Setup 6\ISCC.exe"
if exist "C:\Program Files (x86)\Inno Setup 5\ISCC.exe" set "ISCC=C:\Program Files (x86)\Inno Setup 5\ISCC.exe"
if exist "%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe" set "ISCC=%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe"
if exist "C:\ProgramData\chocolatey\bin\iscc.exe" set "ISCC=C:\ProgramData\chocolatey\bin\iscc.exe"

if defined ISCC (
    echo  - Found Inno Setup Compiler: "!ISCC!"
    "!ISCC!" installer\EBUninstallSetup.iss
    if !ERRORLEVEL! EQU 0 (
        echo [SUCCESS] Setup installer built: build\installer\EBUninstallSetup.exe
    ) else (
        echo [WARNING] Inno Setup compilation returned exit code !ERRORLEVEL!
    )
) else (
    echo [WARNING] Inno Setup Compiler (ISCC.exe) not found in standard paths.
    echo  - Script ready at: installer\EBUninstallSetup.iss
    echo  - Install Inno Setup 6 (https://jrsoftware.org/isdl.php) and compile installer\EBUninstallSetup.iss.
)

echo.
echo =================================================================
echo  Build and Setup generation complete!
echo  Portable Zip:  build\portable\EBUninstaller_Pro_Portable.zip
echo  Installer EXE: build\installer\EBUninstallSetup.exe
echo =================================================================
pause
