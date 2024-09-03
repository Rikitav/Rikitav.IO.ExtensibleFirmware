using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    public struct EFI_DEVICE_PATH_PROTOCOL
    {
        public DeviceProtocolType Type;
        public byte SubType;
        public ushort Length;
        public byte[] Data;
    }
}
