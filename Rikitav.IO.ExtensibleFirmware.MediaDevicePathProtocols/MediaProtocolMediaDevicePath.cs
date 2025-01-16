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

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// The Media Protocol Device Path is used to denote the protocol that is being used in a device path at the location of the path specified. Many protocols are inherent to the style of device path.
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#media-protocol-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.Media, 5)]
    public sealed class MediaProtocolMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.Media;

        /// <inheritdoc/>
        public override byte SubType => 5;

        /// <summary>
        /// The ID of the protocol
        /// </summary>
        public Guid ProtocolGUID { get; set; }

        /// <summary>
        /// Create new <see cref="MediaProtocolMediaDevicePath"/> protocol instance
        /// </summary>
        public MediaProtocolMediaDevicePath()
            : base() { }

        /// <summary>
        /// Create new <see cref="MediaProtocolMediaDevicePath"/> protocol instance from protocol GUID identificator
        /// </summary>
        public MediaProtocolMediaDevicePath(Guid protocolGUID)
            : base() => ProtocolGUID = protocolGUID;

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData) => ProtocolGUID = new Guid(protocolData);

        /// <inheritdoc/>
        protected override byte[] Serialize() => ProtocolGUID.ToByteArray();

        /// <inheritdoc/>
        public override string ToString() => ProtocolGUID.ToString();
    }
}
