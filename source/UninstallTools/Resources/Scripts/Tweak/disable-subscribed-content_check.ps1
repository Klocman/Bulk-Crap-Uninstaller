if(-Not (Test-Path "Registry::HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager"))
{
    # If ContentDeliveryManager doesn't exist it's likely not supported by the OS
    exit 2 
}

try
{
    $prop = Get-ItemPropertyValue "Registry::HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" "SubscribedContentEnabled" 
}
catch
{
    # If SubscribedContentEnabled value doesn't exist assume it's turned on
    exit 0
}

if($prop -eq 0)
{
    # Function is disabled if set to 0
    exit 1
}

exit 0
