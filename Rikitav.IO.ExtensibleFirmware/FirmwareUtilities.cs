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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware
{
    /// <summary>
    /// Features for interacting with UEFI
    /// </summary>
    public static class FirmwareUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool CheckFirmwareAvailablity()
        {
            _ = NativeMethods.GetFirmwareEnvironmentVariableExW("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0, out _);
            return Marshal.GetLastWin32Error() != NativeMethods.ERROR_INVALID_FUNCTION;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="Value"></param>
        /// <param name="PtrSize"></param>
        public static void SetGlobalEnvironmentVariable(string VarName, IntPtr Value, int PtrSize)
            => SetEnvironmentVariable(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, Value, PtrSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="DataLength"></param>
        /// <param name="VarSize"></param>
        /// <returns></returns>
        public static IntPtr GetGlobalEnvironmentVariable(string VarName, out int DataLength, int VarSize = 4)
            => GetEnvironmentVariable(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, out DataLength, VarSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="attributes"></param>
        /// <param name="Value"></param>
        /// <param name="PtrSize"></param>
        public static void SetGlobalEnvironmentVariableEx(string VarName, VariableAttributes attributes, IntPtr Value, int PtrSize)
            => SetEnvironmentVariableEx(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, attributes, Value, PtrSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="attributes"></param>
        /// <param name="DataLength"></param>
        /// <param name="VarSize"></param>
        /// <returns></returns>
        public static IntPtr GetGlobalEnvironmentVariableEx(string VarName, out VariableAttributes attributes, out int DataLength, int VarSize = 4)
            => GetEnvironmentVariableEx(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, out attributes, out DataLength, VarSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="EnvironmentIdentificator"></param>
        /// <param name="Value"></param>
        /// <param name="PtrSize"></param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="FirmwareEnvironmentException"></exception>
        public static void SetEnvironmentVariable(string VarName, Guid EnvironmentIdentificator, IntPtr Value, int PtrSize)
        {
            if (!FirmwareInterface.Available)
                throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

            try
            {
                // Execution and error check
                using (new SystemEnvironmentPriviledge())
                {
                    if (!NativeMethods.SetFirmwareEnvironmentVariableW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", Value, PtrSize))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode);
                    }
                }
            }
            catch (Exception ex)
            {
                // Something wrong happened
                throw new FirmwareEnvironmentException("Failed to write environment variable", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="EnvironmentIdentificator"></param>
        /// <param name="DataLength"></param>
        /// <param name="VarSize"></param>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="FirmwareEnvironmentException"></exception>
        public static IntPtr GetEnvironmentVariable(string VarName, Guid EnvironmentIdentificator, out int DataLength, int VarSize = 4) // 4 - int size
        {
            if (!FirmwareInterface.Available)
                throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

            // Data
            IntPtr pointer = IntPtr.Zero;
            DataLength = 0;

            try
            {
                // Allocate variable pointer
                pointer = Marshal.AllocHGlobal(VarSize);

                // Reading variable
                using (new SystemEnvironmentPriviledge())
                    DataLength = (int)NativeMethods.GetFirmwareEnvironmentVariableW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", pointer, VarSize);

                // Error check
                if (DataLength == 0)
                {
                    Marshal.FreeHGlobal(pointer);
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }

                return pointer;
            }
            catch (Exception ex)
            {
                // Something wrong happened
                Marshal.FreeHGlobal(pointer);
                throw new FirmwareEnvironmentException("Failed to read environment variable", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="EnvironmentIdentificator"></param>
        /// <param name="attributes"></param>
        /// <param name="Value"></param>
        /// <param name="PtrSize"></param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="FirmwareEnvironmentException"></exception>
        public static void SetEnvironmentVariableEx(string VarName, Guid EnvironmentIdentificator, VariableAttributes attributes, IntPtr Value, int PtrSize)
        {
            if (!FirmwareInterface.Available)
                throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

            try
            {
                // Execution and error check
                using (new SystemEnvironmentPriviledge())
                {
                    if (!NativeMethods.SetFirmwareEnvironmentVariableExW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", Value, PtrSize, attributes))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode);
                    }
                }
            }
            catch (Exception ex)
            {
                // Something wrong happened
                throw new FirmwareEnvironmentException("Failed to write environment variable", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VarName"></param>
        /// <param name="EnvironmentIdentificator"></param>
        /// <param name="attributes"></param>
        /// <param name="DataLength"></param>
        /// <param name="VarSize"></param>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="FirmwareEnvironmentException"></exception>
        public static IntPtr GetEnvironmentVariableEx(string VarName, Guid EnvironmentIdentificator, out VariableAttributes attributes, out int DataLength, int VarSize = 4) // 4 - int size
        {
            // Data
            IntPtr pointer = IntPtr.Zero;
            DataLength = 0;

            if (!FirmwareInterface.Available)
                throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

            try
            {
                // Allocate variable pointer
                pointer = Marshal.AllocHGlobal(VarSize);

                // Reading variable
                using (new SystemEnvironmentPriviledge())
                    DataLength = (int)NativeMethods.GetFirmwareEnvironmentVariableExW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", pointer, VarSize, out attributes);

                // Error check
                if (DataLength == 0)
                {
                    Marshal.FreeHGlobal(pointer);
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }

                return pointer;
            }
            catch (Exception ex)
            {
                // Something wrong happened
                Marshal.FreeHGlobal(pointer);
                throw new FirmwareEnvironmentException("Failed to read environment variable", ex);
            }
        }

        private class SystemEnvironmentPriviledge : IDisposable
        {
            private IntPtr hToken = IntPtr.Zero;
            private NativeMethods.TokenPrivelege tp = new NativeMethods.TokenPrivelege()
            {
                Count = 1,
                Luid = 0,
                Attr = NativeMethods.SE_PRIVILEGE_ENABLED
            };

            public SystemEnvironmentPriviledge()
            {
                // Getting process token
                if (!NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, ref hToken))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process token");

                // Getting priviledge info
                if (!NativeMethods.LookupPrivilegeValue(IntPtr.Zero, NativeMethods.SE_SYSTEM_ENVIRONMENT_NAME, ref tp.Luid))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to lookup process privelage value");

                // Promoting process
                if (!NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust process token");
            }

            public void Dispose()
            {
                // Changing privilege state
                tp.Attr = 0;

                // Degrade process
                if (!NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust process token");

                // Freeing process handle
                NativeMethods.CloseHandle(hToken);
            }
        }

        private static class NativeMethods
        {
            public static readonly Guid _FirmwareGlobalEnvironmentIdentificator = new Guid("8BE4DF61-93CA-11D2-AA0D-00E098032B8C");
            public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
            public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
            public const int TOKEN_QUERY = 0x00000008;
            public const int SE_PRIVILEGE_ENABLED = 0x00000002;
            public const int ERROR_INVALID_FUNCTION = 1;

            public static bool Promoted = false;

            [DllImport("kernel32.dll", ExactSpelling = true)]
            public static extern IntPtr GetCurrentProcess();

            [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hObject);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool LookupPrivilegeValue(IntPtr host, string name, ref long pluid);

            [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokenPrivelege newst, int len, IntPtr prev, IntPtr prevlen);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint GetFirmwareEnvironmentVariableExW(
                [MarshalAs(UnmanagedType.LPWStr)] string lpName,
                [MarshalAs(UnmanagedType.LPWStr)] string lpGuid,
                IntPtr pBuffer,
                int nSize,
                out VariableAttributes Attributes);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetFirmwareEnvironmentVariableExW(
                [MarshalAs(UnmanagedType.LPWStr)] string lpName,
                [MarshalAs(UnmanagedType.LPWStr)] string lpGuid,
                IntPtr pValue,
                int nSize,
                VariableAttributes Attributes);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern uint GetFirmwareEnvironmentVariableW(
                [MarshalAs(UnmanagedType.LPWStr)] string lpName,
                [MarshalAs(UnmanagedType.LPWStr)] string lpGuid,
                IntPtr pBuffer,
                int nSize);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetFirmwareEnvironmentVariableW(
                [MarshalAs(UnmanagedType.LPWStr)] string lpName,
                [MarshalAs(UnmanagedType.LPWStr)] string lpGuid,
                IntPtr pValue,
                int nSize);

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct TokenPrivelege
            {
                public int Count;
                public long Luid;
                public int Attr;
            }
        }
    }

    /// <summary>
    /// Represents an error that occurred while working with UEFI
    /// </summary>
    public class FirmwareEnvironmentException : Exception
    {
        /// <inheritdoc/>
        public FirmwareEnvironmentException(string Message)
            : base(Message) { }

        /// <inheritdoc/>
        public FirmwareEnvironmentException(string Message, Exception inner)
            : base(Message, inner) { }
    }
}
