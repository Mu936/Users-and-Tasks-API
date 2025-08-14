namespace UsersAndTasksAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AssigneeId { get; set; }
        public User? Assignee { get; set; }
        public DateTime DueDate { get; set; }
    }
}
