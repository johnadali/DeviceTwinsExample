using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace DeviceTwinsBackend
{
    class Program
    {
        static RegistryManager registryManager;
        static string connectionString = "<<enter your IoT Hub connection string>>";
        static readonly string MXCHIP_DEVICENAME = "<<your device ID>>";

        public static async Task ReadTags()
        {
            var twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            // Read and display Tags
            Console.WriteLine("Tags for {0} device: {1}\n", twin.DeviceId, twin.Tags);

        }

        public static async Task UpdateTags(string strTagName, string strTagValue)
        {
            var twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            // Write Tags
            var patch = $@"{{ tags: {{{strTagName}: {strTagValue}}} }}";


            await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);

            twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            // Read and display Tags
            Console.WriteLine("Tags for {0} device: {1}\n", twin.DeviceId, twin.Tags);
        }

        public static async Task ReadDesiredProperties()
        {
            var twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            // Read and display Desired Properties
            Console.WriteLine("Desired Properties for {0} device: {1}\n", twin.DeviceId, twin.Properties.Desired);
        }

        public static async Task UpdateDesiredProperties(string strPropName, string strPropValue)
        {
            var twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            var newDesiredProp = $@"{{ properties: {{ desired: {{{strPropName}: {strPropValue}}} }} }}";

            await registryManager.UpdateTwinAsync(twin.DeviceId, newDesiredProp, twin.ETag);

            twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            // Read and display Desired Properties
            Console.WriteLine("Desired Properties for {0} device: {1}\n", twin.DeviceId, twin.Properties.Desired);
        }

        public static async Task ReadReportedProperties()
        {
            var twin = await registryManager.GetTwinAsync(MXCHIP_DEVICENAME);

            // Read and display Reported Properties
            Console.WriteLine("Reported Properties for {0} device: {1}\n", twin.DeviceId, twin.Properties.Reported);

        }

        static async Task Main(string[] args)
        {
            var bExit = false;

            registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            do
            {
                // Ask user to choose a selection
                Console.WriteLine("1: Read Device Twin Tags");
                Console.WriteLine("2: Write Device Twin Tags");
                Console.WriteLine("3: Read Device Twin Desired Properties");
                Console.WriteLine("4: Write Device Twin Desired Properties");
                Console.WriteLine("5: Read Device Twin Reported Properties");
                Console.WriteLine("Any other selection exits the program\n");

                Console.Write("Enter your selection: ");
                var nSelection = Console.ReadLine();

                Console.WriteLine("\n");

                switch (nSelection)
                {
                    case "1":     // Read Device Twin Tags
                        ReadTags().Wait();

                        break;

                    case "2":     // Write Device Twin Tags
                        Console.Write("Enter Tag Name: ");
                        var strTagName = Console.ReadLine();
                        Console.Write("Enter Tag Value: ");
                        var strTagvalue = Console.ReadLine();

                        UpdateTags(strTagName, strTagvalue).Wait();

                        break;

                    case "3":     // Read Device Twin Desired Properties
                        ReadDesiredProperties().Wait();

                        break;

                    case "4":     // Write Device Twin Desired Properties
                        Console.Write("Enter Desired Property Name: ");
                        var strDesiredPropName = Console.ReadLine();
                        Console.Write("Enter Desired Property Value: ");
                        var strDesiredPropvalue = Console.ReadLine();

                        UpdateDesiredProperties(strDesiredPropName, strDesiredPropvalue).Wait();

                        break;

                    case "5":     // Read Device Twin Reported Properties
                        ReadReportedProperties().Wait();

                        break;

                    default:
                        bExit = true;
                        break;
                }

            } while (!bExit);
        }
    }
}
