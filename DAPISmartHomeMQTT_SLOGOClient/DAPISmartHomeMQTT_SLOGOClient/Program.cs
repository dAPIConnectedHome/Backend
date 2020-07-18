using MQTTnet;
using S7.Net;
using System;

namespace DAPISmartHomeMQTT_SLOGOClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            Plc plc = new Plc(CpuType.Logo0BA8, "192.168.0.30", 0, 0);

            if (plc.IsAvailable)
            {
                plc.Open();

                plc.WriteBit(DataType.Input, 0, 0, 0, 1);

                plc.Close();
            }
            else
            {
                throw new Exception();
            }
            Console.WriteLine("Hello World!");
        }
    }
}
