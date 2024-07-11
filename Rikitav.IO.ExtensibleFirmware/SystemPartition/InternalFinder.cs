using Microsoft.Win32.SafeHandles;
using Rikitav.IO.ExtensibleFirmware.Win32Native;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    internal static class InternalFinder
    {
        public static PARTITION_INFORMATION_EX FindEfiPartitionInfo()
        {
            if (!FirmwareInterface.Available)
                throw new PlatformNotSupportedException("Running on non UEFI System");

            // Enumerate all partitions by WinApi FindVolume function
            using SafeFileHandle hndl = NativeMethods.CreateFile(
                "\\\\.\\PhysicalDrive0",
                NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
                NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE,
                IntPtr.Zero,
                NativeMethods.OPEN_EXISTING,
                NativeMethods.FILE_ATTRIBUTE_READONLY,
                IntPtr.Zero);

            int lastError = 0;
            int DRIVE_LAYOUT_BUFFER_SIZE = 1024;

            for (int Attempt = 1; Attempt <= 5; Attempt++)
            {
                IntPtr driveLayoutPtr = Marshal.AllocHGlobal(1024 * Attempt);
                if (!NativeMethods.DeviceIoControl(hndl, NativeMethods.IOCTL_DISK_GET_DRIVE_LAYOUT_EX, IntPtr.Zero, 0, driveLayoutPtr, 1024 * (uint)Attempt, out _, IntPtr.Zero))
                {
                    lastError = Marshal.GetLastWin32Error();
                    if (lastError != NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                        throw new DriveNotFoundException();

                    Marshal.FreeHGlobal(driveLayoutPtr);
                }

                DRIVE_LAYOUT_INFORMATION_EX driveLayout = (DRIVE_LAYOUT_INFORMATION_EX)Marshal.PtrToStructure(driveLayoutPtr, typeof(DRIVE_LAYOUT_INFORMATION_EX));
                long PartitionEntryAddress = driveLayoutPtr.ToInt64() + Marshal.OffsetOf(typeof(DRIVE_LAYOUT_INFORMATION_EX), "PartitionEntry").ToInt64();
                Marshal.FreeHGlobal(driveLayoutPtr);

                for (uint ParIndex = 0; ParIndex < driveLayout.PartitionCount; ParIndex++)
                {
                    IntPtr ptr = new IntPtr(PartitionEntryAddress + (ParIndex * Marshal.SizeOf(typeof(PARTITION_INFORMATION_EX))));
                    PARTITION_INFORMATION_EX partInfo = (PARTITION_INFORMATION_EX)Marshal.PtrToStructure(ptr, typeof(PARTITION_INFORMATION_EX));

                    if (partInfo.Gpt.PartitionType == EfiPartition.TypeID)
                    {
                        return partInfo;
                    }
                }
            }

            // Else throw DirNotFound
            throw new DriveNotFoundException();
        }

        internal static class NativeMethods
        {
            public const uint GENERIC_WRITE = 0x01U << 30;
            public const uint GENERIC_READ = 0x01U << 31;
            public const uint FILE_SHARE_READ = 0x00000001U;
            public const uint FILE_SHARE_WRITE = 0x00000002U;
            public const uint OPEN_EXISTING = 3U;
            public const uint FILE_ATTRIBUTE_READONLY = 0x00000001U;
            public const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
            public const uint IOCTL_DISK_GET_DRIVE_LAYOUT_EX = 0x00070050U;

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool DeviceIoControl(
                SafeHandle hDevice,
                uint dwIoControlCode,
                IntPtr lpInBuffer,
                uint nInBufferSize,
                [Out] IntPtr lpOutBuffer,
                uint nOutBufferSize,
                out uint lpBytesReturned,
                IntPtr lpOverlapped);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern SafeFileHandle CreateFile(
               string lpFileName,
               uint dwDesiredAccess,
               uint dwShareMode,
               IntPtr SecurityAttributes,
               uint dwCreationDisposition,
               uint dwFlagsAndAttributes,
               IntPtr hTemplateFile);
        }
    }
}
