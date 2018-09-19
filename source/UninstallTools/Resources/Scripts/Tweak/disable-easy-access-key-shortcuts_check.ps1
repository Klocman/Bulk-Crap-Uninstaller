$res1 = Get-ItemPropertyValue "HKCU:\Control Panel\Accessibility\StickyKeys" "Flags" -ErrorAction SilentlyContinue
if ($res1 -ne 506)
{ exit 0 }

$res2 = Get-ItemPropertyValue "HKCU:\Control Panel\Accessibility\Keyboard Response" "Flags" -ErrorAction SilentlyContinue
if ($res2 -ne 122)
{ exit 0 }

$res3 = Get-ItemPropertyValue "HKCU:\Control Panel\Accessibility\ToggleKeys" "Flags" -ErrorAction SilentlyContinue
if ($res3 -ne 58)
{ exit 0 }

exit 1
