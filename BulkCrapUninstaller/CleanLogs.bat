title Removing BCU log files...
@echo off
cls

if not exist %LOCALAPPDATA% (
REM LOCALAPPDATA doesn't exist on windows older than Vista
set APPDATASAFE=%APPDATA%
) else (
set APPDATASAFE=%LOCALAPPDATA%
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
cd /d "%APPDATASAFE%\Microsoft"

echo Deleting Assembly Usage Logs...
del /f /s BCUninstaller.exe.log SteamHelper.exe.log StoreAppHelper.exe.log UninstallerAutomatizer.exe.log UpdateHelper.exe.log

echo Deleting .NET generated drectories...
REM Settings directory automatically created by .NET
set SETTINGSDIR=%APPDATASAFE%\Marcin_Szeniak

if not exist %SETTINGSDIR% (
exit
)

REM Remove all directories related to BCU
for /d %%G in ("%SETTINGSDIR%\BCUninstaller*") do rd /s /q "%%~G"

REM Check if the settings dir is empty. If there are directories left, exit.
for /d %%i in ("%SETTINGSDIR%\*") do exit

REM Dir is empty, so remove it.
rd "%SETTINGSDIR%"

exit