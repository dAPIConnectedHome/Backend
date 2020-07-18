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
    public class MqttclientsController : ControllerBase
    {
        private readonly SmartHomeDBContext _context;

        public MqttclientsController(SmartHomeDBContext context)
        {
            _context = context;
        }

        // GET: api/Mqttclients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mqttclients>>> GetMqttclients()
        {

            return await _context.Mqttclients.ToListAsync();
        }

        // GET: api/Mqttclients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mqttclients>> GetMqttclients(string id)
        {
            var mqttclients = await _context.Mqttclients.FirstAsync(x => x.ClientId.Equals(id));

            if (mqttclients == null)
            {
                return NotFound();
            }

            return mqttclients;
        }

        // PUT: api/Mqttclients/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMqttclients(string id, string[] data)
        {
            List<Mqttclients> tempclients = _context.Mqttclients.Where(x => x.ClientId.Equals(id)).ToList();
            Mqttclients tempclient = tempclients.First();
            if (tempclients.Any() && data.Length >= 3)
            {
                tempclient.Name = data[0];
                tempclient.Room = data[1];
                tempclient.GroupId = data[2];
                _context.Entry(tempclient).State = EntityState.Modified;
            }
            else
            {
                return BadRequest();
            }

            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MqttclientsExists(id))
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

        // POST: api/Mqttclients
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("{id}")]
        public async Task<ActionResult<Mqttclients>> PostMqttclients(Mqttclients mqttclients)
        {
            _context.Mqttclients.Add(mqttclients);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMqttclients", new { id = mqttclients.ClientId }, mqttclients);
        }

        // DELETE: api/Mqttclients/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Mqttclients>> DeleteMqttclients(string id)
        {
            var mqttclients = await _context.Mqttclients.FindAsync(id);
            if (mqttclients == null)
            {
                return NotFound();
            }

            _context.Mqttclients.Remove(mqttclients);
            await _context.SaveChangesAsync();

            return mqttclients;
        }

        private bool MqttclientsExists(string id)
        {
            return _context.Mqttclients.Any(e => e.ClientId == id);
        }
    }
}
