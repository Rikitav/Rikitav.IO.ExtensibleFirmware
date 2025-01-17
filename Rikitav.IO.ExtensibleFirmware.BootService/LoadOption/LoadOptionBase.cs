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
                if (loadOption.FilePathList[i].Type == DeviceProtocolType.End && loadOption.FilePathList[i].SubType == 0xFF)
                    break; // End protocol

                Type? protocolWrapperType = DevicePathProtocolWrapperSelector.GetRegisteredType(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType);
                OptionProtocols[i] = protocolWrapperType == null
                    ? new RawMediaDevicePath(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType)
                    : DevicePathProtocolBase.CreateProtocol(protocolWrapperType, loadOption.FilePathList[i]);
            }
        }
        
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
