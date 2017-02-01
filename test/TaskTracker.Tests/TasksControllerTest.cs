using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskList.Controllers;
using TaskList.Models;
using Xunit;

namespace TaskTracker.Controllers
{
    public class TasksControllerTest
    {
        private readonly IServiceProvider ServiceProvider;

        public TasksControllerTest()
        {
            var services = new ServiceCollection();
            services.AddDbContext<TasksContext>(options => options.UseInMemoryDatabase());

            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<TaskCategory, TaskItemDTO.TaskItemDTOCategory>().ConvertUsing(category =>
                {
                    return new TaskItemDTO.TaskItemDTOCategory { Id = category.Id, Title = category.Title };
                });
                cfg.CreateMap<TaskItem, TaskItemDTO>();
            });

            services.AddTransient<TasksController>();

            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Details_Positive() 
        {
            // Arrange
            var dbContext = ServiceProvider.GetRequiredService<TasksContext>();
            await InitializeDbContext(dbContext);

            var controller = ServiceProvider.GetRequiredService<TasksController>();

            // Act
            var result = await controller.Details(Guid.Parse("00000000-1111-2222-3333-444444444444"));

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var task = Assert.IsType<TaskItemDTO>(jsonResult.Value);
            Assert.Equal(task.Title, "Dishes");
        }

        private async Task InitializeDbContext(TasksContext dbContext)
        {
            dbContext.TaskCategories.Add(new TaskCategory { Id = Guid.Parse("67d17cb1-60d9-4494-85c8-042194c756ba"), Title = "Chores" });
            await dbContext.SaveChangesAsync();

            var newTask = new TaskItem { Id = Guid.Parse("00000000-1111-2222-3333-444444444444"), Title = "Dishes" };
            dbContext.Tasks.Add(newTask);

            var choresCategory = dbContext.TaskCategories.Where(c => c.Title.Equals("Chores", StringComparison.Ordinal)).FirstOrDefault();
            if (choresCategory != null)
            {
                dbContext.TaskCategoryJoins.Add(new TaskItemXTaskCategory { Category = choresCategory, Task = newTask });
            }
            await dbContext.SaveChangesAsync();
        }
        
    }
}
