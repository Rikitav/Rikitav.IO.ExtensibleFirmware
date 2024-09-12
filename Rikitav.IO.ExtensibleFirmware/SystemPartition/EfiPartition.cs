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
        private static PARTITION_INFORMATION_EX _PartitionInfo = InternalEfiSystemPartitionFinder.FindEfiPartitionInfo();

        /// <summary>
        /// Guid identifier of the system EFI partition
        /// </summary>
        public static Guid Identificator => _PartitionInfo.Gpt.PartitionId;

        /// <summary>
        /// Guid type of system EFI partition
        /// </summary>
        public static Guid TypeID = new Guid("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");

        /// <summary>
        /// Get volume GUID path for system EFI partition
        /// </summary>
        /// <returns></returns>
        public static string GetFullPath()
            => @"\\?\Volume{" + Identificator + @"}\";

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
    }
}
