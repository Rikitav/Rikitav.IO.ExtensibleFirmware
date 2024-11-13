// Rikitav.IO.ExtensibleFirmware
// Copyright (C) 2024 Rikitav
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using Rikitav.IO.ExtensibleFirmware.Win32Native;
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    /// <summary>
    /// System partition containing boot files of operating systems
    /// </summary>
    public static class EfiPartition
    {
        /// <summary>
        /// System partition information
        /// </summary>
        private static PARTITION_INFORMATION_EX _PartitionInfo = FindEfiPartitionInfo();

        /// <summary>
        /// Guid identifier of the system EFI partition
        /// </summary>
        public static Guid Identificator => _PartitionInfo.Gpt.PartitionId;

        /// <summary>
        /// Guid type of system EFI partition
        /// </summary>
        public static readonly Guid TypeID = Guid.Parse("c12a7328-f81f-11d2-ba4b-00a0c93ec93b");

        /// <summary>
        /// Get volume GUID path for system EFI partition
        /// </summary>
        /// <returns></returns>
        public static string GetFullPath()
            => string.Concat("\\\\?\\Volume{", Identificator.ToString(), "}\\");

        /// <summary>
        /// Directory information for system EFI partition
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo GetDirectoryInfo()
            => new DirectoryInfo(GetFullPath());

        /// <summary>
        /// Get volume information for system EFI partition
        /// </summary>
        /// <returns></returns>
        public static PARTITION_INFORMATION_EX GetPartitionInfo()
            => _PartitionInfo;

        private static PARTITION_INFORMATION_EX FindEfiPartitionInfo()
        {
            if (!FirmwareInterface.Available)
                throw new PlatformNotSupportedException("Executing on non UEFI System");

            Guid EfiPartType = Guid.Parse("c12a7328-f81f-11d2-ba4b-00a0c93ec93b");
            foreach (PARTITION_INFORMATION_EX partition in new IoctlVolumeEnumerable(0))
            {
                if (partition.PartitionStyle != PartitionStyle.GuidPartitionTable)
                    throw new DriveNotFoundException("Drive signature is not GPT (GuidPartitionTable)");

                if (partition.Gpt.PartitionType == EfiPartType)
                    return partition;
            }

            throw new DriveNotFoundException("Efi partition was not found");
        }
    }
}
