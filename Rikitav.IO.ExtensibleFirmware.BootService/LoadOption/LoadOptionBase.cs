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
using System;

namespace Rikitav.IO.ExtensibleFirmware.BootService.LoadOption
{
    /// <summary>
    /// <see href="https://uefi.org/specs/UEFI/2.9_A/03_Boot_Manager.html#load-options">Load option</see> used to determine the boot parameters of a specific object, be it a driver or an OS
    /// </summary>
    public abstract class LoadOptionBase
    {
        /// <summary>
        /// The attributes for this load option entry
        /// </summary>
        public LoadOptionAttributes Attributes { get; set; }

        /// <summary>
        /// The user readable description for the load option
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// <para>FilePathList</para>
        /// A packed array of UEFI device paths.The first element of the array is a device path that describes the device and location of the Image for this load option.The FilePathList[0] is specific to the device type.Other device paths may optionally exist in the FilePathList, but their usage is OSV specific. Each element in the array is variable length, and ends at the device path end structure. Because the size of Description is arbitrary, this data structure is not guaranteed to be aligned on a natural boundary.This data structure may have to be copied to an aligned natural boundary before it is used
        /// </summary>
        public DevicePathProtocolBase[] OptionProtocols { get; protected set; }

        /// <summary>
        /// The remaining bytes in the load option descriptor are a binary data buffer that is passed to the loaded image. If the field is zero bytes long, a NULL pointer is passed to the loaded image. The number of bytes in OptionalData can be computed by subtracting the starting offset of OptionalData from total size in bytes of the EFI_LOAD_OPTION
        /// </summary>
        protected byte[] OptionalData;

        /// <summary>
        /// Create new <see cref="LoadOptionBase"/> load option instance from attributes and description
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="description"></param>
        protected LoadOptionBase(LoadOptionAttributes attributes, string description)
        {
            Attributes = attributes;
            Description = description;
            OptionProtocols = Array.Empty<DevicePathProtocolBase>();
            OptionalData = Array.Empty<byte>();
        }

        /// <summary>
        /// Create new <see cref="LoadOptionBase"/> load option instance from raw structure
        /// </summary>
        /// <param name="loadOption"></param>
        protected LoadOptionBase(EFI_LOAD_OPTION loadOption)
        {
            Attributes = loadOption.Attributes;
            Description = loadOption.Description;
            OptionalData = loadOption.OptionalData;

            OptionProtocols = new DevicePathProtocolBase[loadOption.FilePathList.Length];
            for (int i = 0; i < loadOption.FilePathList.Length; i++)
            {
                Type? protocolWrapperType = DevicePathProtocolWrapperSelector.GetRegisteredType(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType);
                OptionProtocols[i] = protocolWrapperType == null
                    ? new RawMediaDevicePath(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType)
                    : DevicePathProtocolBase.CreateProtocol(protocolWrapperType, loadOption.FilePathList[i]);
            }
        }

        /*
        /// <summary>
        /// Create new <see cref="LoadOptionBase"/> load option instance from raw structure
        /// </summary>
        /// <param name="binaryReader"></param>
        protected LoadOptionBase(BinaryReader binaryReader)
        {
            Attributes = 0;
            Description = string.Empty;
            OptionProtocols = Array.Empty<DevicePathProtocolBase>();
            _OptionalData = Array.Empty<byte>();

            MarshalFromBinaryReader(binaryReader);
        }
        */

        /*
        /// <summary>
        /// Returns the length of the native EFI_LOAD_OPTION structure
        /// </summary>
        /// <returns></returns>
        internal int GetStructureLength()
        {
            return OptionProtocols.Sum(x => x.DataLength) // Protocols data
                + ((Description.Length + 1) * 2)          // Description string
                + _OptionalData.Length                    // Optional data
                + 2                                       // FilePathListLength
                + 4;                                      // Attributes
        }
        */

        /*
        /// <summary>
        /// ¬озвращает нативное представление структуры <see cref="EFI_LOAD_OPTION"/>
        /// </summary>
        /// <returns></returns>
        internal EFI_LOAD_OPTION ToLoadOption()
        {
            EFI_DEVICE_PATH_PROTOCOL[] FilePathList = OptionProtocols.Select(x => x.ToEfiProtocol()).ToArray();
            return new EFI_LOAD_OPTION()
            {
                Attributes = Attributes,
                FilePathListLength = (ushort)FilePathList.Sum(x => x.Length),
                Description = Description,
                FilePathList = FilePathList,
                OptionalData = _OptionalData
            };
        }
        */

        /*
        /// <summary>
        /// Writes structure data as a native <see cref="EFI_LOAD_OPTION"/> structure to a <see cref="BinaryWriter"/> stream
        /// </summary>
        /// <param name="writer"></param>
        internal void MarshalToBinaryWriter(BinaryWriter writer)
        {
            // Writing attributes field
            writer.Write((int)Attributes);

            // Writing length of filepath list
            writer.Write((ushort)OptionProtocols.Sum(p => p.DataLength));

            // Writing option description
            writer.Write(Encoding.Unicode.GetBytes(Description + "\0"));

            // Writing filepath list
            foreach (DevicePathProtocolBase edpp in OptionProtocols)
                edpp.MarshalToBinaryWriter(writer);

            // Writing optional data
            writer.Write(_OptionalData);
        }
        */

        /*
        internal void MarshalFromBinaryReader(BinaryReader reader)
        {
            // Reading Attributes field
            Attributes = (LoadOptionAttributes)reader.ReadUInt32();

            // Reading length of filepath list
            ushort FilePathListLength = reader.ReadUInt16();

            // Reading Description (Load option name)
            StringBuilder builder = new StringBuilder();
            for (ushort chr = reader.ReadUInt16(); chr != 0; chr = reader.ReadUInt16())
                builder.Append((char)chr);

            // Reading Description (Load option name)
            Description = builder.ToString();

            // Reading Device path list
            List<EFI_DEVICE_PATH_PROTOCOL> FilePathListBuilder = new List<EFI_DEVICE_PATH_PROTOCOL>();
            while (true)
            {
                EFI_DEVICE_PATH_PROTOCOL edpp = DeviceProtocolMarshaller.BinaryReaderToStructure(reader);
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
        }
        */

        /*
        public object CopyTo(LoadOptionBase optionCopy)
        {
            optionCopy.Attributes = Attributes;
            optionCopy.Description = Description;
            
            optionCopy.OptionProtocols = new DevicePathProtocolBase[OptionProtocols.Length];
            OptionProtocols.CopyTo(optionCopy.OptionProtocols, 0);

            optionCopy._OptionalData = _OptionalData;
            return optionCopy;
        }
        */

        internal byte[] GetOptionalData()
        {
            return OptionalData;
        }

        /// <summary>
        /// Modifies optional data within a structure
        /// </summary>
        /// <param name="data"></param>
        public void OverrideOptionalData(byte[] data)
        {
            OptionalData = data;
        }
    }
}
