[CustomMessages]
de.dotnetfx35lp_title=.NET Framework 3.5 Sprachpaket: Deutsch


dotnetfx35lp_size=13 MB - 51 MB    
fr.dotnetfx35lp_size=13 Mo - 51 Mo

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.dotnetfx35lp_lcid=1033                       
pl.dotnetfx35lp_lcid=1045
fr.dotnetfx35lp_lcid=1036
de.dotnetfx35lp_lcid=1031

de.dotnetfx35lp_url=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x86de.exe 
de.dotnetfx35lp_url=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x86de.exe
de.dotnetfx35lp_url=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x86de.exe
de.dotnetfx35lp_url=http://download.microsoft.com/download/d/7/2/d728b7b9-454b-4b57-8270-45dac441b0ec/dotnetfx35langpack_x86de.exe
                    

[Code]
procedure dotnetfx35lp();
begin
	if (ActiveLanguage() <> 'en') then begin
		if (not netfxinstalled(NetFx35, CustomMessage('dotnetfx35lp_lcid'))) then
			AddProduct('dotnetfx35_' + ActiveLanguage() + '.exe',
				'/lang:enu /passive /norestart',
				CustomMessage('dotnetfx35lp_title'),
				CustomMessage('dotnetfx35lp_size'),
				CustomMessage('dotnetfx35lp_url'),
				false, false);
	end;
end;
