using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware
{
    public class FirmwareUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        public enum FirmwareType
        {
            Unknown = 0,
            Bios = 1,
            Uefi = 2,
            Max = 3
        }

        public static FirmwareType GetFirmwareType()
        {
            if (NativeMethods.GetFirmwareType(out FirmwareType FwType))
                return FwType;

            return FirmwareType.Unknown;
        }

        internal static IntPtr GetPartitionHandle(string DevicePath)
        {
            return NativeMethods.CreateFile(
                DevicePath.TrimEnd('\\'), // Partition DevicePath
                0, FileShare.Read,
                IntPtr.Zero, FileMode.Open,
                0, IntPtr.Zero);
        }

        internal static TOutBuffer GetIoDeviceData<TOutBuffer>(IntPtr hDevice, uint dwIoControlCode) where TOutBuffer : struct
        {
            int OutBufferSize = Marshal.SizeOf<TOutBuffer>();
            return SaveAllocate.Global(OutBufferSize, StructPtr =>
            {
                // Copying structure to allocated memory
                Marshal.StructureToPtr(new TOutBuffer(), StructPtr, true);
                uint lpBytesReturned = 0;

                // Executing DeviceIoControl()
                bool bResult = NativeMethods.DeviceIoControl(
                    hDevice, dwIoControlCode,
                    IntPtr.Zero, 0,
                    StructPtr, (uint)OutBufferSize,
                    ref lpBytesReturned, IntPtr.Zero);

                // Creating new struct from allocated byte buffer
                return Marshal.PtrToStructure<TOutBuffer>(StructPtr);
            });
        }

        internal static PARTITION_INFORMATION_EX FindEfiSystemPartitionInfo()
        {
            // Enumerate all partitions by WinApi FindVolume function
            foreach (DirectoryInfo Volume in new PartitionEnumerable())
            {
                IntPtr VolumeHandle = GetPartitionHandle(Volume.Name.TrimEnd('\\'));
                if (VolumeHandle == NativeMethods.INVALID_HANDLE_VALUE)
                {
                    NativeMethods.CloseHandle(VolumeHandle);
                    continue;
                }

                // Getting partition header data
                PARTITION_INFORMATION_EX DeviceControlResult = GetIoDeviceData<PARTITION_INFORMATION_EX>(VolumeHandle, NativeMethods.IOCTL_DISK_GET_PARTITION_INFO_EX);
                NativeMethods.CloseHandle(VolumeHandle);

                // If Header type equals searchable, return his ID
                if (DeviceControlResult.Gpt.PartitionType == FirmwareInterface.PartitionTypeIdentificator)
                    return DeviceControlResult;
            }

            // Else throw DirNotFound
            throw new DriveNotFoundException();
        }

        internal static DirectoryInfo FindEfiSystemPartitionDirectoryInfo()
        {
            // Enumerate all partitions by WinApi FindVolume function
            foreach (DirectoryInfo Volume in new PartitionEnumerable())
            {
                // Getting partition header data
                IntPtr VolumeHandle = GetPartitionHandle(Volume.Name.TrimEnd('\\'));
                if (VolumeHandle == NativeMethods.INVALID_HANDLE_VALUE)
                    continue;

                PARTITION_INFORMATION_EX DeviceControlResult = GetIoDeviceData<PARTITION_INFORMATION_EX>(VolumeHandle, NativeMethods.IOCTL_DISK_GET_PARTITION_INFO_EX);
                NativeMethods.CloseHandle(VolumeHandle);

                // If Header type equals searchable, return his ID
                if (DeviceControlResult.Gpt.PartitionType == FirmwareInterface.PartitionTypeIdentificator)
                    return Volume;
            }

            // Else throw DirNotFound
            throw new DriveNotFoundException();
        }

        internal static class NativeMethods
        {
            public const uint IOCTL_DISK_GET_PARTITION_INFO_EX = (0x00000007 << 16) | (0x0012 << 2) | 0 | (0 << 14);
            public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool DeviceIoControl(
                IntPtr hDevice,
                uint IoControlCode,
                [In] IntPtr InBuffer,
                uint nInBufferSize,
                [Out] IntPtr OutBuffer,
                uint nOutBufferSize,
                ref uint pBytesReturned,
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

            #region Firmware type
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool GetFirmwareType(out FirmwareType FwType);
            #endregion
        }
    }
}
