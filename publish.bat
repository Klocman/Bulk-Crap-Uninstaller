set msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\amd64\MSBuild.exe"
set publish=%CD%\bin\publish
set target=%CD%\bin\publish

del /f /s /q "%target%"

%msbuild% "source\BulkCrapUninstaller.sln" /m /p:filealignment=512 /t:Restore;Rebuild /p:DeployOnBuild=true /p:PublishSingleFile=False /p:SelfContained=True /p:PublishProtocol=FileSystem /p:Configuration=Release /p=Platform:"Any CPU" /p:TargetFrameworks=net5.0-windows10.0.18362.0 /p:PublishDir="%target%" /p:RuntimeIdentifier=win-x64 /p:PublishReadyToRun=false /p:PublishTrimmed=False /verbosity:minimal

%msbuild% "source\BulkCrapUninstaller.sln" /m /p:filealignment=512 /t:Publish /p:DeployOnBuild=true /p:PublishSingleFile=False /p:SelfContained=True /p:PublishProtocol=FileSystem /p:Configuration=Release /p:Platform="Any CPU" /p:TargetFrameworks=net5.0-windows10.0.18362.0 /p:PublishDir="%target%" /p:RuntimeIdentifier=win-x64 /p:PublishReadyToRun=false /p:PublishTrimmed=False /verbosity:minimal

rem mv "%target%\BCU_manual.html" "%publish%\BCU_manual.html"
rem mv "%target%\Licence.txt" "%publish%\Licence.txt"
rem mv "%target%\PrivacyPolicy.txt" "%publish%\PrivacyPolicy.txt"

del /f /s /q "%target%\*.pdb"

pause