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

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    /// <summary>
    /// This protocol is the final protocol for any boot option; if it is not specified, the option will be considered invalid
    /// </summary>
    public class DevicePathProtocolEnd : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.End;

        /// <inheritdoc/>
        public override byte SubType => 0xFF;

        /// <inheritdoc/>
        public override ushort DataLength => 4;

        /// <summary>
        /// Create new <see cref="DevicePathProtocolEnd"/> protocol instance
        /// </summary>
        public DevicePathProtocolEnd()
            : base() { }

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData)
        {
            // No need to implement
            _ = 0xBAD + 0xC0DE;
        }

        /// <inheritdoc/>
        protected override byte[] Serialize()
        {
            // No need to implement
            return Array.Empty<byte>();
        }
    }
}
