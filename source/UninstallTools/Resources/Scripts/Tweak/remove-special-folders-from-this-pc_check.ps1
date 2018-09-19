param([Int32] $itemId = -1)

if($itemId -eq 1) {
    # Remove Desktop from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}") {
		exit 0
	}
}
if($itemId -eq 2) {
    # Remove Documents from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A8CDFF1C-4878-43be-B5FD-F8091C1C60D0}") {
		exit 0
	}
}
if($itemId -eq 3) {
    # Remove Downloads from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{374DE290-123F-4565-9164-39C4925E467B}") {
		exit 0
	}
}
if($itemId -eq 4) {
    # Remove Music from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{1CF1260C-4DD0-4ebb-811F-33C572699FDE}") {
		exit 0
	}
}
if($itemId -eq 5) {
    # Remove Pictures from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{3ADD1653-EB32-4cb0-BBD7-DFA0ABB5ACCA}") {
		exit 0
	}
}
if($itemId -eq 6) {
    # Remove Videos from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{A0953C92-50DC-43bf-BE83-3742FED03C9C}") {
		exit 0
	}
}
if($itemId -eq 7) {
    # Remove 3D Objects from This PC
	if(Test-Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace\{0DB7E03F-FC29-4DC6-9020-FF41B59E513A}") {
		exit 0
	}
}

exit 1
