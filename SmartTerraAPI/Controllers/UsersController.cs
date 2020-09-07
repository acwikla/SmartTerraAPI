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
            //var modes = _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).SelectMany(user => user.Modes).ToList();
            //modes.ToList().Add(mode);

            await _context.Modes.AddAsync(mode);
            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            mode.User = user;
            //await _context.Users.Update(user);
            //_context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return mode;
        }
        
        //PUT: api/users/{userId}/modes/1
        [HttpPut("{userId}/modes/{id}")]
        public async Task<ActionResult<IEnumerable<Mode>>> PutMode([FromRoute] int userId, [FromBody] Mode mode, [FromRoute] int id)
        {
            /*if(id != mode.Id)
            {
                return BadRequest();
            }*/

            mode.Id = id;
            ///var modeTochange = _context.Modes.Where(User => User.Id == userId).ToList().Select(m => m.Id == id);
            _context.Modes.Update(mode);
            var user = await _context.Users.Include(n => n.Modes).Where(user => user.Id == userId).FirstOrDefaultAsync();
            mode.User = user;
            //_context.Entry(user).State = EntityState.Modified;

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

            return Ok(user.Modes);
        }
        private bool ModeExists(int id)
        {
            return _context.Modes.Any(e => e.Id == id);
        }

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