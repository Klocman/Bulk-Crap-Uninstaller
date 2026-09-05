<#
.SYNOPSIS
    EBUninstaller Pro - Setup & Release Installer Builder
.DESCRIPTION
    Builds binaries in Release mode, runs tests, creates portable archive,
    compiles Inno Setup installer, and hashes release artifacts.
#>

[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$Platform = "Any CPU",
    [switch]$SkipTests,
    [string]$InnoCompilerPath = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Split-Path -Parent $ScriptDir

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host " EBUninstaller Pro - Release Setup & Distribution Builder        " -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

# Unblock files
Get-ChildItem -Path $RepoRoot -Recurse -ErrorAction SilentlyContinue | Unblock-File -ErrorAction SilentlyContinue

# Ensure output directories exist
$BuildDir = Join-Path $RepoRoot "build"
$PortableDir = Join-Path $BuildDir "portable"
$InstallerDir = Join-Path $BuildDir "installer"
New-Item -ItemType Directory -Force -Path $PortableDir | Out-Null
New-Item -ItemType Directory -Force -Path $InstallerDir | Out-Null

# 1. Compile Solution
Write-Host "`n[1/4] Compiling Solution ($Configuration - $Platform)..." -ForegroundColor Yellow
$SolutionFile = Join-Path $RepoRoot "source\EBUninstaller.sln"
dotnet build $SolutionFile -c $Configuration /p:Platform="$Platform" /p:Version="7.0.0"

# 2. Run Tests
if (-not $SkipTests) {
    Write-Host "`n[2/4] Running Automated Test Suite..." -ForegroundColor Yellow
    $TestProj = Join-Path $RepoRoot "source\EBUninstallerTests\EBUninstallerTests.csproj"
    dotnet test $TestProj -c $Configuration --no-build --logger "console;verbosity=normal"
}

# 3. Create Portable Zip
Write-Host "`n[3/4] Packaging Portable Release (.ZIP)..." -ForegroundColor Yellow
$BinDir = Join-Path $RepoRoot "bin\$Configuration\AnyCPU"
if (-not (Test-Path $BinDir)) {
    $BinDir = Join-Path $RepoRoot "bin\$Configuration\$Platform"
}

if (Test-Path $BinDir) {
    $ZipPath = Join-Path $PortableDir "EBUninstaller_Pro_Portable.zip"
    Compress-Archive -Path "$BinDir\*" -DestinationPath $ZipPath -Force
    Write-Host " -> Portable package created: $ZipPath" -ForegroundColor Green
} else {
    Write-Warning "Bin output directory not found: $BinDir"
}

# 4. Compile Inno Setup Installer
Write-Host "`n[4/4] Compiling Inno Setup Release Installer..." -ForegroundColor Yellow
$IssScript = Join-Path $RepoRoot "installer\EBUninstallSetup.iss"

$InnoPaths = @(
    $InnoCompilerPath,
    (Get-Command iscc.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source -ErrorAction SilentlyContinue),
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    "C:\Program Files\Inno Setup 6\ISCC.exe",
    "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
    "C:\ProgramData\chocolatey\bin\iscc.exe",
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and (Test-Path $_) }

if ($InnoPaths.Count -gt 0) {
    $Iscc = $InnoPaths[0]
    Write-Host " -> Using Inno Setup compiler: $Iscc" -ForegroundColor Cyan
    & $Iscc $IssScript
    if ($LASTEXITCODE -eq 0) {
        Write-Host " -> Setup installer built successfully: build\installer\EBUninstallSetup.exe" -ForegroundColor Green
    } else {
        Write-Warning "Inno Setup compiler exited with code $LASTEXITCODE"
    }
} else {
    Write-Warning "Inno Setup compiler (ISCC.exe) not found."
    Write-Host "To compile the installer setup:" -ForegroundColor Yellow
    Write-Host " 1. Install Inno Setup 6 (https://jrsoftware.org/isdl.php)"
    Write-Host " 2. Open and compile: $IssScript"
}

# 5. Checksums
$SumsFile = Join-Path $BuildDir "SHA256SUMS.txt"
$Artifacts = Get-ChildItem -Path $BuildDir -Recurse -File -Exclude "SHA256SUMS.txt"
if ($Artifacts.Count -gt 0) {
    $Hashes = foreach ($f in $Artifacts) {
        $h = (Get-FileHash -Path $f.FullName -Algorithm SHA256).Hash.ToLower()
        $rel = $f.FullName.Substring($BuildDir.Length + 1)
        "$h  $rel"
    }
    $Hashes | Set-Content -Path $SumsFile -Encoding UTF8
    Write-Host "`nRelease artifact hashes written to: $SumsFile" -ForegroundColor Cyan
}

Write-Host "`n=================================================================" -ForegroundColor Green
Write-Host " [SUCCESS] Setup building process completed!                     " -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green
