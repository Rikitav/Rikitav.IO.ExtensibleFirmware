using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// The CD-ROM Media Device Path is used to define a system partition that exists on a CD-ROM. The CD-ROM is assumed to contain an ISO-9660 file system and follow the CD-ROM “El Torito” format
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#cd-rom-media-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.MEDIA, 2, typeof(CdRomMediaDevicePath))]
    public sealed class CdRomMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.MEDIA;

        /// <inheritdoc/>
        public override byte SubType => 2;

        /// <inheritdoc/>
        public override ushort DataLength => 24;

        /// <summary>
        /// Boot Entry number from the Boot Catalog. The Initial/Default entry is defined as zero.
        /// </summary>
        public uint BootEntry { get; set; }

        /// <summary>
        /// Starting RBA of the partition on the medium. CD-ROMs use Relative logical Block Addressing.
        /// </summary>
        public ulong PartitionStart { get; set; }

        /// <summary>
        /// Size of the partition in units of Blocks, also called Sectors.
        /// </summary>
        public ulong PartitionSize { get; set; }

        public CdRomMediaDevicePath()
            : base() { }

        public CdRomMediaDevicePath(uint bootEntry, ulong partitionStart, ulong partitionSize) : base()
        {
            BootEntry = bootEntry;
            PartitionStart = partitionStart;
            PartitionSize = partitionSize;
        }

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData)
        {
            using BinaryReader reader = new BinaryReader(new MemoryStream(protocolData));

            BootEntry = reader.ReadUInt32();
            PartitionStart = reader.ReadUInt64() * 512;
            PartitionSize = reader.ReadUInt64() * 512;
        }

        /// <inheritdoc/>
        protected override byte[] Serialize()
        {
            using MemoryStream protocolData = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(protocolData);

            writer.Write(BootEntry);
            writer.Write(PartitionStart / 512);
            writer.Write(PartitionSize / 512);

            return protocolData.ToArray();
        }

        /// <inheritdoc/>
        public override string ToString() => BootEntry.ToString();
    }
}
