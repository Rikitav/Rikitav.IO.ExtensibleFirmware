using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware
{
    internal class FirmwareUtilities
    {
        public static bool FirmwareAvailable()
        {
            _ = NativeMethods.GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);
            int lastError = Marshal.GetLastWin32Error();
            
            Debug.WriteLine("FirmwareAvailable function lastError : " + lastError, nameof(FirmwareUtilities));
            return lastError != NativeMethods.ERROR_INVALID_FUNCTION;
        }

        internal static class NativeMethods
        {
            public const int ERROR_INVALID_FUNCTION = 1;
            [DllImport("kernel32.dll",
               EntryPoint = "GetFirmwareEnvironmentVariableW",
               SetLastError = true,
               CharSet = CharSet.Unicode,
               ExactSpelling = true,
               CallingConvention = CallingConvention.StdCall)]
            public static extern int GetFirmwareType(string lpName, string lpGUID, IntPtr pBuffer, uint size);
        }
    }
}
