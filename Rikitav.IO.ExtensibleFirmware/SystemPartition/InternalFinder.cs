using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Rikitav.IO.ExtensibleFirmware.Win32Native;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    internal static class InternalFinder
    {
        public static PARTITION_INFORMATION_EX FindEfiPartitionInfo()
        {
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
                if (DeviceControlResult.Gpt.PartitionType == EfiPartition.PartitionType)
                    return DeviceControlResult;
            }

            // Else throw DirNotFound
            throw new DriveNotFoundException();
        }

        internal static class NativeMethods
        {
            public const uint IOCTL_DISK_GET_PARTITION_INFO_EX = (0x00000007 << 16) | (0x0012 << 2) | 0 | (0 << 14);
            public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        }
    }
}
