@echo off
setlocal EnableExtensions

echo =================================================================
echo  EBUninstaller Pro - Automated Publishing and Release Builder
echo =================================================================

set "CONFIG=Release"
set "REPO_ROOT=%~dp0"
set "SOLUTION=%REPO_ROOT%source\BulkCrapUninstaller.sln"
set "BUILD_DIR=%REPO_ROOT%build"
set "BIN_DIR=%REPO_ROOT%bin\Release\AnyCPU"

:: 1. Clean previous builds
if exist "%BUILD_DIR%" (
    rmdir /s /q "%BUILD_DIR%" 2>nul
)
mkdir "%BUILD_DIR%\portable" 2>nul
mkdir "%BUILD_DIR%\installer" 2>nul

:: 2. Build via dotnet CLI or MSBuild
echo.
echo [1/3] Compiling EBUninstaller Pro (.NET 8)...

where dotnet >nul 2>nul
if %ERRORLEVEL% equ 0 (
    echo  -> Using dotnet CLI...
    dotnet build "%SOLUTION%" --configuration %CONFIG% -p:Platform="Any CPU" -p:Version="7.0.0"
    if %ERRORLEVEL% neq 0 (
        echo [ERROR] Build failed! Check compiler output above.
        pause
        exit /b 1
    )
) else (
    echo  -> dotnet CLI not found in PATH, searching for MSBuild...
    call :FindAndRunMsbuild
    if %ERRORLEVEL% neq 0 (
        echo [ERROR] Neither dotnet CLI nor MSBuild could build the solution.
        pause
        exit /b 1
    )
)

:: Verify output directory
if not exist "%BIN_DIR%" (
    if exist "%REPO_ROOT%bin\Release\Any CPU" (
        set "BIN_DIR=%REPO_ROOT%bin\Release\Any CPU"
    )
)

:: 3. Create Portable Package
echo.
echo [2/3] Creating Portable ZIP Archive...
if exist "%BIN_DIR%" (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Compress-Archive -Path '%BIN_DIR%\*' -DestinationPath '%BUILD_DIR%\portable\EBUninstaller_Pro_Portable.zip' -Force; Write-Host ' -> Portable archive created: build\portable\EBUninstaller_Pro_Portable.zip'"
) else (
    echo [WARNING] Binary output directory not found at %BIN_DIR%
)

:: 4. Build Inno Setup Installer
echo.
echo [3/3] Compiling Inno Setup Installer...
set "ISCC_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if exist "%ISCC_PATH%" (
    "%ISCC_PATH%" "%REPO_ROOT%installer\EBUninstallSetup.iss"
    echo  -> Inno Setup installer compiled to: build\installer\
) else (
    set "ISCC_PATH64=C:\Program Files\Inno Setup 6\ISCC.exe"
    if exist "%ISCC_PATH64%" (
        "%ISCC_PATH64%" "%REPO_ROOT%installer\EBUninstallSetup.iss"
        echo  -> Inno Setup installer compiled to: build\installer\
    ) else (
        echo [INFO] Inno Setup compiler (ISCC.exe) was not found in default program files.
        echo        Script is ready at: installer\EBUninstallSetup.iss
    )
)

echo.
echo =================================================================
echo  EBUninstaller Pro - Build Process Completed Successfully!
echo =================================================================
pause
exit /b 0

:FindAndRunMsbuild
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if exist "%VSWHERE%" (
    for /f "usebackq tokens=*" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
        set "MSBUILD_EXE=%%i"
    )
)
if defined MSBUILD_EXE (
    "%MSBUILD_EXE%" "%SOLUTION%" /t:Restore;Build /p:Configuration=%CONFIG% /p:Platform="Any CPU" /verbosity:minimal
    exit /b %ERRORLEVEL%
)
exit /b 1
