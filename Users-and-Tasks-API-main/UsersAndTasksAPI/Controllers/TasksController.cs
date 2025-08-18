using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UsersAndTasksAPI.Models;
using UsersAndTasksAPI.Repositories;

namespace UsersAndTasksAPI.Controllers
{
    /// <summary>
    /// Controller for managing tasks
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepo;

        public TasksController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <returns>List of all tasks</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskItem>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll() => Ok(await _taskRepo.GetAllTasksAsync());

        /// <summary>
        /// Get active tasks (not yet due)
        /// </summary>
        /// <returns>List of active tasks</returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskItem>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActive()
        {
            return Ok(await _taskRepo.GetActiveTasksAsync(DateTime.UtcNow));
        }

        /// <summary>
        /// Get expired tasks
        /// </summary>
        /// <returns>List of expired tasks</returns>
        [HttpGet("expired")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskItem>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetExpired()
        {
            return Ok(await _taskRepo.GetExpiredTasksAsync(DateTime.UtcNow));
        }

        /// <summary>
        /// Get tasks by date range
        /// </summary>
        /// <param name="startDate">Start date (inclusive)</param>
        /// <param name="endDate">End date (inclusive, optional - if not provided, returns tasks for the start date only)</param>
        /// <returns>List of tasks within the date range</returns>
        [HttpGet("by-date")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskItem>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery, Required] DateTime startDate,
            [FromQuery] DateTime? endDate = null)
        {
            if (endDate.HasValue && endDate < startDate)
            {
                return BadRequest("End date must be greater than or equal to start date");
            }

            return Ok(await _taskRepo.GetTasksByDateRangeAsync(startDate, endDate));
        }

        /// <summary>
        /// Get tasks assigned to a specific user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>List of tasks assigned to the user</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskItem))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _taskRepo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpGet("date/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskItem>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByDate(DateTime date) => 
            Ok(await _taskRepo.GetTasksByDateRangeAsync(date, date));

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
