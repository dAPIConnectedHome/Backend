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
    class Program
    {
        static List<Mqttclient> mqttClients = new List<Mqttclient>();
        static List<MqttclientType> mqttClientTypes = new List<MqttclientType>();
        static Plc plc = new Plc(CpuType.Logo0BA8, "192.168.0.30", 0, 0);

        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();

            //Client für Hinzufügen von neuen clients
            var Creatorclient = factory.CreateMqttClient();
            Creatorclient.UseApplicationMessageReceivedHandler(CreateNewClientReceivehandler);
            IMqttClientOptions creatorclientOptions = new MqttClientOptionsBuilder()
                .WithClientId("SLogoBot")
                .WithTcpServer("shlogo.dd-dns.de", 60000)
                .Build();
            await Creatorclient.ConnectAsync(creatorclientOptions, CancellationToken.None);

            //TBD ask for id foreach non added Task
            

            // async function for getting outputvalues all 0.5 sek
            Task PollingTask = Task.Run(PollingFunction, CancellationToken.None);

            //var mqttClient = factory.CreateMqttClient();
            //mqttClient.UseApplicationMessageReceivedHandler(RangeLampReceivhandler);
            //IMqttClientOptions clientoptions = new MqttClientOptionsBuilder()
            //    .WithClientId("A1000L")
            //    .WithTcpServer("shlogo.dd-dns.de", 60000)
            //    .Build();
            //await mqttClient.ConnectAsync(clientoptions, CancellationToken.None);
            //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("SHLOGO/A1000").Build());

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter key for corresponding option.");
                Console.WriteLine("c: Add new Logo-Mqtt Opject");
                Console.WriteLine("x: Close the Applikation");

                char menuselect;
                menuselect = Console.ReadKey().KeyChar;
                
                switch(menuselect)
                {
                    case 'c':
                    {
                            Console.WriteLine("Enter the Name of the mqtt client:");
                            string mqttClientName = Console.ReadLine();

                            Console.WriteLine("Enter the tempid of the mqtt client:");
                            string tempclientid = Console.ReadLine();
                            
                            int bytepos;
                            Console.WriteLine("Enter bytepos");
                            while (!int.TryParse(Console.ReadLine(), out bytepos))
                                Console.WriteLine("Pls enter a valid integer!");

                            int bitpos;
                            Console.WriteLine("Enter bitpos");
                            while (!int.TryParse(Console.ReadLine(), out bitpos))
                                Console.WriteLine("Pls enter a valid integer!");

                            Console.WriteLine("Select a existing type or create a new one.");
                            Console.WriteLine("n: Create new type.");
                            Console.WriteLine("s: Select existing.");

                            MqttclientType typeresult;
                            char submenuselection;
                            submenuselection = Console.ReadKey().KeyChar;
                            switch (submenuselection)
                            {
                                case 'n':
                                    {
                                        typeresult = AddClientType();
                                        
                                        break;
                                    }
                                case 's':
                                    {
                                        if (!mqttClientTypes.Any())
                                        {
                                            typeresult = AddClientType();
                                            return;
                                        }

                                        Console.WriteLine("Select via typeid:");
                                        foreach (MqttclientType element in mqttClientTypes)
                                        {
                                            Console.WriteLine("TypeID:  " + element.TypeId + "  Direction:  " + element.Direction + "   Mode:  " + element.Mode + " RangeMin:  " + element.RangeMin + " RangeMax:  " + element.RangeMax);
                                        }

                                        int typeid;
                                        while (!(int.TryParse(Console.ReadLine(), out typeid) && mqttClientTypes.Any(x => x.TypeId.Equals(typeid))))
                                            Console.WriteLine("Pls enter a valid integer!");

                                        typeresult = mqttClientTypes.Where(x => x.TypeId.Equals(typeid)).First();

                                        break;
                                    }
                                default:
                                    {
                                        Console.WriteLine("You Failed to Select!");
                                        continue;
                                    }
                            }

                            //Client für Hinzufügen von neuen clients
                            var newclient = factory.CreateMqttClient();
                            newclient.UseApplicationMessageReceivedHandler(ChangeValueReceivehandler);
                            IMqttClientOptions newclientOptions = new MqttClientOptionsBuilder()
                                .WithClientId("mqttClientName")
                                .WithTcpServer("shlogo.dd-dns.de", 60000)
                                .Build();
                            await newclient.ConnectAsync(newclientOptions, CancellationToken.None);

                            mqttClients.Add(new Mqttclient(bytepos, bitpos, tempclientid, mqttClientName, tempclientid, typeresult, newclient));

                            break;
                    }
                    case 'x':
                        {
                            return;
                        }

                }

                List<Mqttclient> tempcoll = mqttClients.Where(x => x.added.Equals(false)).ToList();

                if(tempcoll.Any())
                {
                    Mqttclient tobeaddedclient = tempcoll.First();
                    while (!Creatorclient.IsConnected);
                    var mqttmessage = new MqttApplicationMessageBuilder()
                        .WithTopic("shlogo/new/" + tobeaddedclient.ClientId)
                        .WithPayload((new System.Text.ASCIIEncoding()).GetBytes(tobeaddedclient.ClientId))
                        .WithExactlyOnceQoS()
                        .Build();
                    await Creatorclient.SubscribeAsync("shlogo/new/" + tobeaddedclient.ClientId);
                    await Creatorclient.PublishAsync(mqttmessage, CancellationToken.None);
                }

            }
        }

        //shlogo/new/ publish id
        //shlogo/new/defaultid/# -> id
        //shlogo/id



        private static void RangeLampReceivhandler(MqttApplicationMessageReceivedEventArgs e)
        {
            if (plc.IsAvailable)
            {
                plc.Open();

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

        private static void CreateNewClientReceivehandler(MqttApplicationMessageReceivedEventArgs e)
        {
            //ID auslesen
            string newid = (new System.Text.ASCIIEncoding()).GetString(e.ApplicationMessage.Payload);

            //Format überprüfen

            //Neuen Client mit ID erstellen
            Mqttclient tobeaddedclient =  mqttClients.Where(x => x.ClientId.Equals(e.ApplicationMessage.Topic.Split('/').Last())).First();
            tobeaddedclient.ClientId = newid;
            tobeaddedclient.Topic = "shlogo/" + newid;

            //Neue ClientDaten übertragen
            var mqttmessage = new MqttApplicationMessageBuilder()
                        .WithTopic(tobeaddedclient.Topic)
                        .WithPayload((new System.Text.ASCIIEncoding()).GetBytes(tobeaddedclient.ClientId + "," + "newClient" + "," + tobeaddedclient.Topic + "," + "-1" + "," + tobeaddedclient.Typeid + "," + -1 + "," + tobeaddedclient.Type.RangeMin))
                        .WithExactlyOnceQoS()
                        .Build();
            tobeaddedclient._mqttClient.SubscribeAsync(tobeaddedclient.Topic);
            tobeaddedclient._mqttClient.PublishAsync(mqttmessage, CancellationToken.None);
            tobeaddedclient.added = true;
        }

        private static void ChangeValueReceivehandler(MqttApplicationMessageReceivedEventArgs e)
        {
            //TBD
        }

        public static void PollingFunction()
        {
            //Update all Clientvalues
            while (true)
            {
                    foreach (Mqttclient element in mqttClients.Where(x => x.added.Equals(true)))
                    {
                        if (plc.IsAvailable)
                        {
                            plc.Open();
                            //TBD Testen
                            if (plc.IsConnected)
                            {
                                byte bitpos = Convert.ToByte(1 << Convert.ToByte(element.bitPos));
                                var result = plc.Read(DataType.DataBlock, 200, element.bytePos, VarType.Bit, 1, bitAdr: bitpos);
                                element.CurrentValue = (byte)result;
                            }
                            plc.Close();
                        }
                        else
                        {
                            throw new Exception("Logo not available!");
                        }
                    }
                Thread.Sleep(500);                
                
            }
        }

        public static MqttclientType AddClientType()
        {
            Console.WriteLine("Enter typeid(0000):");
            int typeid;
            while (!int.TryParse(Console.ReadLine(), out typeid))
                Console.WriteLine("Pls enter a valid integer!");

            Console.WriteLine("Enter Direction(S,R,T)");
            string direction = Console.ReadLine();

            Console.WriteLine("Enter Mode(BOOL,RANGE)");
            string mode = Console.ReadLine();

            Console.WriteLine("Enter minvalue:");
            int minvalue;
            while (!int.TryParse(Console.ReadLine(), out minvalue))
                Console.WriteLine("Pls enter a valid integer!");

            Console.WriteLine("Enter maxvalue:");
            int maxvalue;
            while (!int.TryParse(Console.ReadLine(), out maxvalue))
                Console.WriteLine("Pls enter a valid integer!");

            MqttclientType newtype = new MqttclientType(typeid, direction, mode, minvalue, maxvalue);
            mqttClientTypes.Add(newtype);

            return newtype;
        }

    }
    public partial class Mqttclient
    {
        public bool added = false;

        public IMqttClient _mqttClient;

        public int bytePos { get; set; }
        public int bitPos { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string Topic { get; set; }
        public int Typeid { get; set; }

        public virtual MqttclientType Type { get; set; }

        private int currentvalue = 0;
        public int CurrentValue
        {
            get
            {
                return currentvalue;
            }
            set
            {
                if(currentvalue != value)
                {
                    currentvalue = value;

                    while (!_mqttClient.IsConnected) ;
                    var mqttmessage = new MqttApplicationMessageBuilder()
                        .WithTopic(Topic)
                        .WithPayload(BitConverter.GetBytes(currentvalue))
                        .WithExactlyOnceQoS()
                        .Build();
                    _mqttClient.PublishAsync(mqttmessage, CancellationToken.None);

                }
            }
        }

        public Mqttclient(int bytepos, int bitpos, string clientid, string clientname, string topic, MqttclientType type, IMqttClient mqttClient)
        {
            _mqttClient = mqttClient;

            bytePos = bytepos;
            bitPos = bitpos;
            ClientId = clientid;
            ClientName = clientname;
            Topic = topic;
            Type = type;
            Typeid = Type.TypeId;
        }
    }

    public partial class MqttclientType
    {
        public bool added = false;

        public int TypeId { get; set; }
        public string Direction { get; set; }
        public string Mode { get; set; }
        public int RangeMin { get; set; }
        public int RangeMax { get; set; }
        public MqttclientType(int typeid, string direction, string mode, int rangemin, int rangemax)
        {
            TypeId = typeid;
            Direction = direction;
            Mode = mode;
            RangeMin = rangemin;
            RangeMax = rangemax;
        }
    }
}
