using Microsoft.EntityFrameworkCore;
using System;

namespace TaskList.Models
{
    public class TasksContext: DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<TaskItemXTaskCategory> TaskCategoryJoins { get; set; }

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

            // Setup many-to-many relationships by specifying relationships
            // on the join entity and specifying a primary key.
            modelBuilder.Entity<TaskItemXTaskCategory>(entity =>
            {
                // Primary key (just a composite of the two foreign keys)
                entity.HasKey(e => new { e.TaskItemId, e.TaskCategoryId });

                // The relationship one way out of the join
                entity.HasOne(e => e.Task)
                    .WithMany(t => t.CategoryJoins)
                    .HasForeignKey(e => e.TaskItemId);

                // The relationship the other way out of the join
                entity.HasOne(e => e.Category)
                    .WithMany(t => t.TaskJoins)
                    .HasForeignKey(e => e.TaskCategoryId);
            });
        }
    }
}
