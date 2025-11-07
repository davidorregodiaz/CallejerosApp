using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MappingAnimalAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Breed",
                schema: "adoption",
                table: "Animals",
                newName: "Breed_Value");

            migrationBuilder.RenameColumn(
                name: "AnimalType",
                schema: "adoption",
                table: "Animals",
                newName: "AnimalType_Value");

            migrationBuilder.AlterColumn<string>(
                name: "Breed_Value",
                schema: "adoption",
                table: "Animals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnimalType_Value",
                schema: "adoption",
                table: "Animals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Breed_Value",
                schema: "adoption",
                table: "Animals",
                newName: "Breed");

            migrationBuilder.RenameColumn(
                name: "AnimalType_Value",
                schema: "adoption",
                table: "Animals",
                newName: "AnimalType");

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                schema: "adoption",
                table: "Animals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "AnimalType",
                schema: "adoption",
                table: "Animals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
