using System;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware.Win32Native
{
    /// <summary>
    /// Contains extended information about a drive's partitions.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct DRIVE_LAYOUT_INFORMATION_EX
    {
        [FieldOffset(0)] public uint PartitionStyle;
        [FieldOffset(4)] public uint PartitionCount;
        [FieldOffset(8)] public IntPtr Mbr;
        [FieldOffset(8)] public IntPtr Gpt;
        [FieldOffset(48)] public PARTITION_INFORMATION_EX PartitionEntry;
    }

    /// <summary>
    /// Contains GUID partition table (GPT) partition information.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal struct PARTITION_INFORMATION_GPT
    {
        [FieldOffset(0)] public Guid PartitionType;
        [FieldOffset(16)] public Guid PartitionId;
        [FieldOffset(32)] public ulong Attributes;
        [FieldOffset(40), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)] public string Name;
    }

    /// <summary>
    /// Contains information about a disk partition.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PARTITION_INFORMATION_EX
    {
        [FieldOffset(0)] public uint PartitionStyle;
        [FieldOffset(8)] public long StartingOffset;
        [FieldOffset(16)] public long PartitionLength;
        [FieldOffset(24)] public uint PartitionNumber;
        [FieldOffset(28), MarshalAs(UnmanagedType.I1)] public bool RewritePartition;
        [FieldOffset(32)] public IntPtr Mbr;
        [FieldOffset(32)] public PARTITION_INFORMATION_GPT Gpt;
    }
}
