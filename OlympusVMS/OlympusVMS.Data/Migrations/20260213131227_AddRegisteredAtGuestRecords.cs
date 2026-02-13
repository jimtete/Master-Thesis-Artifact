using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OlympusVMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredAtGuestRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RegisteredAt",
                schema: "Olympus",
                table: "GuestRecord",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                schema: "Olympus",
                table: "GuestRecord");
        }
    }
}
