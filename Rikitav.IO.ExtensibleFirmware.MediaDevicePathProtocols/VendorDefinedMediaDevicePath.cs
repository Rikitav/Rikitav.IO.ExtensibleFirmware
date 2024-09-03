using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// Vendor-Defined Media Device Path
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#vendor-defined-media-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.MEDIA, 3, typeof(VendorDefinedMediaDevicePath))]
    public class VendorDefinedMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.MEDIA;

        /// <inheritdoc/>
        public override byte SubType => 3;

        /// <inheritdoc/>
        public override ushort DataLength => (ushort)(4 + 16 + VendorDefinedData.Length);

        /// <summary>
        /// Vendor-assigned GUID that defines the data that follows
        /// </summary>
        public Guid VendorGuid { get; set; }

        /// <summary>
        /// Vendor-defined variable size data.
        /// </summary>
        public byte[] VendorDefinedData { get; set; }

        public VendorDefinedMediaDevicePath()
            : base() => VendorDefinedData = Array.Empty<byte>();

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData)
        {
            using BinaryReader reader = new BinaryReader(new MemoryStream(protocolData));

            VendorGuid = new Guid(reader.ReadBytes(16));
            VendorDefinedData = reader.ReadBytes(protocolData.Length - 16);
        }

        /// <inheritdoc/>
        protected override byte[] Serialize()
        {
            using MemoryStream protocolData = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(protocolData);

            writer.Write(VendorGuid.ToByteArray());
            writer.Write(VendorDefinedData);

            return protocolData.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString() => VendorGuid.ToString();
    }
}
