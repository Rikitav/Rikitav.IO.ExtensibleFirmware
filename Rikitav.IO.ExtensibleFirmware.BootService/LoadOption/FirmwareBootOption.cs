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
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.BootService.LoadOption
{
    /// <summary>
    /// Basic implementation of <see cref="LoadOptionBase"/>
    /// </summary>
    public class FirmwareBootOption : LoadOptionBase
    {
        /// <summary>
        /// The data coming after the main ones is additional
        /// </summary>
        public byte[] OptionalData => _OptionalData;

        /// <summary>
        /// Create new <see cref="FirmwareBootOption"/> load option instance
        /// </summary>
        /// <param name="loadOption"></param>
        public FirmwareBootOption(EFI_LOAD_OPTION loadOption)
            : base(loadOption) { }

        /// <summary>
        /// Create new <see cref="FirmwareBootOption"/> load option instance from raw variable data
        /// </summary>
        /// <param name="loadOptionVarData"></param>
        public FirmwareBootOption(byte[] loadOptionVarData)
            : base(LoadOptionMarshaller.BinaryReaderToStructure(new BinaryReader(new MemoryStream(loadOptionVarData)))) { }

        /// <summary>
        /// Create new <see cref="FirmwareBootOption"/> load option instance from <see cref="LoadOptionAttributes"/> and Option description
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="description"></param>
        public FirmwareBootOption(LoadOptionAttributes attributes, string description)
            : base(attributes, description) { }

        /// <summary>
        /// Create new <see cref="FirmwareBootOption"/> load option instance from <see cref="LoadOptionAttributes"/>, Option description, <see cref="OptionalData"/> and <see cref="DevicePathProtocolBase"/>s
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="description"></param>
        /// <param name="optionalData"></param>
        /// <param name="protocols"></param>
        public FirmwareBootOption(LoadOptionAttributes attributes, string description, byte[] optionalData, DevicePathProtocolBase[] protocols) : base(attributes, description)
        {
            _OptionalData = optionalData;
            OptionProtocols = protocols;
        }
    }
}
