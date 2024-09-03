using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    internal class DevicePathProtocolWrapperSelector
    {
        private static DevicePathProtocolWrapperSelector? _Instance = null;
        public static DevicePathProtocolWrapperSelector Instance => _Instance ??= new DevicePathProtocolWrapperSelector();

        private Dictionary<TypeTuple, Type> _RegisteredTypes = new Dictionary<TypeTuple, Type>();
        private readonly string[] _KnownProtocolDefineAssemblies = new string[]
        {
            "Rikitav.IO.ExtensibleFirmware.HardwareDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.AcpiDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.MessagingDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols",
            "Rikitav.IO.ExtensibleFirmware.BiosBootSpecificationDevicePathProtocols"
        };

        private DevicePathProtocolWrapperSelector()
        {
            AppDomain.CurrentDomain.GetAssemblies();
            foreach (string assName in _KnownProtocolDefineAssemblies)
            {
                try
                {
                    Assembly loadedAssembly = Assembly.Load(assName);
                    foreach (Type exported in loadedAssembly.GetExportedTypes())
                    {
                        DefineDevicePathProtocolAttribute? attr = exported.GetCustomAttribute<DefineDevicePathProtocolAttribute>();
                        if (attr == null)
                            continue; //Skip

                        _RegisteredTypes.Add(new TypeTuple(attr.Type, attr.SubType), exported);
                    }
                }
                catch
                {
                    // Skip
                }
            }
        }

        public Type? GetRegisteredType(DeviceProtocolType type, byte subType)
        {
            if (type == DeviceProtocolType.END && subType == 0xFF)
                return typeof(DevicePathProtocolEnd);

            return _RegisteredTypes.TryGetValue(new TypeTuple(type, subType), out Type registered)
                ? registered
                : null;
        }

        private struct TypeTuple
        {
            public DeviceProtocolType Type;
            public byte SubType;

            public TypeTuple(DeviceProtocolType type, byte subType)
            {
                Type = type;
                SubType = subType;
            }
        }
    }
}
