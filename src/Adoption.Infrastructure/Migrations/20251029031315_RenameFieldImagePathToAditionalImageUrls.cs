using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameFieldImagePathToAditionalImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagesPath",
                schema: "adoption",
                table: "Animals",
                newName: "AdditionalImagesUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdditionalImagesUrl",
                schema: "adoption",
                table: "Animals",
                newName: "ImagesPath");
        }
    }
}
