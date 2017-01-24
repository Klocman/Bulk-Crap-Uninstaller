title Removing BCU log files...
@echo off
cls

if not exist %LOCALAPPDATA% (
REM LOCALAPPDATA doesn't exist on windows older than Vista
set MSLOGDIR=%APPDATA%\Microsoft
) else (
set MSLOGDIR=%LOCALAPPDATA%\Microsoft
)

REM Wait for BCU to exit so the logs are written
echo Waiting for BCUninstaller to exit...
:loop
tasklist | find "BCUninstaller.exe " >nul
if not errorlevel 1 (
	REM Wait for 1 second before checking again
    ping -n 2 127.0.0.1 >nul
    goto :loop
)

REM Wait some more to be safe
ping -n 2 127.0.0.1 >nul

REM Move to the log dir and remove all logs. They can be under following directories:
REM \CLR_v2.0\UsageLogs \CLR_v2.0_32\UsageLogs \CLR_v4.0\UsageLogs \CLR_v4.0_32\UsageLogs
cd /d "%MSLOGDIR%"

echo Deleting Assembly Usage Logs...
del /f /s BCUninstaller.exe.log SteamHelper.exe.log StoreAppHelper.exe.log UninstallerAutomatizer.exe.log UpdateHelper.exe.log

exit