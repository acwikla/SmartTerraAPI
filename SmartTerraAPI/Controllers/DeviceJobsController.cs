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

        // GET: api/DeviceJobs/deviceId={deviceId}
        [HttpGet("deviceId={deviceId}")]
        public async Task<ActionResult<IEnumerable<DeviceJobDTO>>> GetDeviceJobs(int deviceId)
        {
            List<DeviceJob> deviceJobs = await _context.DeviceJobs.Include(d => d.Device).Include(j => j.Job).Where(deviceJob => deviceJob.Device.Id == deviceId).ToListAsync();
            if (deviceJobs==null)
            {
                return BadRequest($"There is no deviceJob for device with given id: {deviceId}.");
            }
            List<DeviceJobDTO> deviceJobsDTO = new List<DeviceJobDTO>();

            foreach (DeviceJob d in deviceJobs)
            {
                var deviceDTO = new DeviceAddDTO
                {
                    Name = d.Device.Name
                };

                var jobDTO = new JobDTO()
                {
                    Id = d.Job.Id,
                    Name = d.Job.Name,
                    Type = d.Job.Type,
                    Description = d.Job.Description
                };

                var deviceJobDTO = new DeviceJobDTO()
                {
                    Id = d.Id,
                    ExecutionTime = d.ExecutionTime,
                    CreatedDate = d.CreatedDate,
                    Done = d.Done,
                    Body = d.Body,
                    Device = deviceDTO,
                    Job = jobDTO
                };
                deviceJobsDTO.Add(deviceJobDTO);
            }

            return Ok(deviceJobsDTO);
        }

        // GET: api/DeviceJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceJobDTO>> GetDeviceJob(int id)
        {
            var deviceJob = await _context.DeviceJobs.Include(d => d.Device).Include(j => j.Job).Where(deviceJobs => deviceJobs.Id == id).FirstOrDefaultAsync();

            if (deviceJob == null)
            {
                return BadRequest($"There is no deviceJob for given id: {id}.");
            }

            if (deviceJob.Device == null)
            {
                return BadRequest($"There is no device for deviceJob with given id: {id}.");
            }
            var deviceDTO = new DeviceAddDTO
            {
                Name = deviceJob.Device.Name
            };

            if (deviceJob.Job == null)
            {
                return BadRequest($"There is no job for deviceJob with given id: {id}.");
            }
            var jobDTO = new JobDTO()
            {
                Id = deviceJob.Job.Id,
                Name = deviceJob.Job.Name,
                Type = deviceJob.Job.Type,
                Description = deviceJob.Job.Description
            };
            
            var deviceJobDTO = new DeviceJobDTO()
            {
                Id = deviceJob.Id,
                ExecutionTime = deviceJob.ExecutionTime,
                CreatedDate = deviceJob.CreatedDate,
                Done = deviceJob.Done,
                Body = deviceJob.Body,
                Device = deviceDTO,
                Job = jobDTO
            };

            return Ok(deviceJobDTO);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateDoneProperty(int id, JobDoneDTO jobDone)
        {
            var deviceJobToUpdate = await _context.DeviceJobs.Include(d => d.Device).Include(j => j.Job).Where(deviceJobs => deviceJobs.Id == id).FirstOrDefaultAsync();
            if (deviceJobToUpdate == null)
            {
                return BadRequest($"There is no deviceJob for given id: {id}.");
            }

            deviceJobToUpdate.Done = jobDone.Done;

            _context.Entry(deviceJobToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //TODO: change return message to CreatedAtAction and create DTO object(?)
            return Ok($"Successfully changed Done property to: {jobDone.Done}");
        }

        // PUT: api/DeviceJobs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceJob(int id, DeviceJobAddDTO deviceJob)
        {
            /*if (id != deviceJob.Id)
            {
                return BadRequest();
            }*/
            var deviceJobToUpdate = await _context.DeviceJobs.Include(d => d.Device).Where(deviceJobs => deviceJobs.Id == id).FirstOrDefaultAsync();

            if (deviceJobToUpdate == null)
            {
                return BadRequest($"There is no deviceJob for given id: {id}.");
            }

            deviceJobToUpdate.ExecutionTime = deviceJob.ExecutionTime;
            //deviceJobToUpdate.CreatedDate = deviceJob.CreatedDate;
            //deviceJobToUpdate.Done = deviceJob.Done;
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
        public async Task<ActionResult<DeviceJob>> PostDeviceJob(int deviceId, int jobId, DeviceJobAddDTO deviceJob)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device == null)
            {
                return BadRequest($"There is no device for given id: {deviceId}.");
            }
            var deviceDTO = new DeviceAddDTO
            {
                Name = device.Name
            };

            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return BadRequest($"There is no job for given id: {jobId}.");
            }
            var jobDTO = new JobDTO()
            {
                Id = job.Id,
                Name = job.Name,
                Type = job.Type,
                Description = job.Description
            };

            var newDeviceJob = new DeviceJob()
            {
                ExecutionTime = deviceJob.ExecutionTime,
                Body = deviceJob.Body,
                Device = device,
                Job = job
            };

            _context.Entry(device).State = EntityState.Modified;
            _context.Entry(job).State = EntityState.Modified;

            await _context.DeviceJobs.AddAsync(newDeviceJob);
            await _context.SaveChangesAsync();

            var deviceJobDTO = new DeviceJobDTO()
            {
                Id = newDeviceJob.Id,
                ExecutionTime = deviceJob.ExecutionTime,
                CreatedDate = newDeviceJob.CreatedDate,
                Done = newDeviceJob.Done,
                Body = newDeviceJob.Body,
                Device = deviceDTO,
                Job = jobDTO
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
