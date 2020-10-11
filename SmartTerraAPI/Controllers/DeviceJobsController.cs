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
                    Body = d.Body,
                    Device = d.Device,//TODO: add(find) Device and Job from url
                    Job = d.Job
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
                return BadRequest($"There is no deviceJob for deviceJob with given id: {id}.");
            }

            var device = deviceJob.Device;
            if (device == null)
            {
                return BadRequest($"There is no device for deviceJob with given id: {id}.");
            }

            var job = deviceJob.Job;
            if (job == null)
            {
                return BadRequest($"There is no job for deviceJob with given id: {id}.");
            }
            

            var deviceJobDTO = new DeviceJobDTO()
            {
                Id = deviceJob.Id,
                ExecutionTime = deviceJob.ExecutionTime,
                CreatedDate = deviceJob.CreatedDate,
                Done = deviceJob.Done,
                Body = deviceJob.Body,
                Device = deviceJob.Device,
                Job = deviceJob.Job
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
                return BadRequest($"There is no device job for given id: {id}.");
            }

            deviceJobToUpdate.ExecutionTime = deviceJob.ExecutionTime;
            deviceJobToUpdate.CreatedDate = deviceJob.CreatedDate;
            deviceJobToUpdate.Done = deviceJob.Done;
            deviceJobToUpdate.Body = deviceJob.Body;

            var deviceToUpdate = deviceJobToUpdate.Device;

            _context.Entry(deviceToUpdate).State = EntityState.Modified;
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

        // POST: api/DeviceJobs/deviceId={deviceId}/jobId={jobId}
        [HttpPost("deviceId={deviceId}/jobId={jobId}")]
        public async Task<ActionResult<DeviceJob>> PostDeviceJob(int deviceId, int jobId, DeviceJobDTO deviceJob)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
            {
                return BadRequest($"There is no device for given id: {deviceId}.");
            }

            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return BadRequest($"There is no job for given id: {jobId}.");
            }

            var newDeviceJob = new DeviceJob()
            {
                ExecutionTime = deviceJob.ExecutionTime,
                Body = deviceJob.Body,
                Device = device,
                Job = job
            };

            await _context.DeviceJobs.AddAsync(newDeviceJob);
            await _context.SaveChangesAsync();//exeption => 'cannot insert value null'

            _context.Entry(device).State = EntityState.Modified;
            _context.Entry(job).State = EntityState.Modified;
            var deviceJobDTO = new DeviceJobDTO()
            {
                Id = newDeviceJob.Id,
                ExecutionTime = deviceJob.ExecutionTime,
                CreatedDate = newDeviceJob.CreatedDate,
                Done = newDeviceJob.Done,
                Body = newDeviceJob.Body,
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
