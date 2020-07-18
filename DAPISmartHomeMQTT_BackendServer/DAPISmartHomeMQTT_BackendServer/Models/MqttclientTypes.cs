using System;
using System.Collections.Generic;

namespace DAPISmartHomeMQTT_BackendServer.Models
{
    public partial class MqttclientTypes
    {
        public MqttclientTypes()
        {
            Mqttclients = new HashSet<Mqttclients>();
        }

        public int TypeId { get; set; }
        public string Direction { get; set; }
        public string Mode { get; set; }
        public int? RangeMin { get; set; }
        public int? RangeMax { get; set; }

        public virtual ICollection<Mqttclients> Mqttclients { get; set; }
    }
}
