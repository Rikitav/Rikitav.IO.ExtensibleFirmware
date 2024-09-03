using System;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware
{
#pragma warning disable IDE1006
    public static class FirmwareGlobalEnvironment
    {
        /// <summary>
        /// Whether the system is operating in Audit Mode (1) or not (0). All other values are reserved. Should be treated as read-only except when DeployedMode is 0. Always becomes read-only after ExitBootServices() is called.
        /// Variable attributes : BS, RT
        /// </summary>
        public static bool AuditMode
        {
            get => ReadVariable<bool>(nameof(AuditMode));
        }

        /// <summary>
        /// The boot option that was selected for the current boot.
        /// Variable attributes : BS, RT
        /// </summary>
        public static ushort BootCurrent
        {
            get => ReadVariable<ushort>(nameof(BootCurrent));
        }

        /// <summary>
        /// The boot option for the next boot only.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static ushort BootNext
        {
            get => ReadVariable<ushort>(nameof(BootNext));
            set => WriteVariable(nameof(BootNext), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// The ordered boot option load list.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static ushort[] BootOrder
        {
            get => ReadUInt16ArrayVariable(nameof(BootOrder));
            set => WriteUInt16ArrayVariable(nameof(BootOrder), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// The types of boot options supported by the boot manager. Should be treated as read-only.
        /// Variable attributes : BS, RT
        /// </summary>
        public static uint BootOptionSupport
        {
            get => ReadVariable<uint>(nameof(BootOptionSupport));
            set => WriteVariable(nameof(BootOptionSupport), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /*
        /// <summary>
        /// The device path of the default input console.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static EFI_DEVICE_PATH_PROTOCOL ConIn
        {
            get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConIn));
            //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConIn), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
        }

        /// <summary>
        /// The device path of all possible console input devices.
        /// Variable attributes : BS, RT
        /// </summary>
        public static EFI_DEVICE_PATH_PROTOCOL ConInDev
        {
            get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConInDev));
            //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConInDev), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
        }

        /// <summary>
        /// The device path of the default output console.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static EFI_DEVICE_PATH_PROTOCOL ConOut
        {
            get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOut));
            //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOut), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
        }

        /// <summary>
        /// The device path of all possible console output devices.
        /// Variable attributes : BS, RT
        /// </summary>
        public static EFI_DEVICE_PATH_PROTOCOL ConOutDev
        {
            get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOutDev));
            //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOutDev), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
        }
        */

        /// <summary>
        /// Whether the system is operating in Deployed Mode (1) or not (0). All other values are reserved. Should be treated as read-only when its value is 1. Always becomes read-only after ExitBootServices() is called.
        /// Variable attributes : BS, RT
        /// </summary>
        public static bool DeployedMode
        {
            get => ReadVariable<bool>(nameof(DeployedMode));
        }

        /// <summary>
        /// Whether the platform firmware is operating in device authentication boot mode (1) or not (0). All other values are reserved. Should be treated as read-only.
        /// Variable attributes : BS, RT
        /// </summary>
        public static bool devAuthBoot
        {
            get => ReadVariable<bool>(nameof(devAuthBoot));
        }

        /// <summary>
        /// The ordered driver load option list.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static ushort[] DriverOrder
        {
            get => ReadUInt16ArrayVariable(nameof(DriverOrder));
            set => WriteUInt16ArrayVariable(nameof(DriverOrder), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /*
        /// <summary>
        /// The device path of the default error output device.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static EFI_DEVICE_PATH_PROTOCOL ErrOut
        {
            get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ErrOut));
        }

        /// <summary>
        /// The device path of all possible error output devices.
        /// Variable attributes : BS, RT
        /// </summary>
        public static EFI_DEVICE_PATH_PROTOCOL ErrOutDev
        {
            get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ErrOutDev));
        }
        */

        /// <summary>
        /// Identifies the level of hardware error record persistence support implemented by the platform. This variable is only modified by firmware and is read-only to the OS.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static ushort HwErrRecSupport
        {
            get => ReadVariable<ushort>(nameof(HwErrRecSupport));
        }

        /// <summary>
        /// The language code that the system is configured for. This value is deprecated.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static string Lang
        {
            get => ReadStringVariable(nameof(Lang));
            set => WriteStringVariable(nameof(Lang), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// The language codes that the firmware supports. This value is deprecated.
        /// Variable attributes : BS, RT
        /// </summary>
        public static string LangCodes
        {
            get => ReadStringVariable(nameof(LangCodes));
            set => WriteStringVariable(nameof(LangCodes), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// Allows the OS to request the firmware to enable certain features and to take certain actions.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static EfiOsIindications OsIndications
        {
            get => (EfiOsIindications)ReadVariable<long>(nameof(OsIndications));
            set => WriteVariable(nameof(OsIndications), (long)value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// Allows the firmware to indicate supported features and actions to the OS.
        /// Variable attributes : BS, RT
        /// </summary>
        public static EfiOsIindications OsIndicationsSupported
        {
            get => (EfiOsIindications)ReadVariable<long>(nameof(OsIndicationsSupported));
        }

        /// <summary>
        /// OS-specified recovery options.
        /// Variable attributes : BS, RT, NV, AT
        /// </summary>
        public static ushort[] OsRecoveryOrder
        {
            get => ReadUInt16ArrayVariable(nameof(OsRecoveryOrder));
            set => WriteUInt16ArrayVariable(nameof(OsRecoveryOrder), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS | VariableAttributes.NON_VOLATILE | VariableAttributes.AUTHENTICATED_WRITE_ACCESS
        }

        /// <summary>
        /// The language codes that the firmware supports.
        /// Variable attributes : BS, RT
        /// </summary>
        public static string PlatformLangCodes
        {
            get => ReadStringVariable(nameof(PlatformLangCodes));
            set => WriteStringVariable(nameof(PlatformLangCodes), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// The language code that the system is configured for.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static string PlatformLang
        {
            get => ReadStringVariable(nameof(PlatformLang));
            set => WriteStringVariable(nameof(PlatformLang), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /*
        /// <summary>
        /// Array of GUIDs representing the type of signatures supported by the platform firmware. Should be treated as read-only.
        /// Variable attributes : BS, RT
        /// </summary>
        public static Guid[] SignatureSupport
        {
            get
            {
                // Getting variable data
                int ptrSize = 16;
                IntPtr Data = FirmwareUtilities.GetGlobalEnvironmentVariable(nameof(SignatureSupport), out _, out int DataSize, 256);

                // Parsing values
                ushort[] varVal = new ushort[DataSize / ptrSize];
                for (int i = 0; i < varVal.Length; i++)
                {
                    ushort arrVal = (ushort)Marshal.Read(Data, i * ptrSize);
                    varVal[i] = arrVal;
                }

                // Freeing
                Marshal.FreeHGlobal(Data);
                return varVal;
            }
        }
        */

        /// <summary>
        /// Whether the platform firmware is operating in Secure boot mode (1) or not (0). All other values are reserved. Should be treated as read-only.
        /// Variable attributes : BS, RT
        /// </summary>
        public static bool SecureBoot
        {
            get => ReadVariable<bool>(nameof(SecureBoot));
        }

        /// <summary>
        /// Whether the system should require authentication on SetVariable() requests to Secure Boot policy variables (0) or not (1). Should be treated as read-only. The system is in "Setup Mode" when SetupMode==1, AuditMode==0, and DeployedMode==0.
        /// Variable attributes : BS, RT
        /// </summary>
        public static bool SetupMode
        {
            get => ReadVariable<bool>(nameof(SetupMode));
        }

        /// <summary>
        /// The firmware's boot managers timeout, in seconds, before initiating the default boot selection.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static ushort Timeout
        {
            get => ReadVariable<ushort>(nameof(Timeout));
            set => WriteVariable(nameof(Timeout), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
        }

        /// <summary>
        /// Whether the system is configured to use only vendor-provided keys or not. Should be treated as read-only.
        /// Variable attributes : BS, RT
        /// </summary>
        public static bool VendorKeys
        {
            get => ReadVariable<bool>(nameof(VendorKeys));
        }

        /*
        /// <summary>
        /// A boot load option. #### is a printed hex value. No 0x or h is included in the hex value.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static string BootOption(short OptIndex) => "Boot" + OptIndex.ToString("X").PadLeft(4, '0');

        /// <summary>
        /// A driver load option. #### is a printed hex value.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static string DriverOption(short OptIndex) => "Driver" + OptIndex.ToString("X").PadLeft(4, '0');

        /// <summary>
        /// Describes hot key relationship with a Boot#### load option.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static string KeyOption(short OptIndex) => "Key" + OptIndex.ToString("X").PadLeft(4, '0');

        /// <summary>
        /// Platform-specified recovery options. These variables are only modified by firmware and are read-only to the OS.
        /// Variable attributes : BS, RT
        /// </summary>
        public static string PlatformRecoveryOption(short OptIndex) => "PlatformRecovery" + OptIndex.ToString("X").PadLeft(4, '0');

        /// <summary>
        /// A System Prep application load option containing an EFI_LOAD_OPTION descriptor. #### is a printed hex value.
        /// Variable attributes : NV, BS, RT
        /// </summary>
        public static string SysPrepOption(short OptIndex) => "SysPrep" + OptIndex.ToString("X").PadLeft(4, '0');
        */

        private static string ReadStringVariable(string VarName)
        {
            IntPtr pointer = FirmwareUtilities.GetGlobalEnvironmentVariable(VarName, out _, 1024);
            string varVal = Marshal.PtrToStringUni(pointer);
            Marshal.FreeHGlobal(pointer);
            return varVal;
        }

        private static void WriteStringVariable(string VarName, string Value)
        {
            IntPtr pointer = Marshal.StringToHGlobalUni(Value);
            FirmwareUtilities.SetGlobalEnvironmentVariable(VarName, pointer, Value.Length * 2);
            Marshal.FreeHGlobal(pointer);
        }

        private static T ReadVariable<T>(string VarName) where T : struct
        {
            int ptrSize = Marshal.SizeOf<T>();
            IntPtr pointer = FirmwareUtilities.GetGlobalEnvironmentVariable(VarName, out _, ptrSize);
            T varVal = Marshal.PtrToStructure<T>(pointer);
            Marshal.FreeHGlobal(pointer);
            return varVal;
        }

        private static void WriteVariable<T>(string VarName, T Value) where T : struct
        {
            int ptrSize = Marshal.SizeOf<T>();
            IntPtr pointer = Marshal.AllocHGlobal(ptrSize);
            Marshal.StructureToPtr(Value, pointer, false);
            FirmwareUtilities.SetGlobalEnvironmentVariable(VarName, pointer, ptrSize);
            Marshal.FreeHGlobal(pointer);
        }

        private static ushort[] ReadUInt16ArrayVariable(string VarName)
        {
            // Getting variable data
            int ptrSize = 2;
            IntPtr Data = FirmwareUtilities.GetGlobalEnvironmentVariable(VarName, out int DataSize, 256);

            // Parsing values
            ushort[] varVal = new ushort[DataSize / ptrSize];
            for (int i = 0; i < varVal.Length; i++)
            {
                ushort arrVal = (ushort)Marshal.ReadInt16(Data, i * ptrSize);
                varVal[i] = arrVal;
            }

            // Freeing
            Marshal.FreeHGlobal(Data);
            return varVal;
        }

        private static void WriteUInt16ArrayVariable(string VarName, ushort[] Value)
        {
            // Formating new value
            int ptrSize = 2;
            GCHandle handle = GCHandle.Alloc(Value, GCHandleType.Pinned);

            // Setting variable
            FirmwareUtilities.SetGlobalEnvironmentVariable(VarName, handle.AddrOfPinnedObject(), Value.Length * ptrSize);
            if (handle.IsAllocated)
                handle.Free();
        }
    }
}
