using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.BootService.LoadOption
{
    /// <summary>
    /// 
    /// </summary>
    public class FirmwareBootOption : LoadOptionBase
    {
        public byte[] OptionalData => _OptionalData;

        internal FirmwareBootOption(EFI_LOAD_OPTION loadOption)
            : base(loadOption) { }

        public FirmwareBootOption(byte[] loadOptionVarData)
            : base(LoadOptionMarshaller.BinaryReaderToStructure(new BinaryReader(new MemoryStream(loadOptionVarData)))) { }

        public FirmwareBootOption(LoadOptionAttributes attributes, string description, byte[] optionalData, DevicePathProtocolBase[] protocols) : base(attributes, description)
        {
            _OptionalData = optionalData;
            OptionProtocols = protocols;
        }
    }
}
