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

        public static string MqttBrokerAddr = "shlogo.dd-dns.de";

        public static int MqttBrokerPort = 60000;

        public static string Mariadbaccess = "server=192.168.0.10;port=3306;database=SmartHomeDB;user=BackendUser;password=qawsed";

        /// <summary>
        /// Mqttclient of all db entrys
        /// </summary>
        public static List<ClientConnection> MqttClientConnections = new List<ClientConnection>();
    }
}
