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

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDTO>>> GetDevice()
        {
            var devices = await _context.Devices.ToListAsync();
            List<DeviceDTO> devicesDTO = new List<DeviceDTO>();

            foreach (Device d in devices)
            {
                var deviceDTO = new DeviceDTO()
                {
                    Id = d.Id,
                    Name = d.Name,
                    Mode = d.Mode
                };
                devicesDTO.Add(deviceDTO);
            }

            return Ok(devicesDTO);
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDTO>> GetDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            var deviceDTO = new DeviceDTO()
            {
                Id = device.Id,
                Name = device.Name,
                Mode = device.Mode
            };

            return Ok(deviceDTO);
        }

        // PUT: api/Devices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(int id, Device device)
        {
            if (id != device.Id)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

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

        // POST: api/Devices
        [HttpPost]
        public async Task<ActionResult<DeviceDTO>> PostDevice(DeviceDTO device)
        {
            var newDevice = new Device()
            {
                Name = device.Name,
                Mode = device.Mode,
                User = null //TODO: add(find) User from url
            };

            await _context.Devices.AddAsync(newDevice);
            await _context.SaveChangesAsync();

            var deviceDTO = new DeviceDTO()
            {
                Id = newDevice.Id,
                Name = device.Name,
                Mode = device.Mode
            };

            return CreatedAtAction("GetDevice", new { id = deviceDTO.Id }, deviceDTO);
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

            return device;
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }
}
