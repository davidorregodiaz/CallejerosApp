using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnimalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimalType_Value",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Breed_Value",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                schema: "adoption",
                table: "Animals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedBreed",
                schema: "adoption",
                table: "Animals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                schema: "adoption",
                table: "Animals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedSpecies",
                schema: "adoption",
                table: "Animals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Species",
                schema: "adoption",
                table: "Animals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Breed",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "NormalizedBreed",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "NormalizedSpecies",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Species",
                schema: "adoption",
                table: "Animals");

            migrationBuilder.AddColumn<string>(
                name: "AnimalType_Value",
                schema: "adoption",
                table: "Animals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Breed_Value",
                schema: "adoption",
                table: "Animals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
