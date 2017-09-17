[CustomMessages]
de.dotnetfx35lp_title=.NET Framework 3.5 Sprachpaket: Deutsch
es.dotnetfx35lp_title=.NET Framework 3.5 paquete de idioma: Español

es.dotnetfx35lp_size=32,1 MB - 43,3 MB
de.dotnetfx35lp_size=32,5 MB - 44 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
de.dotnetfx35lp_lcid=1031
es.dotnetfx35lp_lcid=3082

de.dotnetfx35lp_url=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x86de.exe 
de.dotnetfx20sp1lp_url_x64=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x64de.exe
es.dotnetfx35lp_url=http://download.microsoft.com/download/4/a/2/4a2b42fc-f271-4cc8-9c15-bc10cdde1eb9/dotnetfx35langpack_x86es.exe
es.dotnetfx20sp1lp_url_x64=http://download.microsoft.com/download/4/a/2/4a2b42fc-f271-4cc8-9c15-bc10cdde1eb9/dotnetfx35langpack_x64es.exe
                    

[Code]
procedure dotnetfx35lp();
begin
	if (ActiveLanguage() <> 'en') then begin
		if (not netfxinstalled(NetFx35, CustomMessage('dotnetfx35lp_lcid'))) then
			AddProduct('dotnetfx35_' + ActiveLanguage() + '.exe',
				'/lang:enu /passive /norestart',
				CustomMessage('dotnetfx35lp_title'),
				CustomMessage('dotnetfx35lp_size'),
				CustomMessage('dotnetfx35lp_url' + GetArchitectureString()),
				false, false);
	end;
end;
