using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Rikitav.IO.ExtensibleFirmware.BootService
{
    public static class FirmwareBootService
    {
        public static ushort CurrentLoadOptionIndex
        {
            get => FirmwareGlobalEnvironment.BootCurrent;
        }

        public static ushort NextLoadOptionIndex
        {
            set => FirmwareGlobalEnvironment.BootNext = value;
        }

        public static ushort[] LoadOrder
        {
            get => FirmwareGlobalEnvironment.BootOrder;
            set => FirmwareGlobalEnvironment.BootOrder = value;
        }

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

        public static void EditLoadOption(ushort BootOptionIndex, Action<FirmwareBootOption> UpdateOptionAction)
        {
            FirmwareBootOption bootOption = ReadLoadOption(BootOptionIndex);
            UpdateOptionAction(bootOption);
            WriteFirmwareLoadOption(bootOption, BootOptionIndex);
        }

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
                Array.Copy(FirmwareGlobalEnvironment.BootOrder, 0, bootorder, 1, FirmwareGlobalEnvironment.BootOrder.Length);
                bootorder[0] = (ushort)newLoadOptionIndex;
            }
            else
            {
                Array.Copy(FirmwareGlobalEnvironment.BootOrder, 0, bootorder, 0, FirmwareGlobalEnvironment.BootOrder.Length);
                bootorder[bootorder.Length - 1] = (ushort)newLoadOptionIndex;
            }

            FirmwareGlobalEnvironment.BootOrder = bootorder;
            return (ushort)newLoadOptionIndex;
        }

        public static void UpdateLoadOption(LoadOptionBase loadOption, ushort BootOptionIndex)
        {
            // Updating variable
            WriteFirmwareLoadOption(loadOption, BootOptionIndex);
        }

        public static EFI_LOAD_OPTION GetRawLoadOption(ushort BootOptionIndex)
        {
            return ReadFirmwareLoadOption(BootOptionIndex);
        }

        public static FirmwareBootOption ReadLoadOption(ushort BootOptionIndex)
        {
            EFI_LOAD_OPTION loadOption = ReadFirmwareLoadOption(BootOptionIndex);
            return new FirmwareBootOption(loadOption);
        }

        public static T? ReadLoadOption<T>(ushort BootOptionIndex) where T : LoadOptionBase
        {
            EFI_LOAD_OPTION loadOption = ReadFirmwareLoadOption(BootOptionIndex);
            return (T?)Activator.CreateInstance(typeof(T), loadOption);
        }

        internal static void WriteFirmwareLoadOption(LoadOptionBase loadOption, ushort BootOptionIndex)
        {
            // Marshalling structure to unmanaged memory pointer
            IntPtr pointer = Marshal.AllocHGlobal(loadOption.StructureLength);
            using (BinaryWriter writer = new BinaryWriter(new MemoryPointerStream(pointer, loadOption.StructureLength, true), Encoding.Unicode, true))
                loadOption.MarshalToBinaryWriter(writer);
#if DEBUG
            byte[] DebugData = new byte[loadOption.StructureLength];
            Marshal.Copy(pointer, DebugData, 0, loadOption.StructureLength);
#endif
            // Writing variable to firmware
            FirmwareUtilities.SetGlobalEnvironmentVariable(
                BS_ValueFormatHelper.GetLoadOptionNameFromIndex(BootOptionIndex),
                pointer,
                loadOption.StructureLength);

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
