[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',
    [ValidateSet('Auto', 'Setup', 'Local')]
    [string]$InstallMode = 'Auto',
    [switch]$SkipRunSetup,
    [switch]$SilentInstall,
    [switch]$SkipSdkAutoInstall,
    [switch]$SkipBuildToolsAutoInstall
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$script:DotnetSandboxAppData = $null
$script:DotnetSandboxNugetPackages = $null
$script:DotnetSandboxNugetHttpCache = $null
$script:DotnetSandboxNugetPluginsCache = $null
$script:DotnetSandboxNugetScratch = $null
$script:DotnetCliPath = $null
$script:DotnetSdkVersion = $null
$script:DotnetSdksPath = $null
$script:DotnetRoot = $null
$script:DotnetSandboxNugetConfig = $null

function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Initialize-DotnetEnvironment {
    $cliHome = Join-Path $PSScriptRoot '.dotnet-cli-home'
    $dotnetHome = Join-Path $PSScriptRoot '.dotnet-home'
    $nugetRoot = Join-Path $PSScriptRoot '.nuget'
    $dotnetHomeRoaming = Join-Path $dotnetHome 'Roaming'
    $nugetPackages = Join-Path $nugetRoot 'packages'
    $nugetHttpCache = Join-Path $nugetRoot 'http-cache'
    $nugetPluginsCache = Join-Path $nugetRoot 'plugins-cache'
    $nugetScratch = Join-Path $nugetRoot 'scratch'
    $nugetConfig = Join-Path $nugetRoot 'NuGet.Config'

    if (-not (Test-Path -LiteralPath $cliHome)) {
        New-Item -ItemType Directory -Path $cliHome -Force | Out-Null
    }
    foreach ($dir in @($dotnetHomeRoaming, $nugetPackages, $nugetHttpCache, $nugetPluginsCache, $nugetScratch)) {
        if (-not (Test-Path -LiteralPath $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
        }
    }
    if (-not (Test-Path -LiteralPath $nugetConfig)) {
        @'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
'@ | Set-Content -LiteralPath $nugetConfig -Encoding ASCII
    }

    $env:DOTNET_CLI_HOME = $cliHome
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    $env:DOTNET_NOLOGO = '1'

    $script:DotnetSandboxAppData = $dotnetHomeRoaming
    $script:DotnetSandboxNugetPackages = $nugetPackages
    $script:DotnetSandboxNugetHttpCache = $nugetHttpCache
    $script:DotnetSandboxNugetPluginsCache = $nugetPluginsCache
    $script:DotnetSandboxNugetScratch = $nugetScratch
    $script:DotnetSandboxNugetConfig = $nugetConfig
}

function Invoke-ProcessWithSandbox {
    param(
        [Parameter(Mandatory = $true)][string]$ToolPath,
        [Parameter(Mandatory = $true)][string[]]$Arguments
    )

    $envBackup = @{
        APPDATA = $env:APPDATA
        NUGET_PACKAGES = $env:NUGET_PACKAGES
        NUGET_HTTP_CACHE_PATH = $env:NUGET_HTTP_CACHE_PATH
        NUGET_PLUGINS_CACHE_PATH = $env:NUGET_PLUGINS_CACHE_PATH
        NUGET_SCRATCH = $env:NUGET_SCRATCH
        DOTNET_ROOT = $env:DOTNET_ROOT
        DOTNET_HOST_PATH = $env:DOTNET_HOST_PATH
        DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR = $env:DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR
        MSBuildSDKsPath = $env:MSBuildSDKsPath
        MSBuildEnableWorkloadResolver = $env:MSBuildEnableWorkloadResolver
    }

    try {
        if ($script:DotnetSandboxAppData) { $env:APPDATA = $script:DotnetSandboxAppData }
        if ($script:DotnetSandboxNugetPackages) { $env:NUGET_PACKAGES = $script:DotnetSandboxNugetPackages }
        if ($script:DotnetSandboxNugetHttpCache) { $env:NUGET_HTTP_CACHE_PATH = $script:DotnetSandboxNugetHttpCache }
        if ($script:DotnetSandboxNugetPluginsCache) { $env:NUGET_PLUGINS_CACHE_PATH = $script:DotnetSandboxNugetPluginsCache }
        if ($script:DotnetSandboxNugetScratch) { $env:NUGET_SCRATCH = $script:DotnetSandboxNugetScratch }
        if ($script:DotnetRoot) { $env:DOTNET_ROOT = $script:DotnetRoot }
        if ($script:DotnetCliPath) { $env:DOTNET_HOST_PATH = $script:DotnetCliPath }
        if ($script:DotnetRoot) { $env:DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR = $script:DotnetRoot }
        if ($script:DotnetSdksPath) { $env:MSBuildSDKsPath = $script:DotnetSdksPath }
        $env:MSBuildEnableWorkloadResolver = 'false'

        & $ToolPath @Arguments | Out-Host
        return [int]$LASTEXITCODE
    }
    finally {
        foreach ($pair in $envBackup.GetEnumerator()) {
            if ($null -eq $pair.Value) {
                Remove-Item -Path ("Env:{0}" -f $pair.Key) -ErrorAction SilentlyContinue
            }
            else {
                Set-Item -Path ("Env:{0}" -f $pair.Key) -Value $pair.Value
            }
        }
    }
}

function Invoke-DotnetWithSandbox {
    param(
        [Parameter(Mandatory = $true)][string]$DotnetPath,
        [Parameter(Mandatory = $true)][string[]]$Arguments
    )

    return Invoke-ProcessWithSandbox -ToolPath $DotnetPath -Arguments $Arguments
}

function Resolve-DotnetCli {
    $dotnetCmd = Get-Command dotnet.exe -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -ne $dotnetCmd) {
        return [string]$dotnetCmd.Source
    }

    $defaultPath = 'C:\Program Files\dotnet\dotnet.exe'
    if (Test-Path -LiteralPath $defaultPath) {
        return $defaultPath
    }

    return $null
}

function Set-DotnetSdkContext {
    param([Parameter(Mandatory = $true)][string]$DotnetPath)

    $sdkVersion = (& $DotnetPath --version 2>$null)
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace(($sdkVersion | Out-String))) {
        return
    }

    $sdkVersion = $sdkVersion.ToString().Trim()
    $dotnetRoot = Split-Path -Parent $DotnetPath
    $sdksPath = Join-Path $dotnetRoot ("sdk\{0}\Sdks" -f $sdkVersion)
    if (-not (Test-Path -LiteralPath (Join-Path $sdksPath 'Microsoft.NET.Sdk\Sdk'))) {
        return
    }

    $script:DotnetCliPath = $DotnetPath
    $script:DotnetSdkVersion = $sdkVersion
    $script:DotnetSdksPath = $sdksPath
    $script:DotnetRoot = $dotnetRoot
}

