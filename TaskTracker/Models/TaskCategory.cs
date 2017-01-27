using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskList.Models
{
    public class TaskCategory
    {
        public TaskCategory()
        {
            Created = DateTime.UtcNow;
            TaskJoins = new List<TaskItemXTaskCategory>();
        }

        // By convention, a property called 'Id' will
        // be used as the primary key if no key is specified
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

        // Many-to-many relationships require an explicit join table
        public virtual List<TaskItemXTaskCategory> TaskJoins { get; set; }

        // Many-to-many helper property (not mapped)
        public IEnumerable<TaskItem> Tasks => TaskJoins.Select(j => j.Task);

        // TODO : In the future, once we add users
        //        we can store users with access to the task category
    }
}
