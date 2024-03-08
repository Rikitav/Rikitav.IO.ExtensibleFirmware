using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    public class EfiExecutableInfo
    {
        public string Application
        {
            get => Path.GetDirectoryName(_FullPath);
        }

        public string Name
        {
            get => Path.GetFileNameWithoutExtension(_FullPath);
        }

        public string FullName
        {
            get => _FullPath;
            private set => _FullPath = value;
        }
        private string _FullPath;

        public bool Exists
        {
            get => File.Exists(_FullPath);
        }

        public ProcessorArchitecture Architecture
        {
            get
            {
                byte[] data = File.ReadAllBytes(_FullPath);
                return (ProcessorArchitecture)BitConverter.ToUInt16(data, BitConverter.ToInt32(data, 0x3c) + 4);
            }
        }

        public EfiExecutableInfo(string RelativePath)
        {
            string ESP = EfiPartition.GetFullPath();
            string TmpFullPath = Path.Combine(ESP, "EFI", RelativePath);

            string TmpExt = Path.GetExtension(TmpFullPath);
            if (string.IsNullOrEmpty(TmpExt) || TmpExt != "efi")
                throw new InvalidDataException("File extension does not match target type");

            _FullPath = TmpFullPath;
            //InitInfoFields();
        }

        public EfiExecutableInfo(string ApplicationName, string RelativePath)
        {
            string ESP = EfiPartition.GetFullPath();
            string TmpFullPath = Path.Combine(ESP, "EFI", ApplicationName, RelativePath);

            string TmpExt = Path.GetExtension(TmpFullPath);
            if (string.IsNullOrEmpty(TmpExt) || TmpExt != "efi")
                throw new InvalidDataException("File extension does not match target type");

            _FullPath = TmpFullPath;
            //InitInfoFields();
        }

        /*
        internal void InitInfoFields()
        {

        }
        */
    }
}
