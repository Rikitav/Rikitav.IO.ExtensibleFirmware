using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    internal static class LoadOptionMarshaller
    {
        private const int BytesPerChar = 2;

        public static EFI_LOAD_OPTION BinaryReaderToStructure(BinaryReader reader)
        {
            // Starting manual marshalling strcture to managed
            EFI_LOAD_OPTION LoadOption = new EFI_LOAD_OPTION();

            // Reading Attributes field
            LoadOption.Attributes = (LoadOptionAttributes)reader.ReadUInt32();

            // Reading length of filepath list
            LoadOption.FilePathListLength = reader.ReadUInt16();

            // Reading Description (Load option name)
            StringBuilder builder = new StringBuilder();
            for (ushort chr = reader.ReadUInt16(); chr != 0; chr = reader.ReadUInt16())
                builder.Append((char)chr);

            // Reading Description (Load option name)
            LoadOption.Description = builder.ToString();

            // Reading Device path list
            List<EFI_DEVICE_PATH_PROTOCOL> FilePathListBuilder = new List<EFI_DEVICE_PATH_PROTOCOL>();
            while (true)
            {
                EFI_DEVICE_PATH_PROTOCOL edpp = DevicePathProtocolMarshaller.BinaryReaderToStructure(reader);
                FilePathListBuilder.Add(edpp);
                if (edpp.Type == DeviceProtocolType.End && edpp.SubType == 0xFF)
                    break;
            }

            // Reading Device path list
            LoadOption.FilePathList = FilePathListBuilder.ToArray();

            // Manually seek to optional data position because EFI_DEVICE_PATH_PROTOCOL sequence not always property aligned
            int SeekLength = sizeof(uint) + sizeof(ushort) + (LoadOption.Description.Length + 1) * sizeof(ushort) + LoadOption.FilePathListLength;
            reader.BaseStream.Seek(SeekLength, SeekOrigin.Begin);

            // Reading OptionalData field
            LoadOption.OptionalData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            return LoadOption;
        }

        public static void StructureToBinaryWriter(EFI_LOAD_OPTION structure, BinaryWriter writer)
        {
            // Writing attributes field
            writer.Write((int)structure.Attributes);

            // Writing length of filepath list
            writer.Write(structure.FilePathListLength);

            // Writing
            byte[] DescriptionString = Encoding.Unicode.GetBytes(structure.Description + "\0");
            writer.Write(DescriptionString);

            // Writing filepath list
            foreach (EFI_DEVICE_PATH_PROTOCOL edpp in structure.FilePathList)
                DevicePathProtocolMarshaller.StructureToBinaryWriter(edpp, writer);

            // Writing optional data
            writer.Write(structure.OptionalData);

            // Flushing
            writer.Flush();
        }

        public static void MarshalToBinaryWriter(LoadOptionBase loadOption, BinaryWriter writer)
        {
            // Writing attributes field
            writer.Write((int)loadOption.Attributes);

            // Writing length of filepath list
            writer.Write((ushort)loadOption.OptionProtocols.Sum(p => DevicePathProtocolMarshaller.GetStrctureLength(p)));

            // Writing option description
            writer.Write(Encoding.Unicode.GetBytes(loadOption.Description + "\0"));

            // Writing filepath list
            foreach (DevicePathProtocolBase edpp in loadOption.OptionProtocols)
                DevicePathProtocolMarshaller.MarshalToBinaryWriter(edpp, writer);

            if (!(loadOption.OptionProtocols.Last() is DevicePathProtocolEnd))
                DevicePathProtocolMarshaller.MarshalToBinaryWriter(new DevicePathProtocolEnd(), writer);

            // Writing optional data
            writer.Write(loadOption.GetOptionalData());
        }

        public static int GetStrcutureLength(LoadOptionBase loadOption)
        {
            int structLength = ((loadOption.Description.Length + 1) * BytesPerChar) + 2 + 4;

            foreach (DevicePathProtocolBase devicePathProtocol in loadOption.OptionProtocols)
                structLength += DevicePathProtocolMarshaller.GetStrctureLength(devicePathProtocol);

            return structLength;
        }
    }
}
