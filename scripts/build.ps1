<#
.SYNOPSIS
    EBUninstaller Pro - Automated Build and Release Script for Windows (.NET 8)
.DESCRIPTION
    Builds the solution in Release mode (x64, ARM64, AnyCPU), runs unit/integration tests,
    generates portable ZIP packages, and builds the Inno Setup installer.
#>

param(
    [string]$Configuration = "Release",
    [string]$Platform = "AnyCPU",
    [switch]$SkipTests,
    [switch]$BuildInstaller
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir
$SolutionPath = Join-Path $RepoRoot "source\BulkCrapUninstaller.sln"
$OutputDir = Join-Path $RepoRoot "bin\$Configuration\$Platform"
$BuildDir = Join-Path $RepoRoot "build"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " Building EBUninstaller Pro ($Configuration - $Platform)        " -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

# 1. Check dotnet CLI
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet CLI is not found in PATH. Please install .NET 8 SDK."
}

# 2. Build Solution
Write-Host "`n[1/4] Compiling Solution..." -ForegroundColor Yellow
dotnet build $SolutionPath -c $Configuration /p:Platform=$Platform

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
}

# 3. Run Unit and Integration Tests
if (-not $SkipTests) {
    Write-Host "`n[2/4] Executing Test Suite..." -ForegroundColor Yellow
    $TestProjectPath = Join-Path $RepoRoot "source\BulkCrapUninstallerTests\BulkCrapUninstallerTests.csproj"
    dotnet test $TestProjectPath -c $Configuration --no-build --logger "console;verbosity=normal"

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Test execution failed!"
    }
} else {
    Write-Host "`n[2/4] Skipping tests (-SkipTests requested)" -ForegroundColor DarkGray
}

# 4. Generate Portable Package
Write-Host "`n[3/4] Creating Portable Release Package..." -ForegroundColor Yellow
$PortableZip = Join-Path $BuildDir "portable\EBUninstaller_Pro_Portable.zip"
New-Item -ItemType Directory -Force -Path (Join-Path $BuildDir "portable") | Out-Null

if (Test-Path $OutputDir) {
    Compress-Archive -Path "$OutputDir\*" -DestinationPath $PortableZip -Force
    Write-Host "Portable package created: $PortableZip" -ForegroundColor Green
}

# 5. Build Installer if requested
if ($BuildInstaller) {
    Write-Host "`n[4/4] Compiling Inno Setup Installer..." -ForegroundColor Yellow
    $InnoCompiler = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    $IssScript = Join-Path $RepoRoot "installer\EBUninstallSetup.iss"

    if (Test-Path $InnoCompiler) {
        & $InnoCompiler $IssScript
        Write-Host "Installer compilation completed." -ForegroundColor Green
    } else {
        Write-Warning "Inno Setup compiler (ISCC.exe) not found at default path. Skipping installer compilation."
    }
}

Write-Host "`nBuild process completed successfully!" -ForegroundColor Green
