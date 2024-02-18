using System.Diagnostics;

namespace Rikitav.IO.ExtensibleFirmware.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_PartitionEnumerable()
        {
            foreach (DirectoryInfo Partition in new PartitionEnumerable())
            {
                Debug.WriteLine(string.Format("\n==({0}){1}", Partition.Name, new string('=', 10)));
                Assert.IsTrue(Directory.Exists(Partition.FullName));
            }
        }

        [TestMethod]
        public void Test_FindEfiPartition()
        {
            DirectoryInfo EfiDir = FirmwareInterface.GetSystemPartition();
            Assert.IsTrue(EfiDir.GetDirectories().Select(dir => dir.Name).Contains("EFI"));
        }
    }
}