using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAPISmartHomeMQTT_BackendServer.Models;

namespace DAPISmartHomeMQTT_BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttclientTypesController : ControllerBase
    {
        private readonly SmartHomeDBContext _context;

        public MqttclientTypesController(SmartHomeDBContext context)
        {
            _context = context;
        }

        // GET: api/MqttclientTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MqttclientTypes>>> GetMqttclientTypes()
        {
            return await _context.MqttclientTypes.ToListAsync();
        }

        // GET: api/MqttclientTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MqttclientTypes>> GetMqttclientTypes(int id)
        {
            var mqttclientTypes = await _context.MqttclientTypes.FirstAsync(y => y.TypeId.Equals(id));

            if (mqttclientTypes == null)
            {
                return NotFound();
            }

            return mqttclientTypes;
        }

        private bool MqttclientTypesExists(int id)
        {
            return _context.MqttclientTypes.Any(e => e.TypeId == id);
        }
    }
}
