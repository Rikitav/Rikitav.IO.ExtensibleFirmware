using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware
{
    public class PartitionEnumerable : IEnumerable<DirectoryInfo>
    {
        public IEnumerator<DirectoryInfo> GetEnumerator() => new PartitionEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => new PartitionEnumerator();
    }

    public class PartitionEnumerator : IEnumerator<DirectoryInfo>
    {
        object IEnumerator.Current => FormatValue();
        DirectoryInfo IEnumerator<DirectoryInfo>.Current => FormatValue();

        private IntPtr EnumerationHandle = IntPtr.Zero;
        private StringBuilder CurrentVolumeName = null;

        public PartitionEnumerator()
        {
            Reset();
        }

        private DirectoryInfo FormatValue()
        {
            return new DirectoryInfo(CurrentVolumeName.ToString());
        }

        private void CheckHandle()
        {
            if (EnumerationHandle == NativeMethods.INVALID_HANDLE_VALUE)
                throw new InvalidEnumerableHandleException("Volume", Marshal.GetLastWin32Error());
        }

        public bool MoveNext()
        {
            if (!NativeMethods.FindNextVolume(EnumerationHandle, CurrentVolumeName, (uint)CurrentVolumeName.Capacity))
            {
                CheckHandle();
                return false;
            }

            return true;
        }

        public void Reset()
        {
            InternalDispose();
            CurrentVolumeName = new StringBuilder(1024);
            EnumerationHandle = NativeMethods.FindFirstVolume(CurrentVolumeName, (uint)CurrentVolumeName.Capacity);
            CheckHandle();
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            InternalDispose();
            IsDisposed = true;
        }

        private void InternalDispose()
        {
            if (EnumerationHandle != IntPtr.Zero)
                NativeMethods.FindVolumeClose(EnumerationHandle);

            if (CurrentVolumeName != null)
                CurrentVolumeName = null;
        }

        internal static class NativeMethods
        {
            public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            #region Device enumeration
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr FindFirstVolume(
            [Out] StringBuilder lpszVolumeName,
            uint cchBufferLength);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool FindNextVolume(
                IntPtr hFindVolume,
                [Out] StringBuilder lpszVolumeName,
                uint cchBufferLength);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool FindVolumeClose(
                IntPtr hFindVolume);
            #endregion
        }
    }

    public class InvalidEnumerableHandleException : Exception
    {
        public int ErrorCode
        {
            get;
            private set;
        }

        public InvalidEnumerableHandleException(string EnumerableType, int errorCode)
            : base(string.Format("WinApi {0}Enumerable handle was invalid", EnumerableType))
        {
            this.ErrorCode = errorCode;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PARTITION_INFORMATION_EX
    {
        public int PartitionStyle;
        public long StartingOffset;
        public long PartitionLength;
        public int PartitionNumber;
        public bool RewritePartition;
        public PARTITION_INFORMATION_GPT Gpt;
        public IntPtr Mbr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PARTITION_INFORMATION_GPT
    {
        public Guid PartitionType;
        public Guid PartitionId;
        [MarshalAs(UnmanagedType.U8)]
        public ulong Attributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string Name;
    }
}
