using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using System;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// The Media Protocol Device Path is used to denote the protocol that is being used in a device path at the location of the path specified. Many protocols are inherent to the style of device path.
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#media-protocol-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.MEDIA, 5, typeof(MediaProtocolMediaDevicePath))]
    public sealed class MediaProtocolMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.MEDIA;

        /// <inheritdoc/>
        public override byte SubType => 5;

        /// <inheritdoc/>
        public override ushort DataLength => 20;

        /// <summary>
        /// The ID of the protocol
        /// </summary>
        public Guid ProtocolGUID { get; set; }

        public MediaProtocolMediaDevicePath()
            : base() { }

        public MediaProtocolMediaDevicePath(Guid protocolGUID)
            : base() => ProtocolGUID = protocolGUID;

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData) => ProtocolGUID = new Guid(protocolData);

        /// <inheritdoc/>
        protected override byte[] Serialize() => ProtocolGUID.ToByteArray();

        /// <inheritdoc/>
        public override string ToString() => ProtocolGUID.ToString();
    }
}
