using System;
using System.Collections.Generic;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware
{
    public enum ProcessorArchitecture
    {
        Unknown = 0,
        AArch64 = 0xaa64,
        Arm = 0x1c2,
        Ia32 = 0x14c,
        x64 = 0x8664
    }
}
