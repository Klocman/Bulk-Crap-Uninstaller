@echo off
REM EBUninstaller Pro - Unblock Repository Files Batch Launcher
echo Unblocking all repository files...
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0unblock_files.ps1"
echo Done.
pause
