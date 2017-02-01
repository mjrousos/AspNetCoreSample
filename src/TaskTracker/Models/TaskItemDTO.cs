using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskList.Models
{
    public class TaskItemDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime Created { get; set; }
        public bool Complete => Completed.HasValue;

        // This could very easily be a Dictionary<Guid, string>, instead
        // to avoid needing the small TaskItemDTOCategory nested type,
        // but I prefer the look of the JSON [] this way.
        public IEnumerable<TaskItemDTOCategory> Categories {get;set;}

        public class TaskItemDTOCategory
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }
    }
}
