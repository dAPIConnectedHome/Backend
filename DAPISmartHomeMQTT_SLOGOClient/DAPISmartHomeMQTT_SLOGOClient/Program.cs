using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using S7.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAPISmartHomeMQTT_SLOGOClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            mqttClient.UseApplicationMessageReceivedHandler(Receivhandler);
            IMqttClientOptions clientoptions = new MqttClientOptionsBuilder()
                .WithClientId("A1000L")
                .WithTcpServer("shlogo.dd-dns.de", 60000)
                .Build();
            await mqttClient.ConnectAsync(clientoptions, CancellationToken.None);
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("SHLOGO/A1000").Build());
            while (true) ;
        }

        //shlogo/new/ publish id
        //shlogo/new/defaultid/# -> id
        //shlogo/id

        private static void Receivhandler(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("I have been here");
            Plc plc = new Plc(CpuType.Logo0BA8, "192.168.0.30", 0, 0);

            if (plc.IsAvailable)
            {
                plc.Open();

                //plc.Read(DataType.Output, int)
                if (plc.IsConnected)
                {
                    int value = 0;
                    if (int.TryParse((new System.Text.ASCIIEncoding()).GetString(e.ApplicationMessage.Payload), out value))
                    {
                        switch (value)
                        {
                            case 1:
                                {
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 0, true);
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 0, false);
                                    break;
                                }
                            case 2:
                                {
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 1, true);
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 1, false);
                                    break;
                                }

                            case 3:
                                {
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 2, true);
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 2, false);
                                    break;
                                }
                            case 4:
                                {
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 3, true);
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 3, false);
                                    break;
                                }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Fehler bei Verbindung");
                }

                plc.Close();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
