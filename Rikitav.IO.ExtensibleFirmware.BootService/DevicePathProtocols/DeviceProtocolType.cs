namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    public enum DeviceProtocolType : byte
    {
        HW = 0x01,
        ACPI = 0x02,
        MSG = 0x03,
        MEDIA = 0x04,
        BIOS = 0x05,
        END = 0x7F
    }
}
