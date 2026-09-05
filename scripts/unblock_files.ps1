# EBUninstaller Pro - Unblock Repository Files Script
# Removes the Mark of the Web (Zone.Identifier Alternate Data Stream) from all source files.

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Definition
$rootPath = Resolve-Path (Join-Path $scriptPath "..")

Write-Host "Unblocking all files in: $rootPath" -ForegroundColor Cyan
Get-ChildItem -Path $rootPath -Recurse -File | ForEach-Object {
    Unblock-File -Path $_.FullName -ErrorAction SilentlyContinue
}
Write-Host "[SUCCESS] All files unblocked successfully. You can now build in Visual Studio." -ForegroundColor Green
