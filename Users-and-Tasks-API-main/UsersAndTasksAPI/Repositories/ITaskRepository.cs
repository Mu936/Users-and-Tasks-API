using UsersAndTasksAPI.Models;

namespace UsersAndTasksAPI.Repositories
{
    public interface ITaskRepository
    {
        /// <summary>
        /// Retrieves all tasks with their assignee information
        /// </summary>
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();

        /// <summary>
        /// Retrieves a specific task by its ID, including assignee information
        /// </summary>
        /// <param name="id">The ID of the task to retrieve</param>
        Task<TaskItem?> GetTaskByIdAsync(int id);

        /// <summary>
        /// Adds a new task to the database
        /// </summary>
        /// <param name="task">The task to add</param>
        Task<TaskItem> AddTaskAsync(TaskItem task);

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="task">The task with updated information</param>
        Task UpdateTaskAsync(TaskItem task);

        /// <summary>
        /// Deletes a task by its ID
        /// </summary>
        /// <param name="id">The ID of the task to delete</param>
        Task DeleteTaskAsync(int id);
        
        /// <summary>
        /// Retrieves all tasks that are expired as of the specified date
        /// </summary>
        /// <param name="currentDate">The date to check for expired tasks</param>
        Task<IEnumerable<TaskItem>> GetExpiredTasksAsync(DateTime currentDate);

        /// <summary>
        /// Retrieves all tasks that are active as of the specified date
        /// </summary>
        /// <param name="currentDate">The date to check for active tasks</param>
        Task<IEnumerable<TaskItem>> GetActiveTasksAsync(DateTime currentDate);

        /// <summary>
        /// Retrieves tasks within a specified date range
        /// </summary>
        /// <param name="startDate">The start of the date range (inclusive)</param>
        /// <param name="endDate">The end of the date range (inclusive), or null to get tasks for start date only</param>
        Task<IEnumerable<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime? endDate = null);

        /// <summary>
        /// Retrieves all tasks assigned to a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId);
    }
}
