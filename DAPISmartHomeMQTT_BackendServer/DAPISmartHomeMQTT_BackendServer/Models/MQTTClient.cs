using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace DAPISmartHomeMQTT_BackendServer.Models
{
    public class MQTTClient
    {
        public string ClientID;

        public string Room;
        public string Topic;
        public MQTTClientType type;
        
    }
}
