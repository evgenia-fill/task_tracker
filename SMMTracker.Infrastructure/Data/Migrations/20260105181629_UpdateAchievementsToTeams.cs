using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMMTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAchievementsToTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "IconClass", "Title" },
                values: new object[] { "Вступите в свою первую команду", "bi-people", "Новичок" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Вступите в 5 команд", "bi-people-fill", 5, "Командный игрок" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "IconClass", "TasksThreshold" },
                values: new object[] { "Вступите в 10 команд", "bi-person-check", 10 });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Вступите в 20 команд", "bi-person-hearts", 20, "Душа компании" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Вступите в 50 команд", "bi-megaphone", 50, "Легенда сообщества" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Вступите в 100 команд", "bi-award", 100, "Друг Пьянзиной" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "IconClass", "Title" },
                values: new object[] { "Выполните свою первую задачу", "bi-check-circle", "Первые шаги" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Выполните 10 задач", "bi-star", 10, "На опыте" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "IconClass", "TasksThreshold" },
                values: new object[] { "Выполните 50 задач", "bi-gem", 50 });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Выполните 100 задач", "bi-trophy", 100, "Мастер" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Выполните 200 задач", "bi-crown", 200, "Элита" });

            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "IconClass", "TasksThreshold", "Title" },
                values: new object[] { "Выполните 300 задач", "bi-rocket-takeoff", 300, "Легенда" });
        }
    }
}
