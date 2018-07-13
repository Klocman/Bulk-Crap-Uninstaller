#include "InstallDependancies.iss"

#define MyAppName "BCUninstaller"      
#define MyAppNameShort "BCUninstaller"   
#define MyAppPublisher "Marcin Szeniak"
#define MyAppURL "http://klocmansoftware.weebly.com/"
#define MyAppExeName "BCUninstaller.exe" 
#define MyAppCopyright "Copyright 2017 Marcin Szeniak"
                                
#define MyAppVersion "4.5.0.0"     
#define MyAppVersionShort "4.5"

#include "Scripts\PortablePage.iss" 
#include "Scripts\PortableIcons.iss"
#include "Scripts\Ngen.iss"

[Setup]
AppId={{f4fef76c-1aa9-441c-af7e-d27f58d898d1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}   

WizardImageFile=bigImage.bmp  
WizardSmallImageFile=smallImage.bmp
SetupIconFile=logo.ico

AllowNoIcons=yes
LicenseFile=Input\Licence.txt
OutputBaseFilename={#MyAppNameShort}_{#MyAppVersionShort}_setup

Compression=lzma2/ultra
SolidCompression=yes

;MinVersion default value: "0,5.0 (Windows 2000+) if Unicode Inno Setup, else 4.0,4.0 (Windows 95+)"
MinVersion=0,5.0
PrivilegesRequired=admin
ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64

VersionInfoCompany={#MyAppPublisher}
;VersionInfoDescription=desc
VersionInfoCopyright={#MyAppCopyright}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoProductTextVersion={#MyAppVersion}       
VersionInfoVersion={#MyAppVersion}          
VersionInfoTextVersion={#MyAppVersion}

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"              
Name: "pl"; MessagesFile: "compiler:Languages\Polish.isl"               
Name: "de"; MessagesFile: "compiler:Languages\German.isl"              
Name: "hu"; MessagesFile: "compiler:Languages\Hungarian.isl"           
Name: "sl"; MessagesFile: "compiler:Languages\Slovenian.isl"           
Name: "nl"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"    
Name: "bpt"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Components]
Name: "main"; Description: "Main Files"; Types: full compact custom; Flags: fixed
Name: "lang"; Description: "Extra Languages"; Types: full

[Files]                                           
Source: "Input\BCUninstaller.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion               
Source: "Input\BCU_manual.html"; DestDir: "{app}"; Components: main; Flags: ignoreversion isreadme        
Source: "Input\CleanLogs.bat"; DestDir: "{app}"; Components: main; Check: IsPortable(); Flags: ignoreversion
Source: "Input\*"; DestDir: "{app}"; Components: main; Flags: ignoreversion; Excludes: "CleanLogs.bat"   

Source: "Input\*"; DestDir: "{app}"; Components: lang; Flags: ignoreversion recursesubdirs; Excludes: "CleanLogs.bat" 
                                                                   
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec
        
