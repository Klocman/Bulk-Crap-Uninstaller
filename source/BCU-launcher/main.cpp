#include <iostream>
#include <Windows.h>
#include <tchar.h>
#include <cstring> 
#include <string>

std::wstring ExePath()
{
	TCHAR buffer[MAX_PATH] = { 0 };
	GetModuleFileName(NULL, buffer, MAX_PATH);
	std::wstring::size_type pos = std::wstring(buffer).find_last_of(L"\\/");
	return std::wstring(buffer).substr(0, pos);
}

BOOL Is64BitOS()
{
	BOOL bIs64BitOS = FALSE;

	// We check if the OS is 64 Bit
	typedef BOOL(WINAPI* LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);

	LPFN_ISWOW64PROCESS fnIsWow64Process = (LPFN_ISWOW64PROCESS)GetProcAddress(GetModuleHandle(L"kernel32"), "IsWow64Process");

	if (NULL != fnIsWow64Process)
	{
		if (!fnIsWow64Process(GetCurrentProcess(), &bIs64BitOS))
		{
			//error, try to start the app anyways
			return true;
		}
	}
	return bIs64BitOS;
}

std::wstring GetLastErrorAsString()
{
	//Get the error message ID, if any.
	DWORD errorMessageID = ::GetLastError();
	if (errorMessageID == 0) {
		return std::wstring(); //No error message has been recorded
	}

	LPWSTR messageBuffer = nullptr;

	//Ask Win32 to give us the string version of that message ID.
	//The parameters we pass in, tell Win32 to create the buffer that holds the message for us (because we don't yet know how long the message string will be).
	size_t size = FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL, errorMessageID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPWSTR)&messageBuffer, 0, NULL);

	//Copy the error message into a std::string.
	std::wstring message(messageBuffer, size);

	//Free the Win32's string's buffer.
	LocalFree(messageBuffer);

	return message;
}
#include <fstream>
bool fexists(const std::wstring& filename) {
	std::ifstream ifile(filename.c_str());
	return (bool)ifile;
}

bool IsWindowsVersionOrGreater(WORD wMajorVersion, WORD wMinorVersion, WORD wServicePackMajor)
{
    OSVERSIONINFOEXW osvi = { sizeof(osvi), 0, 0, 0, 0, {0}, 0, 0 };
    DWORDLONG        const dwlConditionMask = VerSetConditionMask(
        VerSetConditionMask(
        VerSetConditionMask(
            0, VER_MAJORVERSION, VER_GREATER_EQUAL),
               VER_MINORVERSION, VER_GREATER_EQUAL),
               VER_SERVICEPACKMAJOR, VER_GREATER_EQUAL);

    osvi.dwMajorVersion = wMajorVersion;
    osvi.dwMinorVersion = wMinorVersion;
    osvi.wServicePackMajor = wServicePackMajor;

    return VerifyVersionInfoW(&osvi, VER_MAJORVERSION | VER_MINORVERSION | VER_SERVICEPACKMAJOR, dwlConditionMask) != FALSE;
}

bool isWin7orLater()
{
    return IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WIN7), LOBYTE(_WIN32_WINNT_WIN7), 1);
}

// Needed to get MessageBox working with MFC
#pragma comment(lib, "user32.lib")

int main()
{
	std::wstring p;

	if (Is64BitOS())
	{
		p = ExePath() + L"\\win-x64\\BCUninstaller.exe";
		if (!fexists(p))
		{
			MessageBox(nullptr, L"This installation of BCUninstaller does not have files needed to run on 64 bit versions of Windows.\n\nDownload a 64 bit version of BCUninstaller and try again. The installer includes both 32 and 64 bit versions.", L"Could not start BCUninstaller.", MB_ICONERROR | MB_OK);
			return 1;
		}
	}
	else
	{
		p = ExePath() + L"\\win-x86\\BCUninstaller.exe";
		if (!fexists(p))
		{
			MessageBox(nullptr, L"This installation of BCUninstaller does not have files needed to run on 32 bit versions of Windows.\n\nDownload a 32 bit version of BCUninstaller and try again. The installer includes both 32 and 64 bit versions.", L"Could not start BCUninstaller.", MB_ICONERROR | MB_OK);
			return 1;
		}
	}

	if(!isWin7orLater())
	{
		MessageBox(nullptr, L"This version of BCUninstaller needs Windows 7 SP1 / Windows Server 2008 R2 SP1 or later. Either update your system, or use an old version of BCUninstaller.\n\nTo bypass this check you can run BCUninstaller.exe directly from one of the subfolders.", L"Could not start BCUninstaller.", MB_ICONERROR | MB_OK);
		return 2;
	}

	// Required on some systems or CreateProcess fails
	p = L"\"" + p + L"\"";

	auto cl = p.c_str();
	size_t pSizeTerminated = p.size() + 1;
	wchar_t* cla = new wchar_t[pSizeTerminated];
	wcscpy_s(cla, pSizeTerminated, cl);

	STARTUPINFO si;
	PROCESS_INFORMATION pi;
	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));
	// Start the child process. 
	if (!CreateProcess(NULL,   // No module name (use command line)
		cla,        // Command line
		NULL,           // Process handle not inheritable
		NULL,           // Thread handle not inheritable
		FALSE,          // Set handle inheritance to FALSE
		0,              // No creation flags
		NULL,           // Use parent's environment block
		NULL,           // Use parent's starting directory 
		&si,            // Pointer to STARTUPINFO structure
		&pi)           // Pointer to PROCESS_INFORMATION structure
		)
	{
		MessageBox(nullptr, (L"Failed to start BCUninstaller - " + GetLastErrorAsString()).c_str(), L"Could not start BCUninstaller.", MB_ICONERROR | MB_OK);
		printf("CreateProcess failed (%d).\n", GetLastError());
		return 3;
	}

	// Wait until child process exits.
	//WaitForSingleObject( pi.hProcess, INFINITE );

	// Close process and thread handles. 
	CloseHandle(pi.hProcess);
	CloseHandle(pi.hThread);
}
