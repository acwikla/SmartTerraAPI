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
    public class DeviceJobsController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public DeviceJobsController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/DeviceJobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceJobDTO>>> GetDeviceJob()
        {
            var deviceJobs = await _context.DeviceJobs.ToListAsync();
            List<DeviceJobDTO> deviceJobsDTO = new List<DeviceJobDTO>();

            foreach (DeviceJob d in deviceJobs)
            {
                var deviceJobDTO = new DeviceJobDTO()
                {
                    Id = d.Id,
                    ExecutionTime = d.ExecutionTime,
                    CreatedDate = d.CreatedDate,
                    Done = d.Done,
                    Device = null,//TODO: add(find) Device and Job from url
                    Job = null
                };
                deviceJobsDTO.Add(deviceJobDTO);
            }

            return Ok(deviceJobsDTO);
        }

        // GET: api/DeviceJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceJobDTO>> GetDeviceJob(int id)
        {
            var deviceJob = await _context.DeviceJobs.FindAsync(id);

            if (deviceJob == null)
            {
                return NotFound("DeviceJob does not exist.");
            }

            var deviceJobDTO = new DeviceJobDTO()
            {
                Id = deviceJob.Id,
                ExecutionTime = deviceJob.ExecutionTime,
                CreatedDate = deviceJob.CreatedDate,
                Done = deviceJob.Done,
                Device = null,//TODO: add(find) Device and Job from url
                Job = null
            };

            return Ok(deviceJobDTO);
        }

        // PUT: api/DeviceJobs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceJob(int id, DeviceJobDTO deviceJob)
        {
            /*if (id != deviceJob.Id)
            {
                return BadRequest();
            }*/
            var deviceJobToUpdate = await _context.DeviceJobs.FindAsync(id);

            if (deviceJobToUpdate == null)
            {
                return NotFound("DeviceJob does not exist.");
            }

            deviceJobToUpdate.ExecutionTime = deviceJob.ExecutionTime;
            deviceJobToUpdate.CreatedDate = deviceJob.CreatedDate;
            deviceJobToUpdate.Done = deviceJob.Done;
            //TODO: update device & job

            _context.Entry(deviceJobToUpdate).State = EntityState.Modified;

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
        [HttpPost]
        public async Task<ActionResult<DeviceJob>> PostDeviceJob(DeviceJobDTO deviceJob)
        {
            var newDeviceJob = new DeviceJob()
            {
                ExecutionTime = deviceJob.ExecutionTime,
                Device = null,//TODO: add(find) Device and Job from url & set DeviceJobs attribute for Job and Device 
                Job = null
            };

            await _context.DeviceJobs.AddAsync(newDeviceJob);
            await _context.SaveChangesAsync();

            var deviceJobDTO = new DeviceJobDTO()
            {
                Id = newDeviceJob.Id,
                ExecutionTime = deviceJob.ExecutionTime,
                CreatedDate = newDeviceJob.CreatedDate,
                Done = newDeviceJob.Done,
                Device = newDeviceJob.Device,
                Job = newDeviceJob.Job
            };

            return CreatedAtAction("GetDeviceJob", new { id = deviceJobDTO.Id }, deviceJobDTO);
        }

        // DELETE: api/DeviceJobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeviceJob>> DeleteDeviceJob(int id)
        {
            var deviceJob = await _context.DeviceJobs.FindAsync(id);
            if (deviceJob == null)
            {
                return NotFound();
            }

            _context.DeviceJobs.Remove(deviceJob);
            await _context.SaveChangesAsync();

            return Ok("DeviceJob successfully deleted.");
        }

        private bool DeviceJobExists(int id)
        {
            return _context.DeviceJobs.Any(e => e.Id == id);
        }
    }
}