function Has-DotnetSdk {
    param([Parameter(Mandatory = $true)][string]$DotnetPath)

    try {
        $sdks = & $DotnetPath --list-sdks 2>$null
        return ($LASTEXITCODE -eq 0 -and [string]::IsNullOrWhiteSpace(($sdks | Out-String)) -eq $false)
    }
    catch {
        return $false
    }
}

function Install-DotnetSdkWithWinget {
    $wingetCmd = Get-Command winget.exe -ErrorAction SilentlyContinue
    if ($null -eq $wingetCmd) {
        throw "dotnet SDK missing and winget is not available. Install .NET 8 SDK manually."
    }

    Write-Step "Installing .NET 8 SDK with winget (one-time prerequisite)"
    & $wingetCmd.Source install --id Microsoft.DotNet.SDK.8 --exact --silent --disable-interactivity --accept-package-agreements --accept-source-agreements | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "winget failed to install .NET 8 SDK (exit code $LASTEXITCODE)."
    }
}

function Ensure-DotnetSdk {
    $dotnet = Resolve-DotnetCli
    if ($dotnet -is [Array]) { $dotnet = $dotnet | Select-Object -First 1 }
    if ($dotnet) { $dotnet = [string]$dotnet }
    if ($dotnet -and (Has-DotnetSdk -DotnetPath $dotnet)) {
        Set-DotnetSdkContext -DotnetPath $dotnet
        return [string]$dotnet
    }

    if ($SkipSdkAutoInstall) {
        throw "No .NET SDK found. Remove -SkipSdkAutoInstall or install .NET 8 SDK manually."
    }

    Install-DotnetSdkWithWinget

    $dotnet = Resolve-DotnetCli
    if ($dotnet -is [Array]) { $dotnet = $dotnet | Select-Object -First 1 }
    if ($dotnet) { $dotnet = [string]$dotnet }
    if (-not $dotnet -or -not (Has-DotnetSdk -DotnetPath $dotnet)) {
        throw "SDK install completed but dotnet SDK is still not visible in this shell. Open a new terminal and rerun."
    }

    Set-DotnetSdkContext -DotnetPath $dotnet
    return [string]$dotnet
}

