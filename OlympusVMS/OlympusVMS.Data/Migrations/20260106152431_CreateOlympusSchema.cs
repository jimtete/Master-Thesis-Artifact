using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OlympusVMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateOlympusSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "Olympus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
