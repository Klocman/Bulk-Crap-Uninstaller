[CmdletBinding()]
param([string]$DotNet10 = 'dotnet')
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path $PSScriptRoot).Path
$project = Join-Path $root 'source/BulkCrapUninstaller/BulkCrapUninstaller.csproj'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'Visual Studio MSBuild with the desktop/C++ build tools is required.' }
$sdk8 = & dotnet --list-sdks | Where-Object { $_ -match '^8\.0\.' } | Select-Object -Last 1
if ($sdk8 -notmatch '^(\S+) \[(.+)\]$') { throw 'A .NET 8 SDK is required to build the original COM dependencies.' }
$sdk8Path = Join-Path $Matches[2] $Matches[1]
$version = & $DotNet10 --version
if ($LASTEXITCODE -ne 0 -or $version -notmatch '^10\.') { throw 'Pass -DotNet10 with a .NET 10 dotnet executable.' }
$savedSdk = $env:MSBuildSDKsPath
$savedResolver = $env:MSBuildEnableWorkloadResolver
try {
    $env:MSBuildSDKsPath = Join-Path $sdk8Path 'Sdks'
    $env:MSBuildEnableWorkloadResolver = 'false'
    & $msbuild $project /restore /p:Configuration=Release /nologo /v:minimal
    if ($LASTEXITCODE -ne 0) { throw 'Default .NET 8 build failed.' }
} finally {
    $env:MSBuildSDKsPath = $savedSdk
    $env:MSBuildEnableWorkloadResolver = $savedResolver
}
# Dependencies were built above by VS MSBuild (required by ResolveComReference).
# The feature flag changes the GUI project only, leaving those targets at .NET 8.
& $DotNet10 publish $project -c Release -p:EnableNativeDarkMode=true -p:BuildProjectReferences=false -o (Join-Path $root 'bin/NativeDark/publish') --nologo -v minimal
if ($LASTEXITCODE -ne 0) { throw 'Opt-in .NET 10 publish failed.' }
