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

using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    /// <summary>
    /// Provides properties and instance methods for the creation, copying, deletion, moving, and opening of Efi application files
    /// </summary>
    public class EfiExecutableInfo
    {
        /// <summary>
        /// The name of the folder in which the EFI executable is stored is the name of the provider of the executable file
        /// </summary>
        public string Application
        {
            get => Path.GetDirectoryName(FullName);
        }

        /// <summary>
        /// Full path to the executable file, including the partition path
        /// </summary>
        public string FullName
        {
            get => _FileInfo.FullName;
        }

        /// <summary>
        /// Executable architecture based on <see href="https://uefi.org/specs/UEFI/2.9_A/03_Boot_Manager.html#removable-media-boot-behavior">Boot manager docs</see>
        /// </summary>
        public FirmwareApplicationArchitecture Architecture
        {
            get
            {
                // Getting arch bit
                byte[] data = File.ReadAllBytes(FullName);
                ushort archVal = BitConverter.ToUInt16(data, BitConverter.ToInt32(data, 0x3c) + 4);

                // Gefined?
                if (!Enum.IsDefined(typeof(FirmwareApplicationArchitecture), archVal))
                    return FirmwareApplicationArchitecture.Unknown;

                // Result
                return (FirmwareApplicationArchitecture)archVal;
            }
        }
        
        /// <summary>
        /// Executable's file information
        /// </summary>
        public FileInfo FileInfo
        {
            get => _FileInfo;
        }
        private readonly FileInfo _FileInfo;

        /// <summary>
        /// Creates a new instance of <see cref="EfiExecutableInfo"/> from the relative path to the executable file path
        /// </summary>
        /// <param name="RelativePath"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public EfiExecutableInfo(string RelativePath)
        {
            if (string.IsNullOrEmpty(RelativePath))
                throw new ArgumentNullException(nameof(RelativePath));

            if (Path.GetExtension(RelativePath) != ".efi")
                throw new InvalidDataException("File extension does not match target type");

            _FileInfo = new FileInfo(Path.Combine(EfiPartition.GetFullPath(), RelativePath));
        }

        /// <summary>
        /// Creates a new instance of <see cref="EfiExecutableInfo"/> from the application provider name and executable file name
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <param name="ApplicationFile"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public EfiExecutableInfo(string ApplicationName, string ApplicationFile)
        {
            if (string.IsNullOrEmpty(ApplicationName))
                throw new ArgumentNullException(nameof(ApplicationName));

            if (string.IsNullOrEmpty(ApplicationFile))
                throw new ArgumentNullException(nameof(ApplicationFile));

            if (Path.GetExtension(ApplicationFile) != ".efi")
                throw new InvalidDataException("File extension does not match target type");

            _FileInfo = new FileInfo(Path.Combine(EfiPartition.GetFullPath(), "EFI", ApplicationName, ApplicationFile));
        }
    }
}
