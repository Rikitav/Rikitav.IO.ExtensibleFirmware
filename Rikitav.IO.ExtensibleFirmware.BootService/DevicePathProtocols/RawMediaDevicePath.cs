using System;

namespace Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols
{
    public class RawMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => _Type;
        private readonly DeviceProtocolType _Type = 0;

        /// <inheritdoc/>
        public override byte SubType => _SubType;
        private readonly byte _SubType = 0;

        /// <inheritdoc/>
        public override ushort DataLength => (ushort)(ProtocolData.Length + 4);

        /// <summary>
        /// 
        /// </summary>
        public byte[] ProtocolData { get; private set; }

        public RawMediaDevicePath(DeviceProtocolType type, byte subType) : base()
        {
            ProtocolData = Array.Empty<byte>();
            _Type = type;
            _SubType = subType;
        }

        /*
        public RawMediaDevicePath(byte[]? protocolData)
            : base() => ProtocolData = protocolData ?? Array.Empty<byte>();
        */

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData) => ProtocolData = protocolData;

        /// <inheritdoc/>
        protected override byte[] Serialize() => ProtocolData;
    }
}
