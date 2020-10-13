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
    public class DevicesController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public DevicesController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/devices/{id}/modes
        [HttpGet("{id}/modes")]
        public async Task<ActionResult<ModeDTO>> GetMode(int id)
        {
            var device = await _context.Devices.Include(d=> d.Mode).Where(device =>device.Id == id).FirstOrDefaultAsync();

            if (device.Mode == null)
            {
                return BadRequest($"There is no mode for device with given id: {id}.");
            }

            var mode = device.Mode;

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

        // POST: api/devices/{id}/modes
        [HttpPost("{id}/modes")]
        public async Task<ActionResult<ModeDTO>> PostMode(int id, ModeDTO mode)
        {
            var device = await _context.Devices.FindAsync(id);

            var newMode = new Mode()
            {
                Name = mode.Name,
                Temperature = mode.Temperature,
                Humidity = mode.Humidity,
                TwilightHour = mode.TwilightHour,
                HourOfDawn = mode.HourOfDawn,
                Device = device,
                DeviceId = device.Id
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

        // PUT: api/Devices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(int id, DeviceDTO device)
        {
            /*if (id != device.Id)
            {
                return BadRequest();
            }*/
            var deviceToUpdate = await _context.Devices.FindAsync(id);
            if (deviceToUpdate == null)
            {
                return BadRequest($"There is no device for given id: {id}.");
            }

            deviceToUpdate.Name = device.Name;

            _context.Entry(deviceToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Device>> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return Ok("Device successfully deleted.");
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }
}
