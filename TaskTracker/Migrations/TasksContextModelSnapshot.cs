using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TaskList.Models;

namespace TaskTracker.Migrations
{
    [DbContext(typeof(TasksContext))]
    partial class TasksContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TaskList.Models.TaskCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("TaskCategory");
                });

            modelBuilder.Entity("TaskList.Models.TaskItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Complete");

                    b.Property<DateTime?>("Completed");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("TaskItem");
                });

            modelBuilder.Entity("TaskList.Models.TaskItemXTaskCategory", b =>
                {
                    b.Property<Guid>("TaskItemId");

                    b.Property<Guid>("TaskCategoryId");

                    b.HasKey("TaskItemId", "TaskCategoryId");

                    b.HasIndex("TaskCategoryId");

                    b.ToTable("TaskItemXTaskCategory");
                });

            modelBuilder.Entity("TaskList.Models.TaskItemXTaskCategory", b =>
                {
                    b.HasOne("TaskList.Models.TaskCategory", "Category")
                        .WithMany("TaskJoins")
                        .HasForeignKey("TaskCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TaskList.Models.TaskItem", "Task")
                        .WithMany("CategoryJoins")
                        .HasForeignKey("TaskItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
