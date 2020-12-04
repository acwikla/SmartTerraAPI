using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                IsOn = mode.IsOn,
                Temperature = mode.Temperature,
                Humidity = mode.Humidity,
                TwilightHour = mode.TwilightHour,
                HourOfDawn = mode.HourOfDawn
            };

            return Ok(modeDTO);
        }

        // GET: api/devices/{id}/deviceProperties
        [HttpGet("{id}/deviceProperties")]
        public async Task<ActionResult<IEnumerable<DevicePropertiesDTO>>> GetAllDeviceProperties(int id)
        {
            var device = await _context.Devices.Include(d => d.DeviceProperties).Where(device => device.Id == id).FirstOrDefaultAsync();
            var allDeviceProperties = device.DeviceProperties;

            if (allDeviceProperties == null)
            {
                return BadRequest($"There is no device properties for device with given id: {id}.");
            }

            List<DevicePropertiesDTO> allDevicePropertiesDTO = new List<DevicePropertiesDTO>();
            DevicePropertiesDTO devicePropertiesDTO;

            foreach (DeviceProperties d in allDeviceProperties)
            {
                devicePropertiesDTO = new DevicePropertiesDTO()
                {
                    Id = d.Id,
                    isLiquidLevelSufficient = d.isLiquidLevelSufficient,
                    Temperature = d.Temperature,
                    Humidity = d.Humidity,
                    HeatIndex = d.HeatIndex,
                    SoilMoisturePercentage = d.SoilMoisturePercentage,
                    LEDHexColor = d.LEDHexColor,
                    LEDBrightness = d.LEDBrightness
                };
                allDevicePropertiesDTO.Add(devicePropertiesDTO);
            }
            

            return Ok(allDevicePropertiesDTO);
        }

        // GET: api/devices/{id}/latestDeviceProperties
        [HttpGet("{id}/latestDeviceProperties")]
        public async Task<ActionResult<DevicePropertiesDTO>> GetLatestDeviceProperties(int id)
        {
            var device = await _context.Devices.Include(d => d.DeviceProperties).Where(device => device.Id == id).FirstOrDefaultAsync();
            var latestDeviceProperties = device.DeviceProperties.LastOrDefault();

            if (latestDeviceProperties == null)
            {
                return BadRequest($"There is no device properties for device with given id: {id}.");
            }

            DevicePropertiesDTO latestDevicePropertiesDTO;
            latestDevicePropertiesDTO = new DevicePropertiesDTO()
            {
                Id = latestDeviceProperties.Id,
                isLiquidLevelSufficient = latestDeviceProperties.isLiquidLevelSufficient,
                Temperature = latestDeviceProperties.Temperature,
                Humidity = latestDeviceProperties.Humidity,
                HeatIndex = latestDeviceProperties.HeatIndex,
                SoilMoisturePercentage = latestDeviceProperties.SoilMoisturePercentage,
                LEDHexColor = latestDeviceProperties.LEDHexColor,
                LEDBrightness = latestDeviceProperties.LEDBrightness
            };

            return Ok(latestDevicePropertiesDTO);
        }

        // POST: api/devices/{id}/modes
        [HttpPost("{id}/modes")]
        public async Task<ActionResult<ModeDTO>> PostMode(int id, ModeAddDTO mode)
        {
            var device = await _context.Devices.FindAsync(id);

            var newMode = new Mode()
            {
                //isOn domyslnie bedzie true
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
                IsOn = mode.IsOn,
                Temperature = mode.Temperature,
                Humidity = mode.Humidity,
                TwilightHour = mode.TwilightHour,
                HourOfDawn = mode.HourOfDawn
        };

            return CreatedAtAction("GetMode", new { id = modeDTO.Id }, modeDTO);
        }

        // PATCH: api/devices/{id}/deviceProperties
        [HttpPatch("{id}/deviceProperties")]
        public async Task<IActionResult> UpdateDeviceProperties(int id, DevicePropertiesDTO deviceProperties)
        {
            var device = await _context.Devices.FindAsync(id);

            var newDeviceProperties = new DeviceProperties()
            {
                isLiquidLevelSufficient = deviceProperties.isLiquidLevelSufficient,
                Temperature = deviceProperties.Temperature,
                Humidity = deviceProperties.Humidity,
                HeatIndex = deviceProperties.HeatIndex,
                SoilMoisturePercentage = deviceProperties.SoilMoisturePercentage,
                LEDHexColor = deviceProperties.LEDHexColor,
                LEDBrightness = deviceProperties.LEDBrightness,
                Device = device,
                DeviceId = id
            };

            await _context.DeviceProperties.AddAsync(newDeviceProperties);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAllDeviceProperties", new { id = device.Id }, device);
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
