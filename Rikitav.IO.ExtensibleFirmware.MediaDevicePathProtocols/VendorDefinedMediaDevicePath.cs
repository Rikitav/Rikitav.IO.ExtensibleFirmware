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
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// Vendor-Defined Media Device Path
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#vendor-defined-media-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.Media, 3)]
    public class VendorDefinedMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.Media;

        /// <inheritdoc/>
        public override byte SubType => 3;

        /// <summary>
        /// Vendor-assigned GUID that defines the data that follows
        /// </summary>
        public Guid VendorGuid { get; set; }

        /// <summary>
        /// Vendor-defined variable size data.
        /// </summary>
        public byte[] VendorDefinedData { get; set; }

        /// <summary>
        /// Create new <see cref="MediaProtocolMediaDevicePath"/> protocol instance
        /// </summary>
        public VendorDefinedMediaDevicePath()
            : base() => VendorDefinedData = Array.Empty<byte>();

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData)
        {
            using BinaryReader reader = new BinaryReader(new MemoryStream(protocolData));

            VendorGuid = new Guid(reader.ReadBytes(16));
            VendorDefinedData = reader.ReadBytes(protocolData.Length - 16);
        }

        /// <inheritdoc/>
        protected override byte[] Serialize()
        {
            using MemoryStream protocolData = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(protocolData);

            writer.Write(VendorGuid.ToByteArray());
            writer.Write(VendorDefinedData);

            return protocolData.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString() => VendorGuid.ToString();
    }
}
