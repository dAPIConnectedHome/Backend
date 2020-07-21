using DAPISmartHomeMQTT_BackendServer.Models;
using MQTTnet.Client;
using System.Collections.Generic;

namespace DAPISmartHomeMQTT_BackendServer
{
    static public class Constances
    {
        /// <summary>
        /// Mqttclient for dynamic sensor/aktor adding
        /// </summary>
        public static IMqttClient BackendDataClient;

        public static string MqttServerAddr = "shlogo.dd-dns.de";

        public static int MqttServerPort = 60000;

        /// <summary>
        /// Mqttclient of all db entrys
        /// </summary>
        public static List<ClientConnection> MqttClientConnections = new List<ClientConnection>();
    }
}
