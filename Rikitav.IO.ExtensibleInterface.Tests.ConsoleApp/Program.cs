using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        //FirmwareGlobalEnvironment.BootOrder = FirmwareGlobalEnvironment.BootOrder.Where(x => x != 0).ToArray();
        //Console.WriteLine(string.Join("; ", FirmwareGlobalEnvironment.BootOrder));

        //WriteLoadOption(0);

        /*
        List<ushort> newOrder = FirmwareGlobalEnvironment.BootOrder.ToList();
        newOrder.Insert(0, 0);
        FirmwareGlobalEnvironment.BootOrder = newOrder.ToArray();
        //*/

        /*
        FirmwareApplicationBootOption? currentBootOption = FirmwareBootService.ReadLoadOption<FirmwareApplicationBootOption>("Boot0000");
        byte[] WinLoadOptionData = currentBootOption.ToByteArray();
        EFI_LOAD_OPTION WinLoadOptionStruct = currentBootOption.ToLoadOption();

        FirmwareApplicationBootOption firmwareApplication = new FirmwareApplicationBootOption("TestingRefind", "\\EFI\\refind\\refind_x64.efi", EfiPartition.Identificator);
        byte[] firmwareApplicationData = firmwareApplication.ToByteArray();
        EFI_LOAD_OPTION firmwareApplicationStruct = firmwareApplication.ToLoadOption();

        string newLoadOptionName = FirmwareBootService.WriteLoadOption(firmwareApplication);
        WriteLoadOption(newLoadOptionName);
        return;
        //*/

        /*
        //FirmwareBootOption ogBootOption = FirmwareBootService.ReadLoadOption(0);
        FirmwareApplicationBootOption newAppliactionBootOption = new FirmwareApplicationBootOption("Refind", EfiPartition.Identificator, @"\EFI\refind\refind_x64.efi");
        ushort newLoadOptionIndex = FirmwareBootService.CreateLoadOption(newAppliactionBootOption);
        WriteLoadOption(0);
        //*/

        //*
        foreach (ushort bootOptionName in FirmwareBootService.LoadOrder)
        {
            //Console.WriteLine(bootOptionName);
            WriteLoadOption(bootOptionName);
        }
        //*/
    }

    public static void WriteLoadOption(ushort bootOptionindex)
    {
        FirmwareBootOption? BootOption = FirmwareBootService.ReadLoadOption(bootOptionindex);

        Console.WriteLine();
        Console.WriteLine("BootOption name - Boot" + bootOptionindex.ToString("X").PadLeft(4, '0'));
        Console.WriteLine("BootOption desc - " + BootOption.Description);
        Console.WriteLine("BootOption protocols count - " + BootOption.OptionProtocols.Length);
        Console.WriteLine("BootOption optional data - \"" + Encoding.Unicode.GetString(BootOption.OptionalData) + "\"");

        Console.WriteLine("{");
        foreach (DevicePathProtocolBase protocol in BootOption.OptionProtocols)
        {
            if (protocol is RawMediaDevicePath rawProtocol)
                Console.WriteLine("\tRawMediaDevicePath - byte[{0}] t - {1} st - {2}", rawProtocol.ProtocolData.Length, rawProtocol.Type, rawProtocol.SubType);
            else
                Console.WriteLine("\t{0} - {1}", protocol.GetType().Name, protocol.ToString());
        }
        Console.WriteLine("}");
    }
}

public class FirmwareApplicationBootOption : LoadOptionBase
{
    public HardDriveMediaDevicePath HardDriveProtocol => (HardDriveMediaDevicePath)OptionProtocols[0];
    public FilePathMediaDevicePath FilePathProtocol => (FilePathMediaDevicePath)OptionProtocols[1];

    public FirmwareApplicationBootOption(string description, Guid Partition, string FileName) : base(LoadOptionAttributes.ACTIVE, description)
    {
        Description = description;
        OptionProtocols = new DevicePathProtocolBase[]
        {
            new HardDriveMediaDevicePath(Partition),
            new FilePathMediaDevicePath(FileName),
            new DevicePathProtocolEnd()
        };
    }

    public FirmwareApplicationBootOption(LoadOptionBase optionBase) : base(0, string.Empty)
    {
        if (optionBase.OptionProtocols[0] is not HardDriveMediaDevicePath)
            throw new ArgumentException("Option is not firmware application");

        if (optionBase.OptionProtocols[1] is not FilePathMediaDevicePath)
            throw new ArgumentException("Option is not firmware application");

        if (optionBase.OptionProtocols[2] is not DevicePathProtocolEnd)
            throw new ArgumentException("Option is not firmware application");

        optionBase.CopyTo(this);
    }
}