function Install-BuildToolsWithWinget {
    $wingetCmd = Get-Command winget.exe -ErrorAction SilentlyContinue
    if ($null -eq $wingetCmd) {
        throw "MSBuild is missing and winget is not available. Install Visual Studio 2022 Build Tools manually."
    }

    $overrideArgs = @(
        '--quiet',
        '--wait',
        '--norestart',
        '--nocache',
        '--add', 'Microsoft.VisualStudio.Workload.ManagedDesktopBuildTools',
        '--add', 'Microsoft.VisualStudio.Workload.VCTools',
        '--add', 'Microsoft.VisualStudio.Component.VC.Tools.x86.x64',
        '--add', 'Microsoft.VisualStudio.Component.VC.ATLMFC',
        '--add', 'Microsoft.VisualStudio.Component.Windows10SDK.18362'
    )

    Write-Step "Installing Visual Studio 2022 Build Tools with required workloads"
    & $wingetCmd.Source install `
        --id Microsoft.VisualStudio.2022.BuildTools `
        --exact `
        --silent `
        --disable-interactivity `
        --accept-package-agreements `
        --accept-source-agreements `
        --override ($overrideArgs -join ' ') | Out-Host

    if ($LASTEXITCODE -ne 0) {
        throw "winget failed to install Visual Studio 2022 Build Tools (exit code $LASTEXITCODE)."
    }
}

function Resolve-VsWhere {
    $paths = @(
        'C:\Program Files\Microsoft Visual Studio\Installer\vswhere.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe'
    )

    foreach ($path in $paths) {
        if (Test-Path -LiteralPath $path) {
            return $path
        }
    }

    return $null
}

function Resolve-FullMsBuild {
    if ($env:BCU_MSBUILD_PATH -and (Test-Path -LiteralPath $env:BCU_MSBUILD_PATH)) {
        return [string]$env:BCU_MSBUILD_PATH
    }

    $msbuildCmd = Get-Command msbuild.exe -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -ne $msbuildCmd) {
        return [string]$msbuildCmd.Source
    }

    $vswhere = Resolve-VsWhere
    if ($vswhere) {
        foreach ($pattern in @('MSBuild\**\Bin\amd64\MSBuild.exe', 'MSBuild\**\Bin\MSBuild.exe')) {
            $match = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find $pattern 2>$null | Select-Object -First 1
            if ($match -and (Test-Path -LiteralPath $match)) {
                return [string]$match
            }
        }
    }

    $candidates = @(
        'C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
    )

    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    return $null
}

function Ensure-FullMsBuild {
    $msbuild = Resolve-FullMsBuild
    if ($msbuild) {
        return [string]$msbuild
    }

    if (-not $SkipBuildToolsAutoInstall) {
        Install-BuildToolsWithWinget
        $msbuild = Resolve-FullMsBuild
        if ($msbuild) {
            return [string]$msbuild
        }

        throw "Visual Studio Build Tools installation finished, but MSBuild.exe is still not visible. Open a new terminal and rerun this script."
    }

    $message = @"
Full MSBuild is required to publish this repository.

Reason:
- source\UninstallTools\UninstallTools.csproj uses COMReference items.
- dotnet publish runs on MSBuild Core and fails with MSB4803 for this project graph.

Install one of these, then reopen the terminal and rerun this script:
- Visual Studio 2022 Build Tools with MSBuild support
- Visual Studio 2022 Community/Professional/Enterprise

If MSBuild.exe is installed in a custom location, set BCU_MSBUILD_PATH to its full path before running this script.
"@

    throw $message.Trim()
}

function Resolve-Iscc {
    $isccCmd = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($null -ne $isccCmd) { return $isccCmd.Source }

    $paths = @(
        'C:\Program Files (x86)\Inno Setup 6\ISCC.exe',
        'C:\Program Files\Inno Setup 6\ISCC.exe'
    )

    foreach ($path in $paths) {
        if (Test-Path -LiteralPath $path) {
            return $path
        }
    }

    return $null
}

function Publish-App {
    param(
        [Parameter(Mandatory = $true)][string]$MsBuildPath,
        [Parameter(Mandatory = $true)][string]$SolutionPath,
        [Parameter(Mandatory = $true)][string]$OutputDir,
        [Parameter(Mandatory = $true)][string]$ConfigurationValue
    )

    if (Test-Path -LiteralPath $OutputDir) {
        Remove-Item -LiteralPath $OutputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

    $publishDir = [System.IO.Path]::GetFullPath($OutputDir)
    if (-not $publishDir.EndsWith('\')) {
        $publishDir += '\'
    }

    $targetFramework = 'net8.0-windows10.0.18362.0'
    $platform = 'Any CPU'

    $publishArgs = @(
        $SolutionPath,
        '/restore',
        '/p:filealignment=512',
        '/t:Publish',
        '/p:DeployOnBuild=true',
        '/p:PublishSingleFile=False',
        '/p:SelfContained=False',
        '/p:PublishProtocol=FileSystem',
        "/p:Configuration=$ConfigurationValue",
        "/p:Platform=$platform",
        "/p:TargetFrameworks=$targetFramework",
        "/p:PublishDir=$publishDir",
        "/p:RestoreConfigFile=$script:DotnetSandboxNugetConfig",
        '/p:PublishReadyToRun=false',
        '/p:PublishTrimmed=False',
        '/p:RestoreUseStaticGraphEvaluation=false',
        '/verbosity:minimal'
    )

    $publishExitCode = Invoke-ProcessWithSandbox -ToolPath $MsBuildPath -Arguments $publishArgs
    if ($publishExitCode -ne 0) {
        throw "MSBuild publish failed (exit code $publishExitCode). Check output above for details."
    }

    $mainExe = Join-Path $OutputDir 'BCUninstaller.exe'
    if (-not (Test-Path -LiteralPath $mainExe)) {
        throw "Publish completed but BCUninstaller.exe was not found in $OutputDir"
    }
}

function New-Shortcut {
    param(
        [Parameter(Mandatory = $true)][string]$ShortcutPath,
        [Parameter(Mandatory = $true)][string]$TargetPath,
        [Parameter(Mandatory = $true)][string]$WorkingDirectory
    )

    $wsh = New-Object -ComObject WScript.Shell
    $shortcut = $wsh.CreateShortcut($ShortcutPath)
    $shortcut.TargetPath = $TargetPath
    $shortcut.WorkingDirectory = $WorkingDirectory
    $shortcut.IconLocation = "$TargetPath,0"
    $shortcut.Description = 'BCUninstaller local build'
    $shortcut.Save()
}

function Install-LocalBuild {
    param(
        [Parameter(Mandatory = $true)][string]$PublishDir,
        [switch]$RunAfterInstall
    )

    $installDir = Join-Path $env:LOCALAPPDATA 'Programs\BCUninstaller-Local'
    $settingsFile = Join-Path $installDir 'BCUninstaller.settings'
    $settingsBackup = $null

    Write-Step "Installing local build to $installDir"
    if (-not (Test-Path -LiteralPath $installDir)) {
        New-Item -ItemType Directory -Path $installDir -Force | Out-Null
    }

    if (Test-Path -LiteralPath $settingsFile) {
        $settingsBackup = Join-Path $env:TEMP ('BCU-settings-' + [Guid]::NewGuid().ToString('N') + '.settings')
        Copy-Item -LiteralPath $settingsFile -Destination $settingsBackup -Force
    }

    Get-ChildItem -LiteralPath $installDir -Force | Where-Object { $_.Name -ne 'BCUninstaller.settings' } | Remove-Item -Recurse -Force
    Get-ChildItem -LiteralPath $PublishDir -Force | Copy-Item -Destination $installDir -Recurse -Force

    if ($settingsBackup -and (Test-Path -LiteralPath $settingsBackup)) {
        Copy-Item -LiteralPath $settingsBackup -Destination $settingsFile -Force
        Remove-Item -LiteralPath $settingsBackup -Force
    }

    $exePath = Join-Path $installDir 'BCUninstaller.exe'
    if (-not (Test-Path -LiteralPath $exePath)) {
        throw "Local install copy failed: BCUninstaller.exe was not found in $installDir"
    }

    $desktopShortcut = Join-Path ([Environment]::GetFolderPath('DesktopDirectory')) 'BCUninstaller Local Build.lnk'
    $startMenuShortcut = Join-Path ([Environment]::GetFolderPath('StartMenu')) 'Programs\BCUninstaller Local Build.lnk'

    New-Shortcut -ShortcutPath $desktopShortcut -TargetPath $exePath -WorkingDirectory $installDir
    New-Shortcut -ShortcutPath $startMenuShortcut -TargetPath $exePath -WorkingDirectory $installDir

    Write-Host "Desktop icon created: $desktopShortcut" -ForegroundColor Green
    Write-Host "Start menu shortcut created: $startMenuShortcut" -ForegroundColor Green

    if ($RunAfterInstall) {
        Start-Process -FilePath $exePath
    }
}

$repoRoot = $PSScriptRoot
$solutionPath = Join-Path $repoRoot 'source\BulkCrapUninstaller.sln'
$setupPublishDir = Join-Path $repoRoot 'bin\publish-AnyCPU-net8.0'
$localPublishDir = Join-Path $repoRoot 'bin\publish-local'
$issPath = Join-Path $repoRoot 'installer\BcuSetup.iss'

if (-not (Test-Path -LiteralPath $solutionPath)) {
    throw "Solution not found: $solutionPath"
}

Initialize-DotnetEnvironment

$iscc = Resolve-Iscc
if ($InstallMode -eq 'Auto') {
    $InstallMode = if ($iscc) { 'Setup' } else { 'Local' }
}

$dotnet = Ensure-DotnetSdk
$msbuild = Ensure-FullMsBuild

switch ($InstallMode) {
    'Setup' {
        if (-not $iscc) {
            throw "Inno Setup (ISCC.exe) not found. Install Inno Setup 6 or use -InstallMode Local."
        }

        Write-Step "Publishing app for setup"
        Publish-App -MsBuildPath $msbuild -SolutionPath $solutionPath -OutputDir $setupPublishDir -ConfigurationValue $Configuration

        if (-not (Test-Path -LiteralPath $issPath)) {
            throw "Installer script not found: $issPath"
        }

        Write-Step "Compiling setup with ISCC: $iscc"
        & $iscc $issPath
        if ($LASTEXITCODE -ne 0) {
            throw "ISCC failed (exit code $LASTEXITCODE)."
        }

        $setupCandidates = @(
            Get-ChildItem -Path (Join-Path $repoRoot 'installer') -Filter 'BCUninstaller_*_setup.exe' -File -ErrorAction SilentlyContinue
            Get-ChildItem -Path $repoRoot -Filter 'BCUninstaller_*_setup.exe' -File -ErrorAction SilentlyContinue
        ) | Sort-Object LastWriteTime -Descending

        $setup = $setupCandidates | Select-Object -First 1
        if ($null -eq $setup) {
            throw "Setup compiled but .exe was not found."
        }

        Write-Step "Setup ready: $($setup.FullName)"
        if ($SkipRunSetup) {
            Write-Host "Setup launch skipped (-SkipRunSetup)." -ForegroundColor Yellow
            break
        }

        Write-Step "Running setup"
        $setupArgs = @()
        if ($SilentInstall) {
            $setupArgs += '/VERYSILENT'
            $setupArgs += '/SUPPRESSMSGBOXES'
            $setupArgs += '/NORESTART'
        }

        Start-Process -FilePath $setup.FullName -Verb RunAs -ArgumentList $setupArgs -Wait
        Write-Host "Setup installation completed." -ForegroundColor Green
    }
    'Local' {
        Write-Step "Publishing local build"
        Publish-App -MsBuildPath $msbuild -SolutionPath $solutionPath -OutputDir $localPublishDir -ConfigurationValue $Configuration
        Install-LocalBuild -PublishDir $localPublishDir -RunAfterInstall:(!$SkipRunSetup)
        Write-Host "Local install completed and shortcut created." -ForegroundColor Green
    }
}
