using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    internal static class DevicePathProtocolMarshaller
    {
        private const int BytesPerChar = 2;
        private static readonly Dictionary<Type, int> ProtocolWrapperLengthHash = new Dictionary<Type, int>();

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

        public static void MarshalToBinaryWriter(DevicePathProtocolBase devicePathProtocol, BinaryWriter writer)
        {
            byte[] serializingData = devicePathProtocol.GetSerializingData();

            writer.Write((byte)devicePathProtocol.Type);
            writer.Write(devicePathProtocol.SubType);
            writer.Write(serializingData.Length);
            writer.Write(serializingData);
        }

        public static int GetStrctureLength(DevicePathProtocolBase devicePathProtocol)
        {
            Type protocolType = devicePathProtocol.GetType();
            if (ProtocolWrapperLengthHash.TryGetValue(protocolType, out int structureLength))
                return structureLength;

            structureLength
                = 2  // Type
                + 2  // SubType
                + 4; // DataLength

            foreach (PropertyInfo propertyInfo in protocolType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                structureLength += GetUnitLength(propertyInfo, devicePathProtocol);

            ProtocolWrapperLengthHash.Add(protocolType, structureLength);
            return structureLength;
        }

        private static int GetUnitLength(PropertyInfo propertyInfo, DevicePathProtocolBase devicePathProtocol)
        {
            switch (propertyInfo.PropertyType.Name)
            {
                case nameof(String):
                    {
                        string propValue = (string)propertyInfo.GetValue(devicePathProtocol, null);
                        return (propValue.Length + 1) * BytesPerChar;
                    }

                case var _ when propertyInfo.PropertyType.IsEnum:
                    {
                        return Marshal.SizeOf(propertyInfo.PropertyType.GetEnumUnderlyingType());
                    }

                case var _ when propertyInfo.PropertyType.IsByRef:
                    {
                        throw new InvalidDevicePathProtocolStructureException("The protocol wrapper has the wrong structure and contains reference types");
                    }

                default:
                    {
                        return Marshal.SizeOf(propertyInfo.PropertyType);
                    }
            }
        }
    }
}
