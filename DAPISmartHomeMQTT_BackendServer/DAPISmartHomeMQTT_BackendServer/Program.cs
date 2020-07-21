using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAPISmartHomeMQTT_BackendServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using MySql.Data.MySqlClient;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client;
using System.Threading;
using System.Text;

namespace DAPISmartHomeMQTT_BackendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);

            //InitBackend Mqttaccess for dynamicly adding clients
            var factory = new MqttFactory();
            Constances.BackendDataClient = factory.CreateMqttClient();

            Constances.BackendDataClient.UseApplicationMessageReceivedHandler(NewElementReceivehandler);
            Constances.BackendDataClient.UseConnectedHandler(async e =>
            {
                await Constances.BackendDataClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("shlogo/data/").Build());
            });
            Constances.BackendDataClient.ConnectAsync(new MqttClientOptionsBuilder()
                        .WithClientId("BackendAdderClient")
                            .WithTcpServer(Constances.MqttBrokerAddr, Constances.MqttBrokerPort)
                            .Build(), CancellationToken.None);

            //add Mqttclient for each already existing db entry
            using (var context = new SmartHomeDBContext())
            {
                foreach( Mqttclients element in context.Mqttclients)
                {
                    Constances.MqttClientConnections.Add(new ClientConnection(element.ClientId,
                        new MqttClientOptionsBuilder()
                        .WithClientId(element.ClientId)
                            .WithTcpServer(Constances.MqttBrokerAddr, Constances.MqttBrokerPort)
                            .Build()));
                }
            }

            

                CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        /// MqttReceivedHandler for 
        /// </summary>
        private static void NewElementReceivehandler(MqttApplicationMessageReceivedEventArgs e)
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            string[] data = payload.Split(';');
            using(var context = new SmartHomeDBContext())
            {
                MqttclientTypes type;
                string ClientID = data[0];

                string name = data[1];
                string direction = data[2];
                string mode = data[3];
                int rangemin = int.Parse(data[4]);
                int rangemax = int.Parse(data[5]);
                var tempcol = context.MqttclientTypes.Where(x => x.Name.Equals(name) && x.Direction.Equals(direction) && x.Mode.Equals(mode) && x.RangeMin.Equals(rangemin) && x.RangeMax.Equals(rangemax));
                if (tempcol.Any())
                {
                    type = tempcol.First();
                }
                else
                {
                    type = new MqttclientTypes();
                    int i = 200;
                    while (context.MqttclientTypes.Any(x => x.TypeId.Equals(i)))
                        ++i;
                    type.TypeId =i;
                    type.Name = name;
                    type.Direction = direction;
                    type.Mode = mode;
                    type.RangeMin = rangemin;
                    type.RangeMax = rangemax;
                    context.Add<MqttclientTypes>(type);
                    context.SaveChanges();
                }

                Mqttclients newclient = new Mqttclients();
                newclient.ClientId = ClientID;
                newclient.Name = "-1";
                newclient.Topic = ClientID;
                newclient.Room = "-1";
                newclient.Typeid = type.TypeId;
                newclient.Type = type;
                newclient.GroupId = "-1";
                newclient.CurrentValue = rangemin;

                context.Add<Mqttclients>(newclient);
                context.SaveChanges();

                Constances.MqttClientConnections.Add(new ClientConnection(newclient.ClientId, new MqttClientOptionsBuilder()
                        .WithClientId(newclient.ClientId)
                            .WithTcpServer(Constances.MqttBrokerAddr, Constances.MqttBrokerPort)
                            .Build()));
            }
        }
    }
}
