﻿using System;
using System.Collections.Generic;

namespace DAPISmartHomeMQTT_BackendServer.Models
{
    public partial class Mqttclients
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Room { get; set; }
        public int? Typeid { get; set; }
        public int? GroupId { get; set; }

        public virtual MqttclientTypes Type { get; set; }
    }
}