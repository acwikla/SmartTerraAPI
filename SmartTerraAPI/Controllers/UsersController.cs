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
    public class UsersController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public UsersController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser()
        {
            var users = await _context.Users.ToListAsync();
            List <UserDTO> usersDTO = new List<UserDTO>();
            foreach (User u in users)
            {
                var userDTO= new UserDTO
                {
                    Id = u.Id,
                    Login = u.Login,
                    Email = u.Email
                };
                usersDTO.Add(userDTO);
            }
            return Ok(usersDTO);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email
            };

            return Ok(userDTO);
        }

        // GET: api/Users/1/devices
        [HttpGet("{id}/devices")]
        public async Task<ActionResult<DeviceDTO>> GetUserDevices(int id)
        {
            var userDevices = await _context.Users.Include(n => n.Devices).Where(user => user.Id == id).SelectMany(user => user.Devices).ToListAsync();
            List <DeviceDTO> userDevicesDTO = new List<DeviceDTO>();

            if (userDevices == null)
            {
                return NotFound();
            }

            DeviceDTO userDeviceDTO;
            foreach (Device d in userDevices)
            {
                userDeviceDTO = new DeviceDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Mode = d.Mode
                };
                userDevicesDTO.Add(userDeviceDTO);
            }

            return Ok(userDevicesDTO);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserRegisterDTO>> PostUser(UserRegisterDTO user)
        {
            if (EmailExists(user.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            var newUser = new User()
            {
                Login = user.Login,
                Email = user.Email,
                Password = user.Password
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var insertedUser = new UserDTO()
            {
                Id = newUser.Id,
                Login = user.Login,
                Email = user.Email,
            };

            return CreatedAtAction("GetUser", new { id = insertedUser.Id }, insertedUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool EmailExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
    }
}
