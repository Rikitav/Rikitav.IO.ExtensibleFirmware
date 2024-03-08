using System.Diagnostics;

namespace Rikitav.IO.ExtensibleFirmware.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_FindEfiPartition()
        {
            DirectoryInfo EfiDir = FirmwareInterface.GetSystemPartition();
            Assert.IsTrue(EfiDir.GetDirectories().Select(dir => dir.Name).Contains("EFI"));
        }
    }
}