using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;

namespace Rikitav.IO.ExtensibleFirmware
{
    internal class FirmwareUtilities
    {
        public static bool FirmwareAvailable()
        {
            NativeMethods.GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);
            return Marshal.GetLastWin32Error() == 0;
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
