; EBUninstaller Pro - Modern Inno Setup Installer Script
; Professional Next-Generation Windows Uninstaller

#define MyAppName "EBUninstaller Pro"
#define MyAppVersion "7.0.0"
#define MyAppPublisher "EBUninstaller Project"
#define MyAppURL "https://github.com/EhabYT/Bulk-Crap-Uninstaller"
#define MyAppExeName "BCUninstaller.exe"
#define MyAppConsoleExeName "BCU-console.exe"

[Setup]
AppId={{D3F9E17A-72A8-4A1B-8DF9-E342B286E90F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\Licence.txt
OutputDir=..\build\installer
OutputBaseFilename=EBUninstaller_Pro_v{#MyAppVersion}_Setup
SetupIconFile=assets\logo.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible arm64
ArchitecturesInstallIn64BitMode=x64compatible arm64
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=commandline dialog
UsedUserAreasWarning=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
Source: "..\bin\Release\AnyCPU\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName} (Command Line)"; Filename: "{app}\{#MyAppConsoleExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}\Logs"
Type: filesandordirs; Name: "{app}\InfoCache.xml"
Type: filesandordirs; Name: "{app}\CertCache.xml"
