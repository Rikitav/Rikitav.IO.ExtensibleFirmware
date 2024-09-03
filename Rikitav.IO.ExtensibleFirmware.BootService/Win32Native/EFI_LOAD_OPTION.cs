using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    public struct EFI_LOAD_OPTION
    {
        public LoadOptionAttributes Attributes;
        public ushort FilePathListLength;
        public string Description;
        public EFI_DEVICE_PATH_PROTOCOL[] FilePathList;
        public byte[] OptionalData;
    }
}
