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
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.BootService
{
    /// <summary>
    /// Provides methods to read, write, update, and delete boot entries from your computer's UEFI NVRAM
    /// </summary>
    public static class FirmwareBootService
    {
        /// <summary>
        /// Gets the index of the boot record this computer was booted from. Use this index in methods of this <see cref="FirmwareBootService"/>
        /// </summary>
        public static ushort CurrentLoadOptionIndex
        {
            get => FirmwareGlobalEnvironment.BootCurrent;
        }

        /// <summary>
        /// Sets the index of the boot entry that will be loaded next time ONCE. Use this index in methods of this class
        /// </summary>
        public static ushort NextLoadOptionIndex
        {
            set => FirmwareGlobalEnvironment.BootNext = value;
        }

        /// <summary>
        /// Gets or sets the boot order. The array contains the indexes of all boot entries in the order in which they will be loaded. Use this index in methods of this class
        /// </summary>
        public static ushort[] LoadOrder
        {
            get => FirmwareGlobalEnvironment.BootOrder;
            set => FirmwareGlobalEnvironment.BootOrder = value;
        }

        /// <summary>
        /// Resets the Boot#### variable at the specified index and removes it from the boot order
        /// </summary>
        /// <param name="BootOptionIndex"></param>
        public static void DeleteLoadOption(ushort BootOptionIndex)
        {
            // Writing null variable to firmware
            FirmwareUtilities.SetGlobalEnvironmentVariable(
                BS_ValueFormatHelper.GetLoadOptionNameFromIndex(BootOptionIndex),
                IntPtr.Zero,
                0);

            // Removing index from boot order
            LoadOrder = LoadOrder.Where(x => x != BootOptionIndex).ToArray();
        }

        /// <summary>
        /// Reads the boot option from NVRAM, issues it to the delegate for processing and updates the variable at the specified index
        /// </summary>
        /// <param name="BootOptionIndex"></param>
        /// <param name="UpdateOptionAction"></param>
        public static void EditLoadOption(ushort BootOptionIndex, Action<FirmwareBootOption> UpdateOptionAction)
        {
            FirmwareBootOption bootOption = ReadLoadOption(BootOptionIndex);
            UpdateOptionAction(bootOption);
            WriteFirmwareLoadOption(bootOption, BootOptionIndex);
        }

        /// <summary>
        /// Creates a new load option at the first free index, writes into it a serialized copy of the <see cref="LoadOptionBase"/> instance passed to the function, and returns the index of the new entry. Specify the <paramref name="AddFirst"/> parameter as <see langword="true"/> to add the option as the first boot option
        /// </summary>
        /// <param name="loadOption"></param>
        /// <param name="AddFirst"></param>
        /// <returns></returns>
        /// <exception cref="FreeLoadOptionIndexNotFound"></exception>
        public static ushort CreateLoadOption(LoadOptionBase loadOption, bool AddFirst)
        {
            // Getting free variable name
            ushort? newLoadOptionIndex = BS_ValueFormatHelper.GetFirstFreeLoadOptionName(LoadOrder);
            if (newLoadOptionIndex == null)
                throw new FreeLoadOptionIndexNotFound("Failed to find free loadOption name");

            // Creating variable
            WriteFirmwareLoadOption(loadOption, (ushort)newLoadOptionIndex);

            // Setting new boot order
            ushort[] bootorder = new ushort[FirmwareGlobalEnvironment.BootOrder.Length + 1];
            if (AddFirst)
            {
                // Adding as first element
                Array.Copy(FirmwareGlobalEnvironment.BootOrder, 0, bootorder, 1, FirmwareGlobalEnvironment.BootOrder.Length);
                bootorder[0] = (ushort)newLoadOptionIndex;
            }
            else
            {
                // Adding as last element
                Array.Copy(FirmwareGlobalEnvironment.BootOrder, 0, bootorder, 0, FirmwareGlobalEnvironment.BootOrder.Length);
                bootorder[bootorder.Length - 1] = (ushort)newLoadOptionIndex;
            }

            // Updating order
            FirmwareGlobalEnvironment.BootOrder = bootorder;
            return (ushort)newLoadOptionIndex;
        }

        /// <summary>
        /// Writes serialized copy of the <see cref="LoadOptionBase"/> instance passed to the function into existing load option variable at the specified index
        /// </summary>
        /// <param name="loadOption"></param>
        /// <param name="BootOptionIndex"></param>
        public static void UpdateLoadOption(LoadOptionBase loadOption, ushort BootOptionIndex)
        {
            // Updating variable
            WriteFirmwareLoadOption(loadOption, BootOptionIndex);
        }

        /// <summary>
        /// Reads the native representation of the boot option from NVRAM
        /// </summary>
        /// <param name="BootOptionIndex"></param>
        /// <returns></returns>
        public static EFI_LOAD_OPTION ReadRawLoadOption(ushort BootOptionIndex)
        {
            return ReadFirmwareLoadOption(BootOptionIndex);
        }

        /// <summary>
        /// Reads boot option from NVRAM
        /// </summary>
        /// <param name="BootOptionIndex"></param>
        /// <returns></returns>
        public static FirmwareBootOption ReadLoadOption(ushort BootOptionIndex)
        {
            EFI_LOAD_OPTION loadOption = ReadFirmwareLoadOption(BootOptionIndex);
            return new FirmwareBootOption(loadOption);
        }

        /// <summary>
        /// Reads a boot option from NVRAM and converts it to the specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BootOptionIndex"></param>
        /// <returns></returns>
        public static T? ReadLoadOption<T>(ushort BootOptionIndex) where T : LoadOptionBase
        {
            EFI_LOAD_OPTION loadOption = ReadFirmwareLoadOption(BootOptionIndex);
            return (T?)Activator.CreateInstance(typeof(T), loadOption);
        }

        internal static void WriteFirmwareLoadOption(LoadOptionBase loadOption, ushort BootOptionIndex)
        {
            // Checking for End protocol
            if (!(loadOption.OptionProtocols.Last() is DevicePathProtocolEnd))
                throw new InvalidLoadOptionStrcutreException("Load option does not have DevicePathProtocolEnd at the end of the OptionProtocols list. This protocol is necessary for correct recording");

            // Marshalling structure to unmanaged memory pointer
            IntPtr pointer = Marshal.AllocHGlobal(loadOption.GetStructureLength());
            using (BinaryWriter writer = new BinaryWriter(new MemoryPointerStream(pointer, loadOption.GetStructureLength(), true), Encoding.Unicode, true))
                loadOption.MarshalToBinaryWriter(writer);
#if DEBUG
            byte[] DebugData = new byte[loadOption.StructureLength];
            Marshal.Copy(pointer, DebugData, 0, loadOption.StructureLength);
#endif
            // Writing variable to firmware
            FirmwareUtilities.SetGlobalEnvironmentVariable(
                BS_ValueFormatHelper.GetLoadOptionNameFromIndex(BootOptionIndex),
                pointer,
                loadOption.GetStructureLength());

            Marshal.FreeHGlobal(pointer);
        }

        internal static EFI_LOAD_OPTION ReadFirmwareLoadOption(ushort loadOptionIndex)
        {
            // Getting variable data
            IntPtr pointer = FirmwareUtilities.GetGlobalEnvironmentVariable(BS_ValueFormatHelper.GetLoadOptionNameFromIndex(loadOptionIndex), out int DataLength, 1024);
#if DEBUG
            byte[] DebugData = new byte[DataLength];
            Marshal.Copy(pointer, DebugData, 0, DataLength);
#endif
            // Marshalling variable
            using BinaryReader reader = new BinaryReader(new MemoryPointerStream(pointer, DataLength, true));
            EFI_LOAD_OPTION loadOption = LoadOptionMarshaller.BinaryReaderToStructure(reader);
            Marshal.FreeHGlobal(pointer);
            return loadOption;
        }
    }
}
