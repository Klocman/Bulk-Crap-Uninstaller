#include "InstallDependancies.iss"

#define MyAppName "Bulk Crap Uninstaller"      
#define MyAppNameShort "BCUninstaller"   
#define MyAppPublisher "Marcin Szeniak"
#define MyAppURL "http://klocmansoftware.weebly.com/"
#define MyAppExeName "BCUninstaller.exe" 
#define MyAppCopyright "Copyright 2016 Marcin Szeniak"
                                
#define MyAppVersion "3.5.0.0"     
#define MyAppVersionShort "3.5"

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

[Files]                                           
Source: "Input\BCUninstaller.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Input\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec
        