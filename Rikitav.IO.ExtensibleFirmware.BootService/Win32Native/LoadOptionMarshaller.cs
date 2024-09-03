﻿using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    internal class LoadOptionMarshaller
    {
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
                EFI_DEVICE_PATH_PROTOCOL edpp = DeviceProtocolMarshaller.BinaryReaderToStructure(reader);
                FilePathListBuilder.Add(edpp);
                if (edpp.Type == DeviceProtocolType.END && edpp.SubType == 0xFF)
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
                DeviceProtocolMarshaller.StructureToBinaryWriter(edpp, writer);

            // Writing optional data
            writer.Write(structure.OptionalData);

            // Flushing
            writer.Flush();
        }
    }
}
