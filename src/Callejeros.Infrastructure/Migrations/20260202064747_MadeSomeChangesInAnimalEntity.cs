using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeSomeChangesInAnimalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedBreed",
                schema: "adoption",
                table: "Animals",
                newName: "NormalizedLocalization");

            migrationBuilder.RenameColumn(
                name: "Breed",
                schema: "adoption",
                table: "Animals",
                newName: "Localization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedLocalization",
                schema: "adoption",
                table: "Animals",
                newName: "NormalizedBreed");

            migrationBuilder.RenameColumn(
                name: "Localization",
                schema: "adoption",
                table: "Animals",
                newName: "Breed");
        }
    }
}
