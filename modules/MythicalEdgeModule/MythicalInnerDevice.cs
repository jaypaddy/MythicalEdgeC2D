 
 namespace MythicalEdgeModule
{
    using Microsoft.Azure.Devices.Client;
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Diagnostics;



    public class MythicalInnerDevice
    {
        [JsonProperty(PropertyName = "devicename")]
        public String DeviceName { get; set; }

        [JsonProperty(PropertyName = "connstring")]
        public String ConnString { get; set; }


        public MythicalInnerDevice(String deviceName, string connectionString)
        {
            DeviceName = deviceName;
            ConnString = connectionString;
        }

        private Boolean bConnected=false;
        public Boolean IsConnected(){
            return bConnected;
        }

        private  DeviceClient s_deviceClient;
        private static readonly TransportType s_transportType = TransportType.Mqtt;    

        public void ConnectInnerDevice()
        {
            Console.WriteLine("\tConnecting InnerDevice with MQTT transport type...");
            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(ConnString, s_transportType);
            //Need to  add an conneection staatus cheeck handler
            bConnected = true;
            // Now subscibe to receiving the callback.
            //await s_deviceClient.SetReceiveMessageHandlerAsync(OnC2dMessageReceived, s_deviceClient);
        }

        public  async Task  ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await s_deviceClient.ReceiveAsync();
                if (receivedMessage == null) break;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", 
                Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await s_deviceClient.CompleteAsync(receivedMessage);
                receivedMessage.Dispose();
            }
        }

        private async Task OnC2dMessageReceived(Message receivedMessage, object _)
        {
            Console.WriteLine($"{DateTime.Now}> C2D message callback - message received with Id={receivedMessage.MessageId}.");
            ProcessReceivedMessage(receivedMessage);

            await s_deviceClient.CompleteAsync(receivedMessage);
            Console.WriteLine($"{DateTime.Now}> Completed C2D message with Id={receivedMessage.MessageId}.");

            receivedMessage.Dispose();
        }

        private void ProcessReceivedMessage(Message receivedMessage)
        {
            string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
            var formattedMessage = new StringBuilder($"Received message: [{messageData}]\n");

            // User set application properties can be retrieved from the Message.Properties dictionary.
            foreach (KeyValuePair<string, string> prop in receivedMessage.Properties)
            {
                formattedMessage.AppendLine($"\tProperty: key={prop.Key}, value={prop.Value}");
            }
            // System properties can be accessed using their respective accessors.
            formattedMessage.AppendLine($"\tMessageId: {receivedMessage.MessageId}");

            Console.WriteLine($"{DateTime.Now}> {formattedMessage}");
        }
    }
}