using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskList.Models
{
    public class TaskItemXTaskCategory
    {
        public Guid TaskItemId { get; set; }
        public virtual TaskItem Task { get; set; }

        public Guid TaskCategoryId { get; set; }
        public virtual TaskCategory Category { get; set; }
    }
}
