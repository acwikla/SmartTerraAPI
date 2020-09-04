using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTerraAPI.Models;

namespace SmartTerraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModesController : ControllerBase
    {
        private readonly SmartTerraDBContext _context;

        public ModesController(SmartTerraDBContext context)
        {
            _context = context;
        }

        // GET: api/Modes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mode>>> GetModes()
        {
            return await _context.Modes.ToListAsync();
        }

        // GET: api/Modes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mode>> GetMode(int id)
        {
            var mode = await _context.Modes.FindAsync(id);

            if (mode == null)
            {
                return NotFound();
            }

            return mode;
        }

        // PUT: api/Modes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMode(int id, Mode mode)
        {
            if (id != mode.ModeId)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Mode>> PostMode(Mode mode)
        {
            _context.Modes.Add(mode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMode", new { id = mode.ModeId }, mode);
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
            return _context.Modes.Any(e => e.ModeId == id);
        }
    }
}
