using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware
{
    public class SaveAllocate
    {
        public static void Global(int cb, Action<IntPtr> func)
        {
            IntPtr Memory = Marshal.AllocHGlobal(cb);
            try
            {
                func(Memory);
            }
            finally
            {
                Marshal.FreeHGlobal(Memory);
            }
        }

        public static TReturn Global<TReturn>(int cb, Func<IntPtr, TReturn> func)
        {
            IntPtr Memory = Marshal.AllocHGlobal(cb);
            try
            {
                return func(Memory);
            }
            finally
            {
                Marshal.FreeHGlobal(Memory);
            }
        }
    }
}
