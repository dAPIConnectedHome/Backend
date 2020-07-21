using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using S7.Net;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAPISmartHomeMQTT_SLOGOClient
{
    /// <summary>
    /// Class containing static Connectioninfo
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// MqttBroker adress
        /// </summary>
        public static string Brokeradress = "shlogo.dd-dns.de";

        /// <summary>
        /// MqttBroker port
        /// </summary>
        public static int BrokerPort = 60000;

        /// <summary>
        /// localipadress of the siemens logo
        /// </summary>
        public static string SLogoIP = "192.168.0.30";
    }

    class Program
    {
        /// <summary>
        /// Locking object for controlling plc access
        /// </summary>
        static readonly object plclock = new object();
        
        /// <summary>
        /// Plc Object for Siemenslogo access
        /// </summary>
        static Plc plc = new Plc(CpuType.Logo0BA8, Constants.SLogoIP, 0, 0);

        /// <summary>
        /// Mqttclient for Datatransfer
        /// </summary>
        static public IMqttClient mqttClient;

        static async Task Main(string[] args)
        {
            //Mqttclient init
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

            
            //Start Task for updating siemenslogo values to Database
            Task pollingTask = new Task(PollingFunction);
            pollingTask.Start();

            
            
            while (true) ;
        }

        /// <summary>
        /// MqttReceived Handler for Setting Values
        /// </summary>
        private static void RangeLampReceivhandler(MqttApplicationMessageReceivedEventArgs e)
        {
            lock (plclock)
            {
                if (plc.IsAvailable)
                {
                    plc.Open();

                    if (plc.IsConnected)
                    {
                        //Get Data from Message
                        int value = int.Parse(Convert.ToChar(e.ApplicationMessage.Payload.Last()).ToString());

                        //Set plc input Memory depending on Sender of Message
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
                        //Keep Thread alive -> no Exception is throwen
                        Console.WriteLine("Fehler bei Verbindung");
                        return;
                    }

                    plc.Close();
                }
                else
                {
                    //Keep Thread alive -> no Exception is throwen
                    Console.WriteLine("Verbindung zu plc nicht mehr möglich");
                    return;
                }
            }
        }

        /// <summary>
        /// Function for Updating Values in DB
        /// </summary>
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