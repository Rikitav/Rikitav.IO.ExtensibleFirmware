using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.Win32Native
{
    internal partial class InteropInterface
    {
        internal static IntPtr GetPartitionHandle(string DevicePath)
        {
            return NativeMethods.CreateFile(
                DevicePath, // Partition DevicePath
                0, FileShare.Read,
                IntPtr.Zero, FileMode.Open,
                0, IntPtr.Zero);
        }

        internal static TOutBuffer GetIoDeviceData<TOutBuffer>(IntPtr hDevice, uint dwIoControlCode) where TOutBuffer : struct
        {
            using (SaveAllocator StructPtr = SaveAllocator.Structure<TOutBuffer>())
            {
                // Copying structure to allocated memory
                Marshal.StructureToPtr(new TOutBuffer(), StructPtr, true);
                uint lpBytesReturned = 0;

                // Executing DeviceIoControl()
                bool bResult = NativeMethods.DeviceIoControl(
                    hDevice, dwIoControlCode,
                    IntPtr.Zero, 0,
                    StructPtr, (uint)Marshal.SizeOf<TOutBuffer>(),
                    ref lpBytesReturned, IntPtr.Zero);

                // Creating new struct from allocated byte buffer
                return Marshal.PtrToStructure<TOutBuffer>(StructPtr);
            }
        }

        public static void CloseHandle(IntPtr Handle)
        {
            NativeMethods.CloseHandle(Handle);
        }

        private static class NativeMethods
        {
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
        }
    }
}
