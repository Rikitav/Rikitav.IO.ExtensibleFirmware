using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
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
