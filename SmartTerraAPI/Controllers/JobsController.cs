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
    public class JobsController : ControllerBase
    {
        private readonly SmartTerraDbContext _context;

        public JobsController(SmartTerraDbContext context)
        {
            _context = context;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobDTO>>> GetJob()
        {
            var jobs = await _context.Jobs.ToListAsync();
            List<JobDTO> jobsDTO = new List<JobDTO>();
            foreach (Job j in jobs)
            {
                var jobDTO = new JobDTO
                {
                    Id = j.Id,
                    Name = j.Name,
                    Type = j.Type,
                    Body = j.Body,
                    Description = j.Description
                };
                jobsDTO.Add(jobDTO);
            }
            return Ok(jobsDTO);
        }

        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobDTO>> GetJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            var jobDTO = new JobDTO
            {
                Id = job.Id,
                Name = job.Name,
                Type = job.Type,
                Body = job.Body,
                Description = job.Description
            };
            return Ok(jobDTO);
        }

        // PUT: api/Job/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
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

        // POST: api/Jobs
        [HttpPost]
        public async Task<ActionResult<JobDTO>> PostJob(Job job)
        {
            //TODO: add DeviceJobs from url data (device id...)

            await _context.Jobs.AddAsync(job);

            var jobDTO = new JobDTO
            {
                Id = job.Id,
                Name = job.Name,
                Type = job.Type,
                Body = job.Body,
                Description = job.Description
            };

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetJob", new { id = jobDTO.Id }, jobDTO);
        }

        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return job;
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
