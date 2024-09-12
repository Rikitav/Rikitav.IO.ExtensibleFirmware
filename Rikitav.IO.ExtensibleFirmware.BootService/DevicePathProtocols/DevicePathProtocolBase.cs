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

using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    /// <summary>
    /// A <see href="https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#generic-device-path-structures">Device Path</see> is used to define the programmatic path to a device. The primary purpose of a Device Path is to allow an application, such as an OS loader, to determine the physical device that the interfaces are abstracting.
    /// </summary>
    public abstract class DevicePathProtocolBase
    {
        /// <summary>
        /// Protocol type
        /// </summary>
        public abstract DeviceProtocolType Type { get; }

        /// <summary>
        /// Protocol Sub-Type - Varies by Type
        /// </summary>
        public abstract byte SubType { get; }

        /// <summary>
        /// Length of this structure in bytes. Length is 4 + n bytes.
        /// </summary>
        public abstract ushort DataLength { get; }

        /// <summary>
        /// Abstract constructor
        /// </summary>
        protected DevicePathProtocolBase() { }

        /// <summary>
        /// Create protocol wrapper of Type and deserailize structure
        /// </summary>
        /// <param name="protocolType"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        internal static DevicePathProtocolBase CreateProtocol(Type protocolType, EFI_DEVICE_PATH_PROTOCOL protocol)
        {
            DevicePathProtocolBase? protocolWrapper = (DevicePathProtocolBase?)Activator.CreateInstance(protocolType);
            if (protocolWrapper == null)
                throw new DeviceProtocolCastingException("Failed to cast DevicePathProtocol of type {0} and subType {1} to managed object");

            protocolWrapper.Deserialize(protocol.Data);
            return protocolWrapper;
        }

        /// <summary>
        /// Serialize protocol to structre
        /// </summary>
        /// <returns></returns>
        internal EFI_DEVICE_PATH_PROTOCOL ToEfiProtocol()
        {
            return new EFI_DEVICE_PATH_PROTOCOL()
            {
                Type = Type,
                SubType = SubType,
                Length = DataLength,
                Data = Serialize()
            };
        }

        /// <summary>
        /// Serialize and write protocol into BinaryWriter
        /// </summary>
        /// <param name="writer"></param>
        internal void MarshalToBinaryWriter(BinaryWriter writer)
        {
            byte[] data = Serialize();

            writer.Write((byte)Type);
            writer.Write(SubType);
            writer.Write(DataLength);
            writer.Write(data);
        }

        /// <summary>
        /// Deserialize raw data into managed data
        /// </summary>
        /// <param name="protocolData"></param>
        protected abstract void Deserialize(byte[] protocolData);

        /// <summary>
        /// Serialize managed data into raw data
        /// </summary>
        /// <returns></returns>
        protected abstract byte[] Serialize();
    }
}
