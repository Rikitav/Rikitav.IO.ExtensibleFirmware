using Rikitav.IO.ExtensibleFirmware.Win32Native;
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    public static class EfiPartition
    {
        public static Guid Identificator
        {
            get
            {
                if (_PartitionIdentificator == Guid.Empty)
                    _PartitionIdentificator = PartitionInfo.Gpt.PartitionId;

                return _PartitionIdentificator;
            }

            private set
            {
                _PartitionIdentificator = value;
            }
        }
        private static Guid _PartitionIdentificator = Guid.Empty;

        internal static PARTITION_INFORMATION_EX PartitionInfo
        {
            get
            {
                if (_PartitionInfo is null)
                    _PartitionInfo = InternalFinder.FindEfiPartitionInfo();

                return _PartitionInfo.Value;
            }

            private set
            {
                _PartitionInfo = value;
            }
        }
        private static PARTITION_INFORMATION_EX? _PartitionInfo;

        public static Guid TypeID
        {
            get => new Guid("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");
        }

        public static string GetFullPath()
            => @"\\?\Volume{" + Identificator + @"}\";

        public static DirectoryInfo GetDirectoryInfo()
            => new DirectoryInfo(GetFullPath());
    }
}
