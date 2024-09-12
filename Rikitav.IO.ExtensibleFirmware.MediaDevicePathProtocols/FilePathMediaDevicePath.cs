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

using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// File Path Media Device Path
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#file-path-media-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.Media, 4, typeof(FilePathMediaDevicePath))]
    public sealed class FilePathMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.Media;

        /// <inheritdoc/>
        public override byte SubType => 4;

        /// <inheritdoc/>
        public override ushort DataLength => (ushort)((PathName.Length + 1) * 2 + 4);

        /// <summary>
        /// A NULL-terminated Path string including directory and file name.
        /// </summary>
        public string PathName { get; set; } = string.Empty;

        /// <summary>
        /// Create new <see cref="FilePathMediaDevicePath"/> protocol instance
        /// </summary>
        public FilePathMediaDevicePath()
            : base() { }

        /// <summary>
        /// Create new <see cref="FilePathMediaDevicePath"/> protocol instance from file path
        /// </summary>
        public FilePathMediaDevicePath(string pathName)
            : base() => PathName = pathName;

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData) => PathName = Encoding.Unicode.GetString(protocolData).TrimEnd('\0');

        /// <inheritdoc/>
        protected override byte[] Serialize() => Encoding.Unicode.GetBytes(PathName + '\0');

        /// <inheritdoc/>
        public override string ToString() => PathName;
    }
}
