using Rikitav.IO.ExtensibleFirmware.SystemPartition;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(EfiPartition.GetFullPath());
        Console.WriteLine(new EfiExecutableInfo("refind", "refind_x64.efi").Architecture);
    }
}