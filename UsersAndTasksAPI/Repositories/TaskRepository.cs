using Microsoft.EntityFrameworkCore;
using UsersAndTasksAPI.Data;
using UsersAndTasksAPI.Models;

namespace UsersAndTasksAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context) => _context = context;

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync() =>
            await _context.Tasks.Include(t => t.Assignee).ToListAsync();

        public async Task<TaskItem?> GetTaskByIdAsync(int id) =>
            await _context.Tasks.Include(t => t.Assignee).FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId) =>
            await _context.Tasks.Include(t => t.Assignee)
                                 .Where(t => t.AssigneeId == userId)
                                 .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetExpiredTasksAsync() =>
            await _context.Tasks.Include(t => t.Assignee)
                                 .Where(t => t.DueDate < DateTime.UtcNow)
                                 .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetActiveTasksAsync() =>
            await _context.Tasks.Include(t => t.Assignee)
                                 .Where(t => t.DueDate >= DateTime.UtcNow)
                                 .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetTasksByDateAsync(DateTime date) =>
            await _context.Tasks.Include(t => t.Assignee)
                                 .Where(t => t.DueDate.Date == date.Date)
                                 .ToListAsync();

        public async Task UpdateTaskAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }
}
