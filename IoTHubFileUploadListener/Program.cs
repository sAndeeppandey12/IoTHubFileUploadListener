using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace IoTHubFileUploadListener
{
    class Program
    {
        static string connectionString = "HostName=asdads.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=adjlkasjdfsadflkjlksajfd";
        static ServiceClient serviceClient;

        private static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Receive file upload notifications\n");
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString, TransportType.Amqp_WebSocket_Only);
                await ReceiveFileUploadNotificationAsync();
                await serviceClient.CloseAsync();
                Console.WriteLine("Press Enter to exit\n");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                await serviceClient.CloseAsync();
                Console.WriteLine(e);
            }
        }

        private static async Task ReceiveFileUploadNotificationAsync()
        {
            var notificationReceiver = serviceClient.GetFileNotificationReceiver();
            Console.WriteLine("\nReceiving file upload notification from service");
            int totalDownloadedFile = 1;
            var enqueTimeList = new List<double>();

            var stopwatch = Stopwatch.StartNew();
            while (true)
            {

                var fileUploadNotification = await notificationReceiver.ReceiveAsync();
                if (fileUploadNotification == null)
                    continue;

                var totalSeconds = (DateTime.UtcNow - fileUploadNotification.EnqueuedTimeUtc);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(value: $"Number of notification recieved :{totalDownloadedFile++}, TimeTaken : {stopwatch.ElapsedMilliseconds} , FileName : {fileUploadNotification.BlobName} , DeviceId : {fileUploadNotification}");
                Console.ResetColor();
                await notificationReceiver.CompleteAsync(fileUploadNotification);
                enqueTimeList.Add(totalSeconds.TotalSeconds);
            }

            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine($"Min enqueued time :{enqueTimeList.Min()}");
            //Console.WriteLine($"Max enqueued time :{enqueTimeList.Max()}");
            //Console.WriteLine($"Avg enqueued time :{enqueTimeList.Average()}");
        }
    }
}
