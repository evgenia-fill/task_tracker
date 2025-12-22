using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMMTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Calendars",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Calendars");
        }
    }
}
