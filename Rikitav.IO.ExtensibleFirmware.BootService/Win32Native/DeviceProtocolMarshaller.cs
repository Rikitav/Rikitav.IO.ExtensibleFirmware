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
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    internal static class DeviceProtocolMarshaller
    {
        public static EFI_DEVICE_PATH_PROTOCOL BinaryReaderToStructure(BinaryReader reader)
        {
            // Starting manual marshalling strcture to managed
            EFI_DEVICE_PATH_PROTOCOL protocol = new EFI_DEVICE_PATH_PROTOCOL();

            // Reading device type
            protocol.Type = (DeviceProtocolType)reader.ReadByte();

            // Reading device subType
            protocol.SubType = reader.ReadByte();

            // Reading structure length
            protocol.Length = reader.ReadUInt16();

            // Reading protocol data
            protocol.Data = reader.ReadBytes(protocol.Length - 4);

            // Done
            return protocol;
        }

        public static void StructureToBinaryWriter(EFI_DEVICE_PATH_PROTOCOL structure, BinaryWriter writer)
        {
            // Writing device type
            writer.Write((byte)structure.Type);

            // Writing device subType
            writer.Write(structure.SubType);

            // Writing structure length
            writer.Write(structure.Length);

            // Writing protocol data
            writer.Write(structure.Data);
        }
    }
}
