using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    public static class EfiPartition
    {
        public static Guid PartitionId
        {
            get
            {
                if (_PartitionIdentificator == Guid.Empty)
                    _PartitionIdentificator = GetPartitionIndentificator();

                return _PartitionIdentificator;
            }

            private set
            {
                _PartitionIdentificator = value;
            }
        }
        internal static Guid _PartitionIdentificator;

        public static Guid PartitionType
        {
            get => new Guid("C12A7328-F81F-11D2-BA4B-00A0C93EC93B");
        }

        public static Guid GetPartitionIndentificator()
        {
            if (_PartitionIdentificator != Guid.Empty)
                _PartitionIdentificator = InternalFinder.FindEfiPartitionInfo().Gpt.PartitionId;

            return _PartitionIdentificator;
        }

        public static string GetFullPath()
        {
            if (_PartitionIdentificator != Guid.Empty)
                _PartitionIdentificator = InternalFinder.FindEfiPartitionInfo().Gpt.PartitionId;

            return @"\\?\Volume{" + _PartitionIdentificator + @"}\";
        }

        public static DirectoryInfo GetDirectoryInfo()
        {
            if (_PartitionIdentificator != Guid.Empty)
                _PartitionIdentificator = InternalFinder.FindEfiPartitionInfo().Gpt.PartitionId;

            return new DirectoryInfo(@"\\?\Volume{" + _PartitionIdentificator + @"}\");
        }
    }
}
