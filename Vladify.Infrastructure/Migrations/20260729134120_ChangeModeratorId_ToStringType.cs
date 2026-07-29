using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vladify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeModeratorId_ToStringType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssignedModeratorId",
                table: "ModerationTasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedModeratorId",
                table: "ModerationTasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
