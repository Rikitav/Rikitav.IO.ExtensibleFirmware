using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware
{
    public class FirmwareUtilities
    {
        private static PARTITION_INFORMATION_EX _HashedPartitionInfo = default(PARTITION_INFORMATION_EX);

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

        internal static PARTITION_INFORMATION_EX FindEfiSystemPartitionInfo()
        {
            if (!_HashedPartitionInfo.Equals(default(PARTITION_INFORMATION_EX)))
            {
                // Means that EFI partition was already finded
                return _HashedPartitionInfo;
            }

            // Enumerate all partitions by WinApi FindVolume function
            foreach (DirectoryInfo Volume in new PartitionEnumerable())
            {
                IntPtr VolumeHandle = Win32Native.InteropInterface.GetPartitionHandle(Volume.Name.TrimEnd('\\'));
                if (VolumeHandle == NativeMethods.INVALID_HANDLE_VALUE)
                {
                    Win32Native.InteropInterface.CloseHandle(VolumeHandle);
                    continue;
                }

                // Getting partition header data
                PARTITION_INFORMATION_EX DeviceControlResult = Win32Native.InteropInterface.GetIoDeviceData<PARTITION_INFORMATION_EX>(VolumeHandle, NativeMethods.IOCTL_DISK_GET_PARTITION_INFO_EX);
                Win32Native.InteropInterface.CloseHandle(VolumeHandle);

                // If Header type equals searchable, return his ID
                if (DeviceControlResult.Gpt.PartitionType == FirmwareInterface.PartitionTypeIdentificator)
                {
                    _HashedPartitionInfo = DeviceControlResult;
                    return DeviceControlResult;
                }
            }
            // Else throw DirNotFound
            throw new DriveNotFoundException();
        }

        internal static DirectoryInfo FindEfiSystemPartitionDirectoryInfo()
        {
            if (!_HashedPartitionInfo.Equals(default(PARTITION_INFORMATION_EX)))
            {
                // Means that EFI partition was already finded
                return new DirectoryInfo("\\\\?\\Volume{" + _HashedPartitionInfo.Gpt.PartitionId + "}\\");
            }

            // Enumerate all partitions by WinApi FindVolume function
            foreach (DirectoryInfo Volume in new PartitionEnumerable())
            {
                // Getting partition header data
                IntPtr VolumeHandle = Win32Native.InteropInterface.GetPartitionHandle(Volume.Name.TrimEnd('\\'));
                if (VolumeHandle == NativeMethods.INVALID_HANDLE_VALUE)
                    continue;

                PARTITION_INFORMATION_EX DeviceControlResult = Win32Native.InteropInterface.GetIoDeviceData<PARTITION_INFORMATION_EX>(VolumeHandle, NativeMethods.IOCTL_DISK_GET_PARTITION_INFO_EX);
                Win32Native.InteropInterface.CloseHandle(VolumeHandle);

                // If Header type equals searchable, return his ID
                if (DeviceControlResult.Gpt.PartitionType == FirmwareInterface.PartitionTypeIdentificator)
                {
                    _HashedPartitionInfo = DeviceControlResult;
                    return Volume;
                }
            }

            // Else throw DirNotFound
            throw new DriveNotFoundException();
        }

        internal static class NativeMethods
        {
            public const uint IOCTL_DISK_GET_PARTITION_INFO_EX = (0x00000007 << 16) | (0x0012 << 2) | 0 | (0 << 14);
            public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            #region Firmware type
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool GetFirmwareType(out FirmwareType FwType);
            #endregion
        }
    }
}
