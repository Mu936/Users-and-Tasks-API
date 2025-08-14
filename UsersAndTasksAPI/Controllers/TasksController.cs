using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UsersAndTasksAPI.Models;
using UsersAndTasksAPI.Repositories;

namespace UsersAndTasksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Optional: Remove if you haven't implemented JWT yet
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepo;

        public TasksController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _taskRepo.GetAllTasksAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _taskRepo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var tasks = await _taskRepo.GetTasksByUserAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("expired")]
        public async Task<IActionResult> GetExpired() => Ok(await _taskRepo.GetExpiredTasksAsync());

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() => Ok(await _taskRepo.GetActiveTasksAsync());

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetByDate(DateTime date) => Ok(await _taskRepo.GetTasksByDateAsync(date));

        [HttpPost]
        public async Task<IActionResult> Create(TaskItem task)
        {
            var createdTask = await _taskRepo.AddTaskAsync(task);
            return CreatedAtAction(nameof(Get), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskItem task)
        {
            if (id != task.Id) return BadRequest();
            await _taskRepo.UpdateTaskAsync(task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskRepo.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
