using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.BootService.LoadOption
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LoadOptionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public const VariableAttributes LoadOptionVariableAttributes = VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS;

        /// <summary>
        /// 
        /// </summary>
        public LoadOptionAttributes Attributes { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DevicePathProtocolBase[] OptionProtocols { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int StructureLength
        {
            get => OptionProtocols.Sum(x => x.DataLength) // Protocols data
                + ((Description.Length + 1) * 2)                // Description string
                + _OptionalData.Length                    // Optional data
                + 2                                       // FilePathListLength
                + 4;                                      // Attributes
        }

        /// <summary>
        /// 
        /// </summary>
        protected byte[] _OptionalData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="description"></param>
        protected LoadOptionBase(LoadOptionAttributes attributes, string description)
        {
            Attributes = attributes;
            Description = description;
            OptionProtocols = Array.Empty<DevicePathProtocolBase>();
            _OptionalData = Array.Empty<byte>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadOption"></param>
        /// <exception cref="DeviceProtocolCastingException"></exception>
        protected LoadOptionBase(EFI_LOAD_OPTION loadOption)
        {
            Attributes = loadOption.Attributes;
            Description = loadOption.Description;
            _OptionalData = loadOption.OptionalData;

            OptionProtocols = new DevicePathProtocolBase[loadOption.FilePathList.Length];
            for (int i = 0; i < loadOption.FilePathList.Length; i++)
            {
                Type? protocolWrapperType = DevicePathProtocolWrapperSelector.Instance.GetRegisteredType(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType);
                OptionProtocols[i] = protocolWrapperType == null
                    ? new RawMediaDevicePath(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType)
                    : DevicePathProtocolBase.CreateProtocol(protocolWrapperType, loadOption.FilePathList[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal EFI_LOAD_OPTION ToLoadOption()
        {
            EFI_DEVICE_PATH_PROTOCOL[] FilePathList = OptionProtocols.Select(x => x.ToEfiProtocol()).ToArray();
            return new EFI_LOAD_OPTION()
            {
                Attributes = Attributes,
                FilePathListLength = (ushort)FilePathList.Sum(x => x.Length),
                Description = Description,
                FilePathList = FilePathList,
                OptionalData = _OptionalData
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        internal void MarshalToBinaryWriter(BinaryWriter writer)
        {
            // Writing attributes field
            writer.Write((int)Attributes);

            // Writing length of filepath list
            writer.Write((ushort)OptionProtocols.Sum(p => p.DataLength));

            // Writing option description
            writer.Write(Encoding.Unicode.GetBytes(Description + "\0"));

            // Writing filepath list
            foreach (DevicePathProtocolBase edpp in OptionProtocols)
                edpp.MarshalToBinaryWriter(writer);

            // Writing optional data
            writer.Write(_OptionalData);
        }

        internal void MarshalFromBinaryReader(BinaryReader reader)
        {

        }

        public object CopyTo(LoadOptionBase optionCopy)
        {
            optionCopy.Attributes = Attributes;
            optionCopy.Description = Description;
            
            optionCopy.OptionProtocols = new DevicePathProtocolBase[OptionProtocols.Length];
            OptionProtocols.CopyTo(optionCopy.OptionProtocols, 0);

            optionCopy._OptionalData = _OptionalData;
            return optionCopy;
        }

        public void OverrideOptionalData(byte[] data)
        {
            _OptionalData = data;
        }
    }
}
