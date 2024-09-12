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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    internal class DevicePathProtocolWrapperSelector
    {
        private static DevicePathProtocolWrapperSelector? _Instance = null;
        public static DevicePathProtocolWrapperSelector Instance => _Instance ??= new DevicePathProtocolWrapperSelector();

        private readonly Dictionary<TypeTuple, Type> _RegisteredTypes = new Dictionary<TypeTuple, Type>();
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
            if (type == DeviceProtocolType.End && subType == 0xFF)
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
