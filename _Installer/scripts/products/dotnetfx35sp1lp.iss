

[Code]
procedure dotnetfx35sp1lp();
begin
	if (ActiveLanguage() <> 'en') then begin
		if (netfxspversion(NetFx35, CustomMessage('dotnetfx35sp1lp_lcid')) < 1) then
			AddProduct('dotnetfx35sp1_' + ActiveLanguage() + '.exe',
				'/lang:enu /passive /norestart',
				CustomMessage('dotnetfx35sp1lp_title'),
				CustomMessage('dotnetfx35sp1lp_size'),
				CustomMessage('dotnetfx35sp1lp_url'),
				false, false);
	end;
end;