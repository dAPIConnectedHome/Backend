using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using S7.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DAPISmartHomeMQTT_SLOGOClient
{
    public static class Constants
    {
        public static string Brokeradress = "shlogo.dd-dns.de";
        public static int BrokerPort = 60000;
        public static string SLogoIP = "192.168.0.30";
    }

    class Program
    {
        static readonly object plclock = new object();
        static Plc plc = new Plc(CpuType.Logo0BA8, Constants.SLogoIP, 0, 0);
        static public IMqttClient mqttClient;

        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();

            mqttClient = factory.CreateMqttClient();
            mqttClient.UseApplicationMessageReceivedHandler(RangeLampReceivhandler);
            IMqttClientOptions mqttClientI1options = new MqttClientOptionsBuilder()
                .WithClientId("LogoBot")
                .WithTcpServer(Constants.Brokeradress, Constants.BrokerPort)
                .Build();
            await mqttClient.ConnectAsync(mqttClientI1options, CancellationToken.None);
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("shlogo/A1000/set/").Build());
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("shlogo/A1001/set/").Build());
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("shlogo/A1002/set/").Build());
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("shlogo/A1003/set/").Build());

            Task pollingTask = new Task(PollingFunction);
            pollingTask.Start();

            while (true) ;
        }

        private static void RangeLampReceivhandler(MqttApplicationMessageReceivedEventArgs e)
        {
            lock (plclock)
            {
                if (plc.IsAvailable)
                {
                    plc.Open();

                    if (plc.IsConnected)
                    {
                        int value = int.Parse(Convert.ToChar(e.ApplicationMessage.Payload.Last()).ToString());

                        string Aktorid = e.ApplicationMessage.Topic.Split('/').ElementAt(1);
                        Console.WriteLine(Aktorid + ">> Value set to:" + value);
                        switch (Aktorid)
                        {
                            case "A1000":
                                {
                                    if (value > 0)
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 0, true);
                                    }
                                    else
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 0, false);
                                    }
                                    break;
                                }
                            case "A1001":
                                {
                                    if (value > 0)
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 1, true);
                                    }
                                    else
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 1, false);
                                    }
                                    break;
                                }

                            case "A1002":
                                {
                                    if (value > 0)
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 2, true);
                                    }
                                    else
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 2, false);
                                    }
                                    break;
                                }
                            case "A1003":
                                {
                                    if (value > 0)
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 3, true);
                                    }
                                    else
                                    {
                                        plc.WriteBit(DataType.DataBlock, 200, 0, 3, false);
                                    }
                                    break;
                                }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Fehler bei Verbindung");
                        return;
                    }

                    plc.Close();
                }
                else
                {
                    Console.WriteLine("Verbindung zu plc nicht mehr möglich");
                    return;
                }
            }
        }

        private static void ChangeValueReceivehandler(MqttApplicationMessageReceivedEventArgs e)
        {
            if (plc.IsAvailable)
            {
                plc.OpenAsync();

                if (plc.IsConnected)
                {
                    string id = e.ApplicationMessage.Topic.Split('/').Last();
                    int value = 0;
                    if (int.TryParse((new System.Text.ASCIIEncoding()).GetString(e.ApplicationMessage.Payload), out value))
                    {
                        if (id.Equals("A1000"))
                        {
                            lock (plclock)
                            {
                                if (value == 1)
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 1, true);
                                else if (value == 0)
                                    plc.WriteBit(DataType.DataBlock, 200, 0, 1, false);
                            }
                        }
                        else if (id.Equals("A1001"))
                        {
                            if (value == 1)
                                plc.WriteBit(DataType.DataBlock, 200, 0, 2, true);
                            else if (value == 0)
                                plc.WriteBit(DataType.DataBlock, 200, 0, 2, false);
                        }
                        else if (id.Equals("A1002"))
                        {
                            if (value == 1)
                                plc.WriteBit(DataType.DataBlock, 200, 0, 3, true);
                            else if (value == 0)
                                plc.WriteBit(DataType.DataBlock, 200, 0, 2, false);
                        }
                        else if (id.Equals("A1003"))
                        {
                            if (value == 1)
                                plc.WriteBit(DataType.DataBlock, 200, 0, 4, true);
                            else if (value == 0)
                                plc.WriteBit(DataType.DataBlock, 200, 0, 2, false);
                        }

                    }
                }
                else
                {
                    throw new Exception("plc not connected");
                }
            }
            else
            {
                throw new Exception("plc unavailable");
            }
        }

        public static void PollingFunction()
        {
            //Update all Clientvalues
            while (true)
            {
                lock (plclock)
                {
                    if(plc.IsAvailable)
                    {
                        plc.Open();

                        if(plc.IsConnected)
                        {
                            //var currentA1000 = plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 0);
                            //var currentA1001 = plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 1);
                            //var currentA1002 = plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 2);
                            //var currentA1003 = plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 3);

                            bool currentA1000 = (bool)plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 0);
                            bool currentA1001 = (bool)plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 1);
                            bool currentA1002 = (bool)plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 2);
                            bool currentA1003 = (bool)plc.Read(DataType.DataBlock, 200, 1, VarType.Bit, 1, 3);


                            plc.WriteBit(DataType.DataBlock, 200, 0, 0, currentA1000);
                            plc.WriteBit(DataType.DataBlock, 200, 0, 1, currentA1001);
                            plc.WriteBit(DataType.DataBlock, 200, 0, 2, currentA1002);
                            plc.WriteBit(DataType.DataBlock, 200, 0, 3, currentA1003);

                            Console.WriteLine(currentA1000 + "  " + currentA1001 + "  " + currentA1002 + "  " + currentA1003 + "  ");
                            mqttClient.PublishAsync("shlogo/A1000/", currentA1000 == true ? "1" : "0");
                            mqttClient.PublishAsync("shlogo/A1001/", currentA1001 == true ? "1" : "0");
                            mqttClient.PublishAsync("shlogo/A1002/", currentA1002 == true ? "1" : "0");
                            mqttClient.PublishAsync("shlogo/A1003/", currentA1003 == true ? "1" : "0");
                        }

                        plc.WriteTimeout = 100;
                        plc.WriteTimeout = 100;
                        plc.Close();
                    }
                    

                    

                    Thread.Sleep(1000);
                }
            }
        }

    }
}