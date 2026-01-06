using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OlympusVMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateGuestRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Olympus");

            migrationBuilder.CreateTable(
                name: "GuestRecord",
                schema: "Olympus",
                columns: table => new
                {
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    RegisteredBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeetingTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MeetingDurationInHours = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestRecord", x => x.RecordId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuestRecord_EmailAddress",
                schema: "Olympus",
                table: "GuestRecord",
                column: "EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestRecord",
                schema: "Olympus");
        }
    }
}
