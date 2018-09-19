$res1 = Get-ItemPropertyValue "HKCU:\Control Panel\Mouse" "MouseSpeed" -ErrorAction SilentlyContinue
if ($res1 -gt 0)
{ exit 0 }

$res2 = Get-ItemPropertyValue "HKCU:\Control Panel\Mouse" "MouseThreshold1" -ErrorAction SilentlyContinue
if ($res2 -gt 0)
{ exit 0 }

$res3 = Get-ItemPropertyValue "HKCU:\Control Panel\Mouse" "MouseThreshold2" -ErrorAction SilentlyContinue
if ($res3 -gt 0)
{ exit 0 }

exit 1
