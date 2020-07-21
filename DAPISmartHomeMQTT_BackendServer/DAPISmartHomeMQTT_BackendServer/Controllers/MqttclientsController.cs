using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAPISmartHomeMQTT_BackendServer.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using System.Reflection.Metadata;
using System.Threading;

namespace DAPISmartHomeMQTT_BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttclientsController : ControllerBase
    {
        private MqttFactory MqttClientFactory;
        private readonly SmartHomeDBContext _context;

        public MqttclientsController(SmartHomeDBContext context)
        {
            _context = context;
            MqttClientFactory = new MqttFactory();
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
            
            if (tempclients.Any())
            {
                Mqttclients tempclient = tempclients.First();
                if (data.Length >= 3)
                {
                    tempclient.Name = data[0];
                    tempclient.Room = data[1];
                    tempclient.GroupId = data[2];
                    _context.Entry(tempclient).State = EntityState.Modified;
                }
            }
            else
            {
                return NotFound();
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
        public async Task<ActionResult<Mqttclients>> PostMqttclients(string id, int[] value)
        {
            List<Mqttclients> matchedclients = _context.Mqttclients.Where(x => x.ClientId.Equals(id)).ToList();

            if(matchedclients.Any())
            {
                Mqttclients tempclient = matchedclients.First();
                if (value.Length > 0)
                {
                    int newvalue = value[0];
                    MqttclientTypes type = _context.MqttclientTypes.Where(x => x.TypeId.Equals(tempclient.Typeid)).First();
                    if ( newvalue >= type.RangeMin && newvalue <= type.RangeMax)
                    {
                        tempclient.CurrentValue = value[0];
                        _context.Entry(tempclient).State = EntityState.Modified;

                        //TBD send to MQTT client
                        List<ClientConnection> tempconnections = Constances.MqttClientConnections.Where(x => x.Clientid.Equals(tempclient.ClientId)).ToList();
                        if(tempconnections.Any())
                        {
                            ClientConnection tempconnection = tempconnections.First();
                            tempconnection.Send(newvalue.ToString());
                        }
                        else
                        {
                            throw new Exception("filling error");
                        }
                    }
                }
                

                _context.Entry(tempclient).State = EntityState.Modified;
            }
            else
            {
                return NotFound();
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

            return Ok();
        }

        private bool MqttclientsExists(string id)
        {
            return _context.Mqttclients.Any(e => e.ClientId == id);
        }
    }
}
