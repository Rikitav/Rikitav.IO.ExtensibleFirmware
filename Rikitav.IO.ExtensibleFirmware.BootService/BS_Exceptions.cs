using System;
using System.ComponentModel;

namespace Rikitav.IO.ExtensibleFirmware.BootService
{
    public class InvalidLoadOrderException : Exception
    {
        public InvalidLoadOrderException(string Message)
            : base(Message) { }
    }

    public class InvalidLoadOptionException : Exception
    {
        public InvalidLoadOptionException(string Message)
            : base(Message) { }
    }

    public class InvalidHandleValueException : Win32Exception
    {
        public InvalidHandleValueException(int lastError)
            : base(lastError) { }
    }

    public class FreeLoadOptionIndexNotFound : Exception
    {
        public FreeLoadOptionIndexNotFound(string Message)
            : base(Message) { }
    }

    public class InvalidInheritedClassException : Exception
    {
        public InvalidInheritedClassException(string message)
            : base(message) { }
    }

    public class InvalidConstructorParametersException : Exception
    {
        public InvalidConstructorParametersException(string message)
            : base(message) { }
    }

    public class DeviceProtocolCastingException : Exception
    {
        public DeviceProtocolCastingException(string message)
            : base(message) { }
    }
}
