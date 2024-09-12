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

using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.Win32Native;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// The Hard Drive Media Device Path is used to represent a partition on a hard drive.
    /// <see href="https://uefi.org/specs/UEFI/2.9_A/10_Protocols_Device_Path_Protocol.html#hard-drive-media-device-path"/>
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.Media, 1, typeof(HardDriveMediaDevicePath))]
    public class HardDriveMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.Media;

        /// <inheritdoc/>
        public override byte SubType => 1;

        /// <inheritdoc/>
        public override ushort DataLength => 42;

        /// <summary>
        /// Describes the entry in a partition table, starting with entry 1. Partition number zero represents the entire device. Valid partition numbers for a MBR partition are [1, 4]. Valid partition numbers for a GPT partition are [1, NumberOfPar titionEntries].
        /// </summary>
        public uint PartitionNumber { get; set; }

        /// <summary>
        /// Starting LBA of the partition on the hard drive
        /// </summary>
        public ulong PartitionStart { get; set; }

        /// <summary>
        /// Size of the partition in units of Logical Blocks
        /// </summary>
        public ulong PartitionSize { get; set; }

        /// <summary>
        /// Signature unique to this partition, this field contains a 16 byte signature.
        /// </summary>
        public Guid GptPartitionSignature { get; set; }

        /// <summary>
        /// Partition Format: (Unused values reserved) 0x01 - PC-AT compatible legacy MBR (Legacy MBR) . Partition Start and Partition Size come from Parti tionStartingLBA and PartitionSizeInLBA for the partition.0x02—GUID Partition Table
        /// </summary>
        public PartitionFormat PartitionFormat { get; set; }

        /// <summary>
        /// Type of Disk Signature: (Unused values reserved) 0x00 - No Disk Signature. 0x01 - 32-bit signature from address 0x1b8 of the type 0x01 MBR. 0x02 - GUID signature.
        /// </summary>
        public SignatureType SignatureType { get; set; }

        /// <summary>
        /// Create new <see cref="HardDriveMediaDevicePath"/> protocol instance
        /// </summary>
        public HardDriveMediaDevicePath()
            : base() { }

        /// <summary>
        /// Create new <see cref="HardDriveMediaDevicePath"/> protocol instance from <see cref="PARTITION_INFORMATION_EX"/> structure
        /// </summary>
        /// <param name="partition"></param>
        /// <exception cref="InvalidDataException"></exception>
        public HardDriveMediaDevicePath(PARTITION_INFORMATION_EX partition) : base()
        {
            if (partition.PartitionStyle != 2)
                throw new InvalidDataException("Partition information describe MBR pr RAW based partition");

            Deserialize(partition);
        }

        /// <summary>
        /// Create a protocol from unique partition <see cref="Guid"/>
        /// </summary>
        /// <param name="partitionIdentificator"></param>
        /// <exception cref="ArgumentException"></exception>
        public HardDriveMediaDevicePath(Guid partitionIdentificator) : base()
        {
            if (partitionIdentificator == Guid.Empty)
                throw new ArgumentException("Partition identificator has empty value");

            Deserialize(partitionIdentificator);
        }

        private void Deserialize(PARTITION_INFORMATION_EX partInfo)
        {
            PartitionNumber = partInfo.PartitionNumber;
            PartitionStart = (ulong)partInfo.StartingOffset;
            PartitionSize = (ulong)partInfo.PartitionLength;
            GptPartitionSignature = partInfo.Gpt.PartitionId;
            PartitionFormat = PartitionFormat.GuidPartitionTable;
            SignatureType = SignatureType.GuidSignature;
        }

        private void Deserialize(Guid partIdentificator)
        {
            string partPath = @"\\?\Volume{" + partIdentificator + "}";
            IntPtr partHandle = NativeMethods.CreateFile(partPath, 0, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            // Checking for INVALID_HANDLE_VALUE
            if (partHandle == new IntPtr(-1))
            {
                int lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError, "Failed to open partition descriptor");
            }

            // Allocating structure memory
            int StructSize = Marshal.SizeOf<PARTITION_INFORMATION_EX>();
            IntPtr StructPtr = Marshal.AllocHGlobal(StructSize);
            Marshal.StructureToPtr(new PARTITION_INFORMATION_EX(), StructPtr, true);

            //Executing DeviceIoControl()
            if (!NativeMethods.DeviceIoControl(partHandle, NativeMethods.DiskGetPartitionInfoEx, IntPtr.Zero, 0, StructPtr, (uint)StructSize, out _, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to obtain PARTITION_INFORMATION_EX structure");

            //creating new struct from allocated byte buffer
            PARTITION_INFORMATION_EX partInfoEx = Marshal.PtrToStructure<PARTITION_INFORMATION_EX>(StructPtr);
            Marshal.FreeHGlobal(StructPtr);

            // Deserializing
            Deserialize(partInfoEx);
        }

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData)
        {
            using BinaryReader reader = new BinaryReader(new MemoryStream(protocolData));

            PartitionNumber = reader.ReadUInt32();
            PartitionStart = reader.ReadUInt64() * 512;
            PartitionSize = reader.ReadUInt64() * 512;
            GptPartitionSignature = new Guid(reader.ReadBytes(16));
            PartitionFormat = (PartitionFormat)reader.ReadByte();
            SignatureType = (SignatureType)reader.ReadByte();
        }

        /// <inheritdoc/>
        protected override byte[] Serialize()
        {
            using MemoryStream protocolData = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(protocolData);

            writer.Write(PartitionNumber);
            writer.Write(PartitionStart / 512);
            writer.Write(PartitionSize / 512);
            writer.Write(GptPartitionSignature.ToByteArray());
            writer.Write((byte)PartitionFormat);
            writer.Write((byte)SignatureType);

            return protocolData.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString() => GptPartitionSignature.ToString();

        private static class NativeMethods
        {
            public const uint DiskGetPartitionInfoEx = 0x00000007 << 16 | 0x0012 << 2 | 0 | 0 << 14;

            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool DeviceIoControl(
                IntPtr hDevice,
                uint IoControlCode,
                [In] IntPtr InBuffer,
                uint nInBufferSize,
                [Out] IntPtr OutBuffer,
                uint nOutBufferSize,
                out uint pBytesReturned,
                [In] IntPtr Overlapped);

            [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr CreateFile(
                [MarshalAs(UnmanagedType.LPWStr)] string filename,
                [MarshalAs(UnmanagedType.U4)] FileAccess access,
                [MarshalAs(UnmanagedType.U4)] FileShare share,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
                [In] IntPtr templateFile);
        }
    }

    /// <summary>
    /// Partition format
    /// </summary>
    public enum PartitionFormat : byte
    {
        /// <summary>
        /// Mastre boot record
        /// </summary>
        LegacyMBR = 0x01,

        /// <summary>
        /// Guid partition table
        /// </summary>
        GuidPartitionTable = 0x02
    }

    /// <summary>
    /// Signature type
    /// </summary>
    public enum SignatureType : byte
    {
        /// <summary>
        /// Raw
        /// </summary>
        NoDiskSignature = 0x00,

        /// <summary>
        /// Mastre boot record
        /// </summary>
        MbrSignature = 0x01,

        /// <summary>
        /// Guid partition table
        /// </summary>
        GuidSignature = 0x02
    }
}
