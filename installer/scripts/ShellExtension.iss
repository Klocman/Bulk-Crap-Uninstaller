[Run]
Filename: "{win}\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe"; Parameters: "/nologo /codebase ""{app}\win-x64\ContextMenuExtension.dll"""

[UninstallRun]
Filename: "{win}\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe"; Parameters: "/nologo /unregister ""{app}\win-x64\ContextMenuExtension.dll"""
Filename: "{sys}\taskkill"; Parameters: "/F /IM explorer.exe"
Filename: "{win}\explorer.exe";
