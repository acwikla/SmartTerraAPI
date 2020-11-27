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

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDoneProperty(int id, ModeIsOnDTO modeIsOn)
        {
            var modeToUpdate = await _context.Modes.Include(d => d.Device).Where(mode => mode.Id == id).FirstOrDefaultAsync();
            if (modeToUpdate == null)
            {
                return BadRequest($"There is no mode for given id: {id}.");
            }

            modeToUpdate.isOn = modeIsOn.isOn;

            _context.Entry(modeToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"Successfully changed Done property to: {modeToUpdate.isOn}");
        }

        // PUT: api/Modes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMode(int id, ModeDTO mode)
        {
            /*if (id != mode.Id)
            {
                return BadRequest();
            }*/

            var modeToUpdate = await _context.Modes.FindAsync(id);

            if (modeToUpdate == null)
            {
                return BadRequest($"There is no mode for given id: {id}.");
            }

            modeToUpdate.Name = mode.Name;
            modeToUpdate.Temperature = mode.Temperature;
            modeToUpdate.Humidity = mode.Humidity;
            modeToUpdate.TwilightHour = mode.TwilightHour;
            modeToUpdate.HourOfDawn = mode.HourOfDawn;

            _context.Entry(modeToUpdate).State = EntityState.Modified;

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

            return Ok("Mode successfully deleted.");
        }

        private bool ModeExists(int id)
        {
            return _context.Modes.Any(e => e.Id == id);
        }
    }
}
