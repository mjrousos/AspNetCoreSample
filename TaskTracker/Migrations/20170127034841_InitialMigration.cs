using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTracker.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Complete = table.Column<bool>(nullable: false),
                    Completed = table.Column<DateTime>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskItemXTaskCategory",
                columns: table => new
                {
                    TaskItemId = table.Column<Guid>(nullable: false),
                    TaskCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItemXTaskCategory", x => new { x.TaskItemId, x.TaskCategoryId });
                    table.ForeignKey(
                        name: "FK_TaskItemXTaskCategory_TaskCategory_TaskCategoryId",
                        column: x => x.TaskCategoryId,
                        principalTable: "TaskCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskItemXTaskCategory_TaskItem_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "TaskItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItemXTaskCategory_TaskCategoryId",
                table: "TaskItemXTaskCategory",
                column: "TaskCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItemXTaskCategory");

            migrationBuilder.DropTable(
                name: "TaskCategory");

            migrationBuilder.DropTable(
                name: "TaskItem");
        }
    }
}
