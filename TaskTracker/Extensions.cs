using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskList.Models;

namespace TaskList
{
    internal static class Extensions
    {
        public static void Seed(this TasksContext dbContext)
        {
            if (!dbContext.TaskCategories.Any())
            {
                Log.Information("Seeding task categories");
                dbContext.TaskCategories.Add(new TaskCategory { Title = "Chores" });
                dbContext.SaveChanges();
            }

            if (!dbContext.Tasks.Any())
            {
                Log.Information("Seeding tasks");
                var newTask = new TaskItem { Title = "Dishes" };
                dbContext.Tasks.Add(newTask);

                var choresCategory = dbContext.TaskCategories.Where(c => c.Title.Equals("Chores", StringComparison.Ordinal)).FirstOrDefault();
                if (choresCategory != null)
                {
                    dbContext.TaskCategoryJoins.Add(new TaskItemXTaskCategory { Category = choresCategory, Task = newTask });
                }
                dbContext.SaveChanges();
            }
        }
    }
}
