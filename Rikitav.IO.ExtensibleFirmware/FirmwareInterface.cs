using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware
{
    public static class FirmwareInterface
    {
        /// <summary>
        /// "Partition type GUID" for partition that contains all of systems boot loaders
        /// </summary>
        public static readonly Guid PartitionTypeIdentificator = new Guid("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");

        /// <summary>
        /// Checks whether the UEFI platform is available on this system
        /// </summary>
        /// <returns>If available, return <see langword="true"/>, else <see langword="false"/></returns>
        public static bool CheckAvailability()
        {
            return FirmwareUtilities.GetFirmwareType() == FirmwareUtilities.FirmwareType.Uefi;
        }

        /// <summary>
        /// Searches among the partition for the one that is marked as EfiSystemPartition
        /// </summary>
        /// <returns><see cref="DirectoryInfo"/> of EfiSystemPartition</returns>
        public static DirectoryInfo GetSystemPartition()
        {
            return FirmwareUtilities.FindEfiSystemPartitionDirectoryInfo();
        }

        internal static class NativeMethods
        {
            // Nope
        }
    }
}
