using System;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware.Win32Native
{
    public class SaveAllocator : IDisposable
    {
        public enum AllocAs
        {
            Global,
            ComTask
        }

        public IntPtr Memory
        {
            get => _AllocatedMemory;
            private set => _AllocatedMemory = value;
        }
        private IntPtr _AllocatedMemory;

        public AllocAs Type
        {
            get => _AllocatedType;
            private set => _AllocatedType = value;
        }
        private AllocAs _AllocatedType;

        public bool IsInvalidOrEmpty
        {
            get => Memory == IntPtr.Zero | Memory == new IntPtr(-1);
        }

        public int AsInt32
        {
            get => Memory.ToInt32();
        }

        public long AsInt64
        {
            get => Memory.ToInt64();
        }

        private SaveAllocator(IntPtr Value, AllocAs AllocationType)
        {
            Memory = Value;
            Type = AllocationType;
        }

        ~SaveAllocator() => Dispose();

        public static SaveAllocator Global(IntPtr cb)
        {
            IntPtr Memory = Marshal.AllocHGlobal(cb);
            return new SaveAllocator(Memory, AllocAs.Global);
        }

        public static SaveAllocator Global(int cb)
        {
            IntPtr Memory = Marshal.AllocHGlobal(cb);
            return new SaveAllocator(Memory, AllocAs.Global);
        }

        public static SaveAllocator Global(uint cb)
        {
            IntPtr Memory = Marshal.AllocHGlobal((int)cb);
            return new SaveAllocator(Memory, AllocAs.Global);
        }

        public static SaveAllocator ComTask(IntPtr cb)
        {
            IntPtr Memory = Marshal.AllocCoTaskMem(cb.ToInt32());
            return new SaveAllocator(Memory, AllocAs.ComTask);
        }

        public static SaveAllocator ComTask(int cb)
        {
            IntPtr Memory = Marshal.AllocCoTaskMem(cb);
            return new SaveAllocator(Memory, AllocAs.ComTask);
        }

        public static SaveAllocator ComTask(uint cb)
        {
            IntPtr Memory = Marshal.AllocCoTaskMem((int)cb);
            return new SaveAllocator(Memory, AllocAs.ComTask);
        }

        public void Reallocate(IntPtr cb)
        {
            switch (Type)
            {
                case AllocAs.Global:
                    Memory = Marshal.ReAllocHGlobal(Memory, cb);
                    break;

                case AllocAs.ComTask:
                    Memory = Marshal.ReAllocCoTaskMem(Memory, cb.ToInt32());
                    break;
            }
        }

        public void Reallocate(int cb)
        {
            switch (Type)
            {
                case AllocAs.Global:
                    Memory = Marshal.ReAllocHGlobal(Memory, new IntPtr(cb));
                    break;

                case AllocAs.ComTask:
                    Memory = Marshal.ReAllocCoTaskMem(Memory, cb);
                    break;
            }
        }

        public static SaveAllocator Structure<TStruct>() where TStruct : struct
        {
            int Size = Marshal.SizeOf(typeof(TStruct));
            IntPtr Memory = Marshal.AllocHGlobal(Size);
            return new SaveAllocator(Memory, AllocAs.Global);
        }

        public void Free()
        {
            if (IsInvalidOrEmpty)
                return;

            switch (Type)
            {
                case AllocAs.Global:
                    Marshal.FreeHGlobal(Memory);
                    break;

                case AllocAs.ComTask:
                    Marshal.FreeCoTaskMem(Memory);
                    break;
            }
        }

        public void Dispose()
        {
            if (IsInvalidOrEmpty)
                return;

            GC.SuppressFinalize(this);
            Dispose(!IsInvalidOrEmpty);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Free();
            }
        }

        public static implicit operator IntPtr(SaveAllocator obj) => obj.Memory;
        public static implicit operator int(SaveAllocator obj) => obj.AsInt32;
        public static implicit operator long(SaveAllocator obj) => obj.AsInt64;
    }
}
