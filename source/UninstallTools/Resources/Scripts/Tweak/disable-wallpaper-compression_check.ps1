$res1 = Get-ItemPropertyValue "HKCU:\Control Panel\Desktop" "JPEGImportQuality" -ErrorAction SilentlyContinue
if ($res1 -ne 100)
 { exit 0 }
exit 1
