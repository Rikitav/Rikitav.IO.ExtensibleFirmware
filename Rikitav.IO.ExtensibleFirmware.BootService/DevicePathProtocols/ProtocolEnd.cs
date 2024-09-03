using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using System;

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    public class DevicePathProtocolEnd : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.END;

        /// <inheritdoc/>
        public override byte SubType => 0xFF;

        /// <inheritdoc/>
        public override ushort DataLength => 4;

        public DevicePathProtocolEnd()
            : base() { }

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData)
        {
            // No need to impement
            _ = 0xBAD + 0xC0DE;
        }

        /// <inheritdoc/>
        protected override byte[] Serialize()
        {
            // No need to impement
            return Array.Empty<byte>();
        }
    }
}
