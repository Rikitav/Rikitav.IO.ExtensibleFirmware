# ExtensibleFirmware
![Markdown](https://github.com/Rikitav/Rikitav.IO.ExtensibleFirmware/blob/main/icon.png)

## Description
ExtensibleFirmware is a .NET class library for working with UEFI firmware. It provides tools for editing the boot process, allowing developers to modify and configure boot parameters for enhanced flexibility.

## Features
- Getting EFI system partition
- Editing and creating boot entries.
- Editing global variables for firmware.

## Installation
```bash
# Install via NuGet
Install-Package ExtensibleFirmware
```

## Usage Examples

### Example 1: Getting ESP
```csharp
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.IO;

namespace Examples
{
    class Program
    {
        public static void Main()
        {
            // Getting 'EFI system partition' volume path
            string EspVolumePath = EfiPartition.GetFullPath();

            // Example reading config file from ESP using volume path, instead of using MountVol
            string configText = File.ReadAllText(Path.Combine(EspVolumePath, "EFI", "Ubuntu", "grub.cfg"));
        }
    }
}
```

### Example 2: Reading boot option
```csharp
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using System;

namespace Examples
{
    class Program
    {
        public static void Main()
        {
            // Reading boot option
            FirmwareBootOption bootOption = FirmwareBootService.ReadLoadOption(0x0003); // <-- Set here your variable index
            
            // Enumerating all protocols
            foreach (DevicePathProtocolBase protocol in bootOption.OptionProtocols)
                Console.WriteLine(protocol.ToString()); // Getting string representation of protocol
        }
    }
}
```

### Example 3: Creating boot option
```csharp
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using System;

namespace Examples
{
    class Program
    {
        public static void Main()
        {
            // Setting device path protocols
            // This protocols instructions boot service how to load your option
            DevicePathProtocolBase[] protocols = new DevicePathProtocolBase[]
            {
                new HardDriveMediaDevicePath(new Guid("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX")), // The partition on which the bootloader is located
                new FilePathMediaDevicePath("EFI\\MyApplication\\bootx64.efi"), // Path to the EFI application to be loaded
                new DevicePathProtocolEnd() // Indicates the end of the boot option. if this is omitted, the option will not be considered valid
            };

            // Creating simple load option
            FirmwareBootOption bootOption = new FirmwareBootOption(LoadOptionAttributes.ACTIVE, "MyLoader", Array.Empty<byte>(), protocols);

            // Creating new load option
            ushort newLoadOptionIndex = FirmwareBootService.CreateLoadOption(bootOption, true);

            // Logging new boot option index
            Console.WriteLine("Boot option sucessfully created, new option index : {0}", newLoadOptionIndex);
        }
    }
}
```

## License
This project is licensed under the GNU GPL License. See [LICENSE](LICENSE) for details.
