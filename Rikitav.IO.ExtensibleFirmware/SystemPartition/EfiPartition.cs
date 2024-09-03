using Rikitav.IO.ExtensibleFirmware.Win32Native;
using System;
using System.IO;

namespace Rikitav.IO.ExtensibleFirmware.SystemPartition
{
    public static class EfiPartition
    {
        public static Guid Identificator
        {
            get => _PartitionIdentificator ??= PartitionInfo.Gpt.PartitionId;
            private set => _PartitionIdentificator = value;
        }
        private static Guid? _PartitionIdentificator = null;

        internal static PARTITION_INFORMATION_EX PartitionInfo
        {
            get => _PartitionInfo ??= InternalEfiSystemPartitionFinder.FindEfiPartitionInfo();
            private set => _PartitionInfo = value;
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

        public static PARTITION_INFORMATION_EX GetPartitionInfo()
            => PartitionInfo;
    }
}
