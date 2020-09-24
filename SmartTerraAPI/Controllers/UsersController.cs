using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SmartTerraAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace SmartTerraAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private SmartTerraDBContext _context;

        public UsersController(SmartTerraDBContext context)
        {
            _context = context;
        }

        [Authorize]
        //GET: api/users/{userId}
        [HttpGet("{userId}")]
        public User GetUser([FromRoute] int userId)
        {
            //User user = await _context.Users.FindAsync(userId); //modes= null
            //User user = _context.Users.Find(userId);// modes=> modes local= empty , modes results view= {...}  users=> modes local = null, modes results view = null
            var user = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefault();
            return user;
        }

        [Authorize]
        //GET: api/users/{userId}/modes
        [HttpGet("{userId}/modes")]
        public IEnumerable<Mode> GetModes([FromRoute] int userId)
        {
            IEnumerable<Mode> modes = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).SelectMany(user => user.Modes).ToList();
            
            return modes;
        }

        [Authorize]
        //GET: api/users/{userId}/modes/{id}
        [HttpGet("{userId}/modes/{id}")]
        public async Task<ActionResult<Mode>> GetMode([FromRoute] int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            var mode = user.Modes.Where(mode => mode.Id == id);
            if ( mode == null)
            {
                return NotFound();
            }
            return Ok(mode);
        }

        [Authorize]
        //POST: api/users/{userId}/modes
        [HttpPost("{userId}/modes")]
        public async Task<Mode> PostMode([FromRoute] int userId, Mode mode)
        {
            await _context.Modes.AddAsync(mode);
            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            mode.User = user;
            //await _context.Users.Update(user);
            //_context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return mode;
        }

        [Authorize]
        //PUT: api/users/{userId}/modes/1
        [HttpPut("{userId}/modes/{id}")]
        public async Task<ActionResult<IEnumerable<Mode>>> PutMode([FromRoute] int userId, [FromBody] Mode mode, [FromRoute] int id)
        {

            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            mode.User = user;

            var modeToUpdate = await _context.Modes.FindAsync(id);
            
            if (modeToUpdate == null)
            {
                return NotFound();
            }

            modeToUpdate.Title = mode.Title;
            modeToUpdate.Id = id;
            modeToUpdate.Temperature = mode.Temperature;
            modeToUpdate.Humidity = mode.Humidity;
            modeToUpdate.HeatIndex = mode.HeatIndex;
            modeToUpdate.Brightness = mode.Brightness;
            modeToUpdate.User = mode.User;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!ModeExists(id))
            {
                return NotFound();
            }

            return Ok(user.Modes);
        }
        private bool ModeExists(int id)
        {
            return _context.Modes.Any(e => e.Id == id);
        }

        [Authorize]
        // DELETE: api/users/{userId}/modes
        [HttpDelete("{userId}/modes")]
        public async Task<ActionResult<Mode>> DeleteModes([FromRoute] int userId)
        {
            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (user.Modes == null)
            {
                return NotFound();
            }
            _context.Modes.RemoveRange(user.Modes);

            foreach (Mode mode in user.Modes) 
            {
                user.Modes.ToList().Remove(mode);
            }
                
            await _context.SaveChangesAsync();

            return Ok(user.Modes);
        }

        [Authorize]
        // DELETE: api/users/{userId}/modes/1
        [HttpDelete("{userId}/modes/{id}")]
        public async Task<ActionResult<Mode>> DeleteMode([FromRoute] int userId, int id)
        {
            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            //var modes = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).SelectMany(user => user.Modes).ToListAsync();

            var mode = await _context.Modes.FindAsync(id);
            if (mode == null)
            {
                return NotFound();
            }
            //modes.Remove(mode);
            _context.Modes.Remove(mode);
            await _context.SaveChangesAsync();

            return Ok(user.Modes);
        }

    }
}