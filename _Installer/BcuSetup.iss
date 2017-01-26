#include "InstallDependancies.iss"

#define MyAppName "Bulk Crap Uninstaller"      
#define MyAppNameShort "BCUninstaller"   
#define MyAppPublisher "Marcin Szeniak"
#define MyAppURL "http://klocmansoftware.weebly.com/"
#define MyAppExeName "BCUninstaller.exe" 
#define MyAppCopyright "Copyright 2017 Marcin Szeniak"
                                
#define MyAppVersion "3.6.3.0"     
#define MyAppVersionShort "3.6.3"

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

[Components]
Name: "main"; Description: "Main Files"; Types: full compact custom; Flags: fixed
Name: "lang"; Description: "Languages"; Types: full
Name: "lang\en"; Description: "English"; Types: full compact custom; Flags: fixed
Name: "lang\cs"; Description: "Czech"; Types: full           
Name: "lang\de"; Description: "German"; Types: full
Name: "lang\es"; Description: "Spanish"; Types: full
Name: "lang\fr"; Description: "French"; Types: full
Name: "lang\hu"; Description: "Hungarian"; Types: full
Name: "lang\it"; Description: "Italian"; Types: full
Name: "lang\pl"; Description: "Polish"; Types: full
Name: "lang\pt"; Description: "Portuguese"; Types: full
Name: "lang\ru"; Description: "Russian"; Types: full
Name: "lang\sl"; Description: "Slovene"; Types: full

[Files]                                           
Source: "Input\BCUninstaller.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion               
Source: "Input\BCU_manual.html"; DestDir: "{app}"; Components: main; Flags: ignoreversion isreadme
Source: "Input\*"; DestDir: "{app}"; Components: main; Flags: ignoreversion   

Source: "Input\cs\*"; DestDir: "{app}"; Components: lang\cs; Flags: ignoreversion  
Source: "Input\de\*"; DestDir: "{app}"; Components: lang\de; Flags: ignoreversion  
Source: "Input\es\*"; DestDir: "{app}"; Components: lang\es; Flags: ignoreversion  
Source: "Input\fr\*"; DestDir: "{app}"; Components: lang\fr; Flags: ignoreversion  
Source: "Input\hu\*"; DestDir: "{app}"; Components: lang\hu; Flags: ignoreversion  
Source: "Input\it\*"; DestDir: "{app}"; Components: lang\it; Flags: ignoreversion  
Source: "Input\pl\*"; DestDir: "{app}"; Components: lang\pl; Flags: ignoreversion  
Source: "Input\pt\*"; DestDir: "{app}"; Components: lang\pt; Flags: ignoreversion  
Source: "Input\ru\*"; DestDir: "{app}"; Components: lang\ru; Flags: ignoreversion  
Source: "Input\sl\*"; DestDir: "{app}"; Components: lang\sl; Flags: ignoreversion  
                                                                   
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec
        