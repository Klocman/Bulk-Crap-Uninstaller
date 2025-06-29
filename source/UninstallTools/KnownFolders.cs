using System;
using System.Runtime.InteropServices;

namespace UninstallTools;

internal static class KnownFolders
{
    public static string GetKnownFolderPath(Guid rfid)
    {
        IntPtr pPath;
        SHGetKnownFolderPath(rfid, 0, IntPtr.Zero, out pPath);

        var path = Marshal.PtrToStringUni(pPath);
        Marshal.FreeCoTaskMem(pPath);
        return path;
    }

    [DllImport("shell32.dll")]
    private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
                                                   uint dwFlags, IntPtr hToken, out IntPtr pszPath);
}
