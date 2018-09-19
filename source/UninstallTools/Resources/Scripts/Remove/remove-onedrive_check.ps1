$odpath = Get-ItemPropertyValue "Registry::HKEY_CURRENT_USER\SOFTWARE\Microsoft\OneDrive" "CurrentVersionPath" -ErrorAction SilentlyContinue
if (Test-Path -Path $odpath -PathType Container)
{ exit 0 }

$res = Get-ItemPropertyValue "Registry::HKEY_CLASSES_ROOT\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}" "System.IsPinnedToNameSpaceTree" -ErrorAction SilentlyContinue
if ($res -gt 0)
{ exit 0 }

$res2 = Get-ItemPropertyValue "Registry::HKEY_CLASSES_ROOT\Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}" "System.IsPinnedToNameSpaceTree" -ErrorAction SilentlyContinue
if ($res2 -gt 0)
{ exit 0 }

exit 1

