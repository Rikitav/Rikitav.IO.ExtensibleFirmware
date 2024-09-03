using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols
{
    /// <summary>
    /// File Path Media Device Path
    /// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#file-path-media-device-path
    /// </summary>
    [DefineDevicePathProtocol(DeviceProtocolType.MEDIA, 4, typeof(FilePathMediaDevicePath))]
    public sealed class FilePathMediaDevicePath : DevicePathProtocolBase
    {
        /// <inheritdoc/>
        public override DeviceProtocolType Type => DeviceProtocolType.MEDIA;

        /// <inheritdoc/>
        public override byte SubType => 4;

        /// <inheritdoc/>
        public override ushort DataLength => (ushort)((PathName.Length + 1) * 2 + 4);

        /// <summary>
        /// A NULL-terminated Path string including directory and file name.
        /// </summary>
        public string PathName { get; set; } = string.Empty;

        public FilePathMediaDevicePath()
            : base() { }

        public FilePathMediaDevicePath(string pathName)
            : base() => PathName = pathName;

        /// <inheritdoc/>
        protected override void Deserialize(byte[] protocolData) => PathName = Encoding.Unicode.GetString(protocolData).TrimEnd('\0');

        /// <inheritdoc/>
        protected override byte[] Serialize() => Encoding.Unicode.GetBytes(PathName + '\0');

        /// <inheritdoc/>
        public override string ToString() => PathName;
    }
}
