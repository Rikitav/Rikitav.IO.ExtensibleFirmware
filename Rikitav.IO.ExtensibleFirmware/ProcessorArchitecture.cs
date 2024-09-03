namespace Rikitav.IO.ExtensibleFirmware
{
    public enum ProcessorArchitecture
    {
        /// <summary>
        /// Unknown architecture
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 32-bit
        /// </summary>
        Ia32 = 0x14c,
        
        /// <summary>
        /// AMD64
        /// </summary>
        x64 = 0x8664,

        /// <summary>
        /// Intel Itanium 64
        /// </summary>
        Ia64 = 0x200,

        /// <summary>
        /// AArch32 architecture
        /// </summary>
        Arm = 0x1c2,

        /// <summary>
        /// AArch64 architecture
        /// </summary>
        AArch64 = 0xaa64,

        /// <summary>
        /// RISC-V 32-bit architecture
        /// </summary>
        RISC_V32 = 0x5032,

        /// <summary>
        /// RISC-V 64-bit architecture
        /// </summary>
        RISC_V64 = 0x5064,

        /// <summary>
        /// RISC-V 128-bit architecture
        /// </summary>
        RISC_V128 = 0x5128
    }
}
