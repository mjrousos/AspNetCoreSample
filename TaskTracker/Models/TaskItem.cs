using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskList.Models
{
    public class TaskItem
    {
        public TaskItem()
        {
            Created = DateTime.UtcNow;
            CategoryJoins = new List<TaskItemXTaskCategory>();
        }

        // By convention, a property called 'Id' will
        // be used as the primary key if no key is specified
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Complete { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }

        // Many-to-many relationships require an explicit join table
        public virtual List<TaskItemXTaskCategory> CategoryJoins { get; set; }

        // Many-to-many helper property (not mapped)
        public IEnumerable<TaskCategory> Categories => CategoryJoins.Select(j => j.Category).Distinct();

        // TODO : In the future, once we add users
        //        we can store users with access to the task
    }
}
