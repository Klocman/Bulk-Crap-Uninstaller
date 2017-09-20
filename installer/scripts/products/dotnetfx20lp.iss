//http://www.microsoft.com/downloads/details.aspx?familyid=92E0E1CE-8693-4480-84FA-7D85EEF59016

[CustomMessages]
de.dotnetfx20lp_title=.NET Framework 2.0 Sprachpaket: Deutsch
es.dotnetfx20lp_title=.NET Framework 2.0 paquete de idioma: Español

de.dotnetfx20lp_size=1,8 MB
es.dotnetfx20lp_size=1,8 MB

de.dotnetfx20lp_size_x64=3 MB
es.dotnetfx20lp_size_x64=2,9 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
de.dotnetfx20lp_lcid=1031
es.dotnetfx35lp_lcid=3082

de.dotnetfx20lp_url=http://download.microsoft.com/download/2/9/7/29768238-56c3-4ea6-abba-4c5246f2bc81/langpack.exe
de.dotnetfx20lp_url_x64=http://download.microsoft.com/download/2/e/f/2ef250ba-a868-4074-a4c9-249004866f87/langpack.exe
de.dotnetfx20lp_url_ia64=http://download.microsoft.com/download/8/9/8/898c5670-5e74-41c4-82fc-68dd837af627/langpack.exe
es.dotnetfx20lp_url=http://download.microsoft.com/download/e/a/a/eaaf696f-3bd4-4e05-a471-d488b29ee52b/langpack.exe
es.dotnetfx20lp_url_x64=http://download.microsoft.com/download/9/9/3/993fcd8e-2b70-4270-9bee-c5138bd9a384/langpack.exe


[Code]
procedure dotnetfx20lp();
begin
	if (ActiveLanguage() <> 'en') then begin
		if (not netfxinstalled(NetFx20, CustomMessage('dotnetfx20lp_lcid'))) then
			AddProduct('dotnetfx20' + GetArchitectureString() + '_' + ActiveLanguage() + '.exe',
				'/passive /norestart /lang:ENU',
				CustomMessage('dotnetfx20lp_title'),
				CustomMessage('dotnetfx20lp_size' + GetArchitectureString()),
				CustomMessage('dotnetfx20lp_url' + GetArchitectureString()),
				false, false);
	end;
end;
