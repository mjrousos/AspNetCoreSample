using System;

namespace TaskList.Models
{
    public class TaskItem
    {
        public TaskItem()
        {
            Created = DateTime.UtcNow;
        }

        // By convention, a property called 'Id' will
        // be used as the primary key if no key is specified
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Complete { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public Guid? TaskCategoryId { get; set; }

        public virtual TaskCategory Category { get; set; }

        // TODO : In the future, once we add users
        //        we can store users with access to the task
    }
}
