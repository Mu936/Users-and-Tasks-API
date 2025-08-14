using UsersAndTasksAPI.Models;

namespace UsersAndTasksAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId);
        Task<IEnumerable<TaskItem>> GetExpiredTasksAsync();
        Task<IEnumerable<TaskItem>> GetActiveTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByDateAsync(DateTime date);
        Task<TaskItem> AddTaskAsync(TaskItem task);
        Task UpdateTaskAsync(TaskItem task);
        Task DeleteTaskAsync(int id);
    }
}
