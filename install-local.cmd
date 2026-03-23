@echo off
setlocal
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0install-local.ps1" %*
exit /b %errorlevel%
