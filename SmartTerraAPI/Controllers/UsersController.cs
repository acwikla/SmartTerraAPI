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
                var userDTO= new UserDTO()
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
                return BadRequest($"There is no user for given id: {id}.");
            }

            var userDTO = new UserDTO()
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
                return BadRequest($"There is no device for user with given id: {id}.");
            }

            DeviceDTO userDeviceDTO;
            foreach (Device d in userDevices)
            {
                userDeviceDTO = new DeviceDTO()
                {
                    Id = d.Id,
                    Name = d.Name
                };
                userDevicesDTO.Add(userDeviceDTO);
            }

            return Ok(userDevicesDTO);
        }

        // GET: api/Users/1/Devices/1
        [HttpGet("{id}/devices/{deviceId}")]
        public async Task<ActionResult<DeviceDTO>> GetDevice(int id, int deviceId)
        {
            var userDevices = await _context.Users.Include(n => n.Devices).Where(user => user.Id == id).SelectMany(user => user.Devices).ToListAsync();
            Device device = userDevices.Where(device => device.Id == deviceId).FirstOrDefault();

            if (device == null)
            {
                return BadRequest($"There is no device for given id: {deviceId}.");
            }

            var deviceDTO = new DeviceDTO()
            {
                Id = device.Id,
                Name = device.Name,
            };

            return Ok(deviceDTO);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserRegisterDTO user)
        {
            /*if (id != user.Id)
            {
                return BadRequest();
            }*/

            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                return BadRequest($"There is no user for given id: {id}.");
            }

            userToUpdate.Login = user.Login;
            userToUpdate.Email = user.Email;
            userToUpdate.Password = user.Password;

            _context.Entry(userToUpdate).State = EntityState.Modified;

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
                return BadRequest($"User with this email already exists.");
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

        // POST: api/users/5/devices
        [HttpPost("{id}/devices")]
        public async Task<ActionResult<DeviceDTO>> PostDevice(int id, DeviceAddDTO device)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                return BadRequest($"There is no user for given id: {id}.");
            }

            var newDevice = new Device()
            {
                Name = device.Name,
                User = user
            };

            await _context.Devices.AddAsync(newDevice);
            await _context.SaveChangesAsync();

            var deviceDTO = new DeviceDTO()
            {
                Id = newDevice.Id,
                Name = device.Name
            };

            return CreatedAtAction("GetDevice", "Devices", new { id = deviceDTO.Id }, deviceDTO);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest($"There is no user for given id: {id}.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User successfully deleted.");
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
