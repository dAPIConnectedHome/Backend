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

        // PUT: api/MqttclientTypes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMqttclientTypes(int id, MqttclientTypes mqttclientTypes)
        {
            if (id != mqttclientTypes.TypeId)
            {
                return BadRequest();
            }

            _context.Entry(mqttclientTypes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MqttclientTypesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MqttclientTypes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MqttclientTypes>> PostMqttclientTypes(MqttclientTypes mqttclientTypes)
        {
            _context.MqttclientTypes.Add(mqttclientTypes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMqttclientTypes", new { id = mqttclientTypes.TypeId }, mqttclientTypes);
        }

        // DELETE: api/MqttclientTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MqttclientTypes>> DeleteMqttclientTypes(int id)
        {
            var mqttclientTypes = await _context.MqttclientTypes.FindAsync(id);
            if (mqttclientTypes == null)
            {
                return NotFound();
            }

            _context.MqttclientTypes.Remove(mqttclientTypes);
            await _context.SaveChangesAsync();

            return mqttclientTypes;
        }

        private bool MqttclientTypesExists(int id)
        {
            return _context.MqttclientTypes.Any(e => e.TypeId == id);
        }
    }
}
