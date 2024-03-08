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
        /// <summary>
        /// 
        /// </summary>
        public enum FirmwareType
        {
            Unknown = 0,
            Bios = 1,
            Uefi = 2,
            Max = 3
        }

        public static FirmwareType GetFirmwareType()
        {
            if (NativeMethods.GetFirmwareType(out FirmwareType FwType))
                return FwType;

            return FirmwareType.Unknown;
        }

        internal static class NativeMethods
        {
            #region Firmware type
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool GetFirmwareType(out FirmwareType FwType);
            #endregion
        }
    }
}
