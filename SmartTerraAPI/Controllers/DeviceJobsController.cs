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
    public class DeviceJobsController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public DeviceJobsController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/DeviceJobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceJob>>> GetDeviceJob()
        {
            return await _context.DeviceJob.ToListAsync();
        }

        // GET: api/DeviceJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceJob>> GetDeviceJob(int id)
        {
            var deviceJob = await _context.DeviceJob.FindAsync(id);

            if (deviceJob == null)
            {
                return NotFound();
            }

            return deviceJob;
        }

        // PUT: api/DeviceJobs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceJob(int id, DeviceJob deviceJob)
        {
            if (id != deviceJob.Id)
            {
                return BadRequest();
            }

            _context.Entry(deviceJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceJobExists(id))
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

        // POST: api/DeviceJobs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DeviceJob>> PostDeviceJob(DeviceJob deviceJob)
        {
            _context.DeviceJob.Add(deviceJob);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeviceJob", new { id = deviceJob.Id }, deviceJob);
        }

        // DELETE: api/DeviceJobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeviceJob>> DeleteDeviceJob(int id)
        {
            var deviceJob = await _context.DeviceJob.FindAsync(id);
            if (deviceJob == null)
            {
                return NotFound();
            }

            _context.DeviceJob.Remove(deviceJob);
            await _context.SaveChangesAsync();

            return deviceJob;
        }

        private bool DeviceJobExists(int id)
        {
            return _context.DeviceJob.Any(e => e.Id == id);
        }
    }
}
