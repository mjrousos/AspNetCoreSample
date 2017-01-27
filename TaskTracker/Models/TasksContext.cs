using Microsoft.EntityFrameworkCore;
using System;

namespace TaskList.Models
{
    public class TasksContext: DbContext
    {
        public DbSet<TaskItem> Tasks;
        public DbSet<TaskCategory> TaskCategories;

        public TasksContext(DbContextOptions<TasksContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Modify the model here, as necessary
            // Many of these configurations could also be made
            // via attributes on the model types.
            // https://docs.microsoft.com/en-us/ef/core/modeling/
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TaskCategory>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
