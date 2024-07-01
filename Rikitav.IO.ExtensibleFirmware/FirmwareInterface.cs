using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware
{
    public static class FirmwareInterface
    {
        /// <summary>
        /// Checks whether the UEFI platform is available on this system
        /// </summary>
        /// <returns>If available, return <see langword="true"/>, else <see langword="false"/></returns>
        public static bool Available
        {
            get => FirmwareUtilities.FirmwareAvailable();
        }

        /// <summary>
        /// Searches among the partition for the one that is marked as EfiSystemPartition
        /// </summary>
        /// <returns><see cref="DirectoryInfo"/> of EfiSystemPartition</returns>
        public static DirectoryInfo GetSystemPartition()
        {
            return EfiPartition.GetDirectoryInfo();
        }

        internal static class NativeMethods
        {
            // Nope
        }
    }
}
