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
    public class DevicePropertiesController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public DevicePropertiesController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/DeviceProperties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceProperties>> GetDeviceProperties(int id)
        {
            var device = await _context.Devices.Include(d => d.DeviceProperties).Where(device => device.Id == id).FirstOrDefaultAsync();

            if (device.DeviceProperties == null)
            {
                return BadRequest($"There is no device roperties for given id: {id}.");
            }

            var deviceProperties = device.DeviceProperties;

            var devicePropertiesDTO = new DevicePropertiesDTO()
            {
                Id = deviceProperties.Id,
                isLiquidLevelSufficient = deviceProperties.isLiquidLevelSufficient,
                Temperature = deviceProperties.Temperature,
                Humidity = deviceProperties.Humidity,
                HeatIndex = deviceProperties.HeatIndex,
                SoilMoisturePercentage = deviceProperties.SoilMoisturePercentage,
                LEDHexColor = deviceProperties.LEDHexColor,
                LEDBrightness = deviceProperties.LEDBrightness
            };

            return Ok(devicePropertiesDTO);
        }

        [HttpPatch("{id}/LiquidLevel")]
        public async Task<IActionResult> UpdateLiquidLevelProperty(int id, DeviceLiquidLevelDTO deviceLquidLevelData)
        {
            var devicePropertiesToUpdate = await _context.DeviceProperties.Include(d => d.Device).Where(DeviceProperties => DeviceProperties.Id == id).FirstOrDefaultAsync();
            if (devicePropertiesToUpdate == null)
            {
                return BadRequest($"There is no device properties for given id: {id}.");
            }

            devicePropertiesToUpdate.isLiquidLevelSufficient = deviceLquidLevelData.isLiquidLevelSufficient;

            _context.Entry(devicePropertiesToUpdate).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok($"Successfully changed waterLevel property to: {deviceLquidLevelData.isLiquidLevelSufficient}");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDeviceProperties(int id, DevicePropertiesDTO newDeviceProperties)
        {
            var devicePropertiesToUpdate = await _context.DeviceProperties.Include(d => d.Device).Where(DeviceProperties => DeviceProperties.Id == id).FirstOrDefaultAsync();
            if (devicePropertiesToUpdate == null)
            {
                return BadRequest($"There is no device properties for given id: {id}.");
            }

            devicePropertiesToUpdate.isLiquidLevelSufficient = newDeviceProperties.isLiquidLevelSufficient;
            devicePropertiesToUpdate.Temperature = newDeviceProperties.Temperature;
            devicePropertiesToUpdate.Humidity = newDeviceProperties.Humidity;
            devicePropertiesToUpdate.HeatIndex = newDeviceProperties.HeatIndex;
            devicePropertiesToUpdate.SoilMoisturePercentage = newDeviceProperties.SoilMoisturePercentage;
            devicePropertiesToUpdate.LEDHexColor = newDeviceProperties.LEDHexColor;
            devicePropertiesToUpdate.LEDBrightness = newDeviceProperties.LEDBrightness;

            _context.Entry(devicePropertiesToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"Successfully changed device properties.");
        }

        // POST: api/DeviceProperties
        [HttpPost]
        public async Task<ActionResult<DeviceProperties>> PostDeviceProperties(DeviceProperties deviceProperties)
        {
            await _context.DeviceProperties.AddAsync(deviceProperties);
            await _context.SaveChangesAsync();

            var newDevicePropertiesDTO = new DevicePropertiesDTO()
            {
                Id = deviceProperties.Id,
                isLiquidLevelSufficient = deviceProperties.isLiquidLevelSufficient,
                Temperature = deviceProperties.Temperature,
                Humidity = deviceProperties.Humidity,
                HeatIndex = deviceProperties.HeatIndex,
                SoilMoisturePercentage = deviceProperties.SoilMoisturePercentage,
                LEDHexColor = deviceProperties.LEDHexColor,
                LEDBrightness = deviceProperties.LEDBrightness
            };

            return CreatedAtAction("GetDeviceProperties", new { id = newDevicePropertiesDTO.Id }, newDevicePropertiesDTO);
        }

        // DELETE: api/DeviceProperties/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeviceProperties>> DeleteDeviceProperties(int id)
        {
            var deviceProperties = await _context.DeviceProperties.FindAsync(id);
            if (deviceProperties == null)
            {
                return NotFound();
            }

            _context.DeviceProperties.Remove(deviceProperties);
            await _context.SaveChangesAsync();

            return deviceProperties;
        }
    }
}
