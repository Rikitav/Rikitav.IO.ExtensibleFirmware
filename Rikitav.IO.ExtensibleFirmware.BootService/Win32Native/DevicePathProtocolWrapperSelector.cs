using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    /// <summary>
    /// An assistant class that tells the marshaler what type to use to represent the native <see cref="EFI_DEVICE_PATH_PROTOCOL"/> structure
    /// </summary>
    public static class DevicePathProtocolWrapperSelector
    {
        private static readonly Dictionary<short, Type> RegisteredWrapperTypes = new Dictionary<short, Type>();
        private static readonly string[] KnownWrapperLibraries = new string[]
        {
            "Rikitav.IO.ExtensibleFirmware.HardwareDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.AcpiDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.MessagingDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.BiosBootSpecificationDevicePathProtocols"
        };

        static DevicePathProtocolWrapperSelector()
        {
            EnumerateKnownLibraries();
        }

        internal static void EnumerateKnownLibraries()
        {
            foreach (string wrapperLib in KnownWrapperLibraries)
            {
                try
                {
                    Assembly assembly = Assembly.Load(wrapperLib);
                    RegisterWrapperLibrary(assembly);
                }
                catch
                {
                    // Library doesnt exist in domain
                }
            }
        }

        internal static Type? GetRegisteredType(DeviceProtocolType type, byte subType)
        {
            short wrapperIdentificator = DPPWS_Helper.GetWrapperIdentificator(type, subType);
            return RegisteredWrapperTypes.TryGetValue(wrapperIdentificator, out Type registered) ? registered : null;
        }

        /// <summary>
        /// Scans and logs all valid protocol wrapper types
        /// </summary>
        /// <param name="assembly"></param>
        public static void RegisterWrapperLibrary(Assembly assembly)
        {
            foreach (Type wrapperType in assembly.GetExportedTypes())
            {
                if (!wrapperType.IsSubclassOf(typeof(DevicePathProtocolBase)))
                    continue;

                DefineDevicePathProtocolAttribute? defineWrapperAttr = wrapperType.GetCustomAttribute<DefineDevicePathProtocolAttribute>();
                if (defineWrapperAttr == null)
                    continue;

                short wrapperIdentificator = DPPWS_Helper.GetWrapperIdentificator(defineWrapperAttr.Type, defineWrapperAttr.SubType);
                RegisteredWrapperTypes.Add(wrapperIdentificator, wrapperType);
            }
        }

        /// <summary>
        /// Registers a protocol wrapper type for later use in marshaling boot options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterType<T>() where T : DevicePathProtocolBase
        {
            RegisterType(typeof(T));
        }

        /// <summary>
        /// Registers a protocol wrapper type for later use in marshaling boot options
        /// </summary>
        /// <param name="wrapperType"></param>
        /// <exception cref="InvalidInheritedClassException"></exception>
        /// <exception cref="MissingDevicePathProtocolWrapperAttributeException"></exception>
        public static void RegisterType(Type wrapperType)
        {
            if (!wrapperType.IsSubclassOf(typeof(DevicePathProtocolBase)))
                throw new InvalidInheritedClassException("The type of protocol wrapper must be inherited from class DevicePathProtocolBase");

            DefineDevicePathProtocolAttribute defineWrapperAttr = wrapperType.GetCustomAttribute<DefineDevicePathProtocolAttribute>();
            if (defineWrapperAttr == null)
                throw new MissingDevicePathProtocolWrapperAttributeException("The protocol wrapper does not have the required \'Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols.DefineDevicePathProtocolAttribute\'");

            short wrapperIdentificator = DPPWS_Helper.GetWrapperIdentificator(defineWrapperAttr.Type, defineWrapperAttr.SubType);
            RegisteredWrapperTypes.Add(wrapperIdentificator, wrapperType);
        }

        private static class DPPWS_Helper
        {
            public static short GetWrapperIdentificator(DeviceProtocolType type, byte subType)
                => (short)((byte)type + (subType << 8));
        }
    }
}
