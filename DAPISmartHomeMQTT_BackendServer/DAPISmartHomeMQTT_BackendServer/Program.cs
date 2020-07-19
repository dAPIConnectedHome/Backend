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

namespace DAPISmartHomeMQTT_BackendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);

            Task.Run(MqttClientAdderTask);

            using (var context = new SmartHomeDBContext())
            {
                foreach( Mqttclients element in context.Mqttclients)
                {
                    Constances.MqttClientConnections.Add(new ClientConnection(element.ClientId,
                        new MqttClientOptionsBuilder()
                        .WithClientId(element.ClientId)
                            .WithTcpServer(Constances.MqttServerAddr, Constances.MqttServerPort)
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

        private static void MqttClientAdderTask()
        {
            var factory = new MqttFactory();

            var Creatorclient = factory.CreateMqttClient();
            Creatorclient.UseApplicationMessageReceivedHandler(AddnewClientReceivehandler);
            var options = new MqttClientOptionsBuilder()
                            .WithClientId("BackendBot")
                            .WithTcpServer(Constances.MqttServerAddr, Constances.MqttServerPort)
                            .Build();
            Creatorclient.SubscribeAsync("shlogo/#");
        }
        private static void AddnewClientReceivehandler(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine((new System.Text.ASCIIEncoding()).GetString(e.ApplicationMessage.Payload));
        }

    }
}
