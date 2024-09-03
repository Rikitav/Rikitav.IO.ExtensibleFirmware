using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Rikitav.IO.ExtensibleFirmware.BootService.Win32Native
{
    internal class MemoryPointerStream : Stream, IDisposable
    {
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _BufferLength;
        public override long Position
        {
            get => _CurPos;
            set => _CurPos = value;
        }

        private readonly bool _LeaveOpen;
        private readonly long _BufferLength;
        private readonly IntPtr _Buffer;
        private long _CurPos;

        public MemoryPointerStream(IntPtr buffer, long bufferLength, bool leaveOpen)
        {
            _Buffer = buffer;
            _BufferLength = bufferLength;
            _LeaveOpen = leaveOpen;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_CurPos + count > _BufferLength)
                return 0;

            for (int i = offset; i < count; i++)
                buffer[i] = Marshal.ReadByte(_Buffer, (int)_CurPos++);

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_CurPos + count > _BufferLength)
                throw new ArgumentOutOfRangeException();

            for (int i = offset; i < count; i++)
                Marshal.WriteByte(_Buffer, (int)_CurPos++, buffer[i]);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        if (offset > _BufferLength)
                            throw new ArgumentOutOfRangeException();

                        _CurPos = offset;
                        break;
                    }

                case SeekOrigin.Current:
                    {
                        if (_CurPos + offset > _BufferLength)
                            throw new ArgumentOutOfRangeException();

                        _CurPos += offset;
                        break;
                    }

                case SeekOrigin.End:
                    {
                        if (offset > _BufferLength)
                            throw new ArgumentOutOfRangeException();

                        _CurPos = _BufferLength - offset;
                        break;
                    }
            }

            return _CurPos;
        }

        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Flush() => _ = 0xBAD + 0xC0DE;

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_LeaveOpen)
                Marshal.FreeHGlobal(_Buffer);

            base.Dispose(disposing);
        }
    }
}
