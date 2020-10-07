using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTerraAPI.DTO;
using SmartTerraAPI.Models;

namespace SmartTerraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModesController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public ModesController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/Modes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModeDTO>>> GetMode()
        {
            var modes = await _context.Modes.ToListAsync();
            List<ModeDTO> modesDTO = new List<ModeDTO>();

            foreach (Mode m in modes)
            {
                var modeDTO = new ModeDTO()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Temperature = m.Temperature,
                    Humidity = m.Humidity,
                    TwilightHour = m.TwilightHour,
                    HourOfDawn = m.HourOfDawn

                };
                modesDTO.Add(modeDTO);
            }

            return Ok(modesDTO);
        }

        // GET: api/Modes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModeDTO>> GetMode(int id)
        {
            var mode = await _context.Modes.FindAsync(id);

            if (mode == null)
            {
                return NotFound();
            }

            var modeDTO = new ModeDTO()
            {
                Id = mode.Id,
                Name = mode.Name,
                Temperature = mode.Temperature,
                Humidity = mode.Humidity,
                TwilightHour = mode.TwilightHour,
                HourOfDawn = mode.HourOfDawn
            };

            return Ok(modeDTO);
        }

        // PUT: api/Modes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMode(int id, ModeDTO mode)
        {
                if (id != mode.Id)
                {
                    return BadRequest();
                }

            _context.Entry(mode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModeExists(id))
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

        // POST: api/Modes
        [HttpPost]
        public async Task<ActionResult<ModeDTO>> PostMode(ModeDTO mode)
        //ModeDTO because Device and DeviceId are required in Mode class
        //user cannot(should not) send Device object from body (I think)
        {
            //TODO:  check if name at users modes exist

            var newMode = new Mode()
            {
                Name = mode.Name,
                Temperature = mode.Temperature,
                Humidity = mode.Humidity,
                TwilightHour = mode.TwilightHour,
                HourOfDawn = mode.HourOfDawn,
                Device= null,//TODO: add(find) DeviceId and Device form url
                DeviceId = 0
            };

            await _context.Modes.AddAsync(newMode);
            await _context.SaveChangesAsync();

            var modeDTO = new ModeDTO()
            {
                Id = newMode.Id,
                Name = mode.Name,
                Temperature = mode.Temperature,
                Humidity = mode.Humidity,
                TwilightHour = mode.TwilightHour,
                HourOfDawn = mode.HourOfDawn
            };

            return CreatedAtAction("GetMode", new { id = modeDTO.Id }, modeDTO);
        }

        // DELETE: api/Modes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Mode>> DeleteMode(int id)
        {
            var mode = await _context.Modes.FindAsync(id);
            if (mode == null)
            {
                return NotFound();
            }

            _context.Modes.Remove(mode);
            await _context.SaveChangesAsync();

            return mode;
        }

        private bool ModeExists(int id)
        {
            return _context.Modes.Any(e => e.Id == id);
        }
    }
}
