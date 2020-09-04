using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SmartTerraAPI.Models;

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

        //POST: api/users
        [HttpPost]
        public async Task<ActionResult<IEnumerable<User>>> PostUser(User user)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            user.Modes = new List<Mode>();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        //GET: api/users/{userId}
        [HttpGet("{userId}")]
        public User GetUser([FromRoute] int userId)
        {
            //User user = await _context.Users.FindAsync(userId); //modes= null
            //User user = _context.Users.Find(userId);// modes=> modes local= empty , modes results view= {...}  users=> modes local = null, modes results view = null
            var user = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefault();
            return user;
        }

        //GET: api/users/{userId}/modes
        [HttpGet("{userId}/modes")]
        public IEnumerable<Mode> GetModes([FromRoute] int userId)
        {
            IEnumerable<Mode> modes = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).SelectMany(user => user.Modes).ToList();
            
            return modes;
        }

        //GET: api/users/{userId}/modes/{id}
        [HttpGet("{userId}/modes/{id}")]
        public async Task<ActionResult<Mode>> GetMode([FromRoute] int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefault();
            var mode = user.Modes.Where(mode => mode.Id == id);
            if ( mode == null)
            {
                return NotFound();
            }
            return Ok(mode);
        }

        //POST: api/users/{userId}/modes
        [HttpPost("{userId}/modes")]
        public async Task<Mode> PostMode([FromRoute] int userId, Mode mode)
        {
            /*var user = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefault();
            mode.User = user;
            _context.Modes.Add(mode);
            user.Modes = modes;*/
            //IEnumerable<Mode> modes = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).SelectMany(user => user.Modes);
            //mode.User = user;
            //modes.ToList().Add(mode);
            //user.Modes = modes;
            //++mode.Id;
            //var modes = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).SelectMany(user => user.Modes).ToList();
            //user.Modes = modes;

            await _context.Modes.AddAsync(mode);
            var user = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefault();
            mode.User = user;
            _context.Users.Update(user);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return mode;
        }
        
        //PUT: api/users/modes/1
        [HttpPut("modes/{id}")]
        public async Task<ActionResult<Mode>> PutMode([FromRoute] int id,[FromBody]Mode mode)
        {
            if(id != mode.Id)
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
        private bool ModeExists(int id)
        {
            return _context.Modes.Any(e => e.Id == id);
        }

        // DELETE: api/users/modes
        [HttpDelete("modes")]
        public async Task<ActionResult<Mode>> DeleteModes()
        {
            _context.Modes.RemoveRange(_context.Modes);
            _context.SaveChanges();
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModes", null);
        }

        // DELETE: api/users/modes/1
        [HttpDelete("modes/{id}")]
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
    }
}