using System.ComponentModel.DataAnnotations;

namespace DAPISmartHomeMQTT_BackendServer.Models
{
    public class MQTTClientType
    {
        [Key]
        public string TypeID;
        public TypeDirection TypeDirection;
        public TypeMode Mode;
        public double RangeMin;
        public double RangeMax;
    }
}
