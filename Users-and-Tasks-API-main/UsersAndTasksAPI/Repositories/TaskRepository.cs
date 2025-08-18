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

        public async Task<IEnumerable<TaskItem>> GetExpiredTasksAsync(DateTime currentDate)
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Where(t => t.DueDate < currentDate)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetActiveTasksAsync(DateTime currentDate)
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Where(t => t.DueDate >= currentDate)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime? endDate = null)
        {
            var query = _context.Tasks.Include(t => t.Assignee).AsQueryable();
            
            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.DueDate >= startDate.Date && t.DueDate <= endOfDay);
            }
            else
            {
                var endOfDay = startDate.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.DueDate >= startDate.Date && t.DueDate <= endOfDay);
            }

            return await query.OrderBy(t => t.DueDate).ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Assignee)
                .Where(t => t.AssigneeId == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync() =>
            await _context.Tasks.Include(t => t.Assignee).ToListAsync();

        public async Task<TaskItem?> GetTaskByIdAsync(int id) =>
            await _context.Tasks.Include(t => t.Assignee).FirstOrDefaultAsync(t => t.Id == id);

        public async Task UpdateTaskAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }
}
