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
    public class TaskModelsController : ControllerBase
    {
        private readonly SmartTerraDBContext _context;

        public TaskModelsController(SmartTerraDBContext context)
        {
            _context = context;
        }

        // GET: api/TaskModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        // GET: api/TaskModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskModel(int id)
        {
            var taskModel = await _context.Tasks.FindAsync(id);

            if (taskModel == null)
            {
                return NotFound();
            }

            return taskModel;
        }

        // PUT: api/TaskModels/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskModel(int id, TaskModel taskModel)
        {
            if (id != taskModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskModelExists(id))
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

        // POST: api/TaskModels
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TaskModel>> PostTaskModel(TaskModel taskModel)
        {
            _context.Tasks.Add(taskModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskModel", new { id = taskModel.Id }, taskModel);
        }

        // DELETE: api/TaskModels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskModel>> DeleteTaskModel(int id)
        {
            var taskModel = await _context.Tasks.FindAsync(id);
            if (taskModel == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskModel);
            await _context.SaveChangesAsync();

            return taskModel;
        }

        private bool TaskModelExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
