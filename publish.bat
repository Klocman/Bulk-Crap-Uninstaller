
rem Release Debug
set config=Release

set msbuild="D:\Applications\VS2022\MSBuild\Current\Bin\amd64\MSBuild.exe"
set publish=%CD%\bin\publish


rmdir /q /s "%publish%"
if errorlevel 1 (pause)
rmdir /q /s bin\launcher
if errorlevel 1 (pause)


set platform=x64
call :publish

set platform=x86
call :publish

copy bin\launcher\BCU-launcher.exe %publish%\BCUninstaller.exe
copy "%target%\BCU_manual.html" "%publish%\BCU_manual.html"
copy "%target%\Licence.txt" "%publish%\Licence.txt"
copy "%target%\PrivacyPolicy.txt" "%publish%\PrivacyPolicy.txt"
copy "%target%\NOTICE" "%publish%\NOTICE"

rmdir /q /s bin\launcher

IF %config%==Release (del /f /s /q "%publish%\*.pdb")

pause
exit



rem -------------------------------------------------------------

:publish
set identifier=win-%platform%
set target=%CD%\bin\publish\%identifier%

%msbuild% "source\BulkCrapUninstaller.sln" /m /p:filealignment=512 /t:Restore;Rebuild /p:DeployOnBuild=true /p:PublishSingleFile=False /p:SelfContained=True /p:PublishProtocol=FileSystem /p:Configuration=%config% /p:Platform=%platform% /p:TargetFrameworks=net6.0-windows10.0.18362.0 /p:PublishDir="%target%" /p:RuntimeIdentifier=%identifier% /p:PublishReadyToRun=false /p:PublishTrimmed=False /verbosity:minimal

%msbuild% "source\BulkCrapUninstaller.sln" /m /p:filealignment=512 /t:Publish /p:DeployOnBuild=true /p:PublishSingleFile=False /p:SelfContained=True /p:PublishProtocol=FileSystem /p:Configuration=%config% /p:Platform=%platform% /p:TargetFrameworks=net6.0-windows10.0.18362.0 /p:PublishDir="%target%" /p:RuntimeIdentifier=%identifier% /p:PublishReadyToRun=false /p:PublishTrimmed=False /verbosity:minimal

goto :eof

rem -------------------------------------------------------------