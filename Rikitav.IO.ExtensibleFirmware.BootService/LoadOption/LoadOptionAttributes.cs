using System;

namespace Rikitav.IO.ExtensibleFirmware.BootService.LoadOption
{
    [Flags]
    public enum LoadOptionAttributes
    {
        ACTIVE = 0x00000001,
        FORCE_RECONNECT = 0x00000002,
        HIDDEN = 0x00000008,
        CATEGORY = 0x00001F00,
        CATEGORY_BOOT = 0x00000000,
        CATEGORY_APP = 0x00000100
    }
}
