using System;

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefineDevicePathProtocolAttribute : Attribute
    {
        public DeviceProtocolType Type { get; private set; }
        public byte SubType { get; private set; }

        public DefineDevicePathProtocolAttribute(DeviceProtocolType type, byte subType, Type protocolWrapperType)
        {
            // Checking inherited class
            if (!typeof(DevicePathProtocolBase).IsAssignableFrom(protocolWrapperType))
                throw new InvalidInheritedClassException(string.Format("The type {0} is not a descendant of class {1}", protocolWrapperType.Name, typeof(DevicePathProtocolBase).Name));

            // Assigning info
            Type = type;
            SubType = subType;
        }
    }
}
