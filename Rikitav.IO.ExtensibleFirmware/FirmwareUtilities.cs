using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware
{
    public static class FirmwareUtilities
    {
        public static bool CheckInterfaceAvailablity()
        {
            _ = NativeMethods.GetFirmwareEnvironmentVariableExW("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0, out _);
            int lastError = Marshal.GetLastWin32Error();
            return lastError != NativeMethods.ERROR_INVALID_FUNCTION;
        }

        public static void SetGlobalEnvironmentVariable(string VarName, IntPtr Value, int PtrSize)
            => SetEnvironmentVariable(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, Value, PtrSize);

        public static IntPtr GetGlobalEnvironmentVariable(string VarName, out int DataLength, int VarSize = 4)
            => GetEnvironmentVariable(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, out DataLength, VarSize);

        public static void SetGlobalEnvironmentVariableEx(string VarName, VariableAttributes attributes, IntPtr Value, int PtrSize)
            => SetEnvironmentVariableEx(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, attributes, Value, PtrSize);

        public static IntPtr GetGlobalEnvironmentVariableEx(string VarName, out VariableAttributes attributes, out int DataLength, int VarSize = 4)
            => GetEnvironmentVariableEx(VarName, NativeMethods._FirmwareGlobalEnvironmentIdentificator, out attributes, out DataLength, VarSize);

        public static void SetEnvironmentVariable(string VarName, Guid EnvironmentIdentificator, IntPtr Value, int PtrSize)
        {
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

        public static IntPtr GetEnvironmentVariable(string VarName, Guid EnvironmentIdentificator, out int DataLength, int VarSize = 4) // 4 - int size
        {
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

        public static void SetEnvironmentVariableEx(string VarName, Guid EnvironmentIdentificator, VariableAttributes attributes, IntPtr Value, int PtrSize)
        {
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

        public static IntPtr GetEnvironmentVariableEx(string VarName, Guid EnvironmentIdentificator, out VariableAttributes attributes, out int DataLength, int VarSize = 4) // 4 - int size
        {
            // Data
            IntPtr pointer = IntPtr.Zero;
            DataLength = 0;

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

        /*
        private static void PromoteProcessSystemEnvironmentPrivilege()
        {
            if (NativeMethods.Promoted)
                return;

            IntPtr hToken = IntPtr.Zero;
            if (!NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, ref hToken))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open process token");

            NativeMethods.TokenPrivelege tp = new NativeMethods.TokenPrivelege()
            {
                Count = 1,
                Luid = 0,
                Attr = NativeMethods.SE_PRIVILEGE_ENABLED
            };

            if (!NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_SYSTEM_ENVIRONMENT_NAME, ref tp.Luid))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to lookup process privelage value");

            if (!NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust process token");

            NativeMethods.Promoted = true;
            NativeMethods.CloseHandle(hToken);
        }
        */

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
                if (!NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_SYSTEM_ENVIRONMENT_NAME, ref tp.Luid))
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
            public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

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

    [Flags]
    public enum VariableAttributes : int
    {
        NON_VOLATILE = 0x00000001,
        BOOTSERVICE_ACCESS = 0x00000002,
        RUNTIME_ACCESS = 0x00000004,
        HARDWARE_ERROR_RECORD = 0x00000008,
        AUTHENTICATED_WRITE_ACCESS = 0x00000010,
        TIME_BASED_AUTHENTICATED_WRITE_ACCESS = 0x00000020,
        APPEND_WRITE = 0x00000040,
        ENHANCED_AUTHENTICATED_ACCESS = 0x00000080
    }

    public class FirmwareEnvironmentException : Win32Exception
    {
        public FirmwareEnvironmentException(string Message)
            : base(Message) { }

        public FirmwareEnvironmentException(string Message, Exception inner)
            : base(Message, inner) { }

        public FirmwareEnvironmentException(string Message, int ErrorCode)
            : base(ErrorCode, Message) { }
    }
}
