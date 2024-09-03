using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware
{
    public static partial class FirmwareInterface
    {
        /// <summary>
        /// Checks whether the UEFI platform is available on this system
        /// </summary>
        /// <returns>If available, return <see langword="true"/>, else <see langword="false"/></returns>
        public static bool Available
        {
            get => FirmwareUtilities.CheckInterfaceAvailablity();
        }

        /// <summary>
        /// Searches among the partition for the one that is marked as EfiSystemPartition
        /// </summary>
        /// <returns><see cref="DirectoryInfo"/> of EfiSystemPartition</returns>
        public static DirectoryInfo SystemPartition
        {
            get => EfiPartition.GetDirectoryInfo();
        }

        /// <summary>
        /// Boot into the UEFI user interface after rebooting the computer. Does NOT reboot the computer, but sets the boot condition
        /// </summary>
        public static void BootToUserInterface()
        {
            if (!FirmwareGlobalEnvironment.OsIndicationsSupported.HasFlag(EfiOsIindications.BOOT_TO_FW_UI))
                throw new PlatformNotSupportedException();

            FirmwareGlobalEnvironment.OsIndications |= EfiOsIindications.BOOT_TO_FW_UI;
        }
    }
}
