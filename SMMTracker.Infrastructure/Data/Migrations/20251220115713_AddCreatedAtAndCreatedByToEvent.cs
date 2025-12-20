using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMMTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtAndCreatedByToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: DateTime.UtcNow); // Можно оставить DateTime.UtcNow по умолчанию

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Events",
                type: "INTEGER",
                nullable: true); // теперь nullable, как в сущности
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Events");
        }

    }
}
