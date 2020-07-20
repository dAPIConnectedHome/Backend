using DAPISmartHomeMQTT_BackendServer.Models;
using MQTTnet.Client;
using System.Collections.Generic;

namespace DAPISmartHomeMQTT_BackendServer
{
    public enum TypeDirection
    {
        S,  //Send
        R,  //Receive
        T   //Transive
    }

    public enum TypeMode
    {
        Bool,
        Range,
    }

    static public class Constances
    {
        public static IMqttClient BackendDataClient;
        public static string MqttServerAddr = "shlogo.dd-dns.de";
        public static int MqttServerPort = 60000;
        public static List<ClientConnection> MqttClientConnections = new List<ClientConnection>();
    }
}
