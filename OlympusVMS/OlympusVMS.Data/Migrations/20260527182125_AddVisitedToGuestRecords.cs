using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OlympusVMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitedToGuestRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visited",
                schema: "Olympus",
                table: "GuestRecord",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visited",
                schema: "Olympus",
                table: "GuestRecord");
        }
    }
}
