using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "animal_type");

            migrationBuilder.DropTable(
                name: "breeds");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "animals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "breed",
                table: "animals",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "animals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_animals_age",
                table: "animals",
                column: "age");

            migrationBuilder.CreateIndex(
                name: "IX_animals_animal_id",
                table: "animals",
                column: "animal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_animals_breed",
                table: "animals",
                column: "breed");

            migrationBuilder.CreateIndex(
                name: "IX_animals_name",
                table: "animals",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_animals_type",
                table: "animals",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_animals_age",
                table: "animals");

            migrationBuilder.DropIndex(
                name: "IX_animals_animal_id",
                table: "animals");

            migrationBuilder.DropIndex(
                name: "IX_animals_breed",
                table: "animals");

            migrationBuilder.DropIndex(
                name: "IX_animals_name",
                table: "animals");

            migrationBuilder.DropIndex(
                name: "IX_animals_type",
                table: "animals");

            migrationBuilder.DropColumn(
                name: "breed",
                table: "animals");

            migrationBuilder.DropColumn(
                name: "type",
                table: "animals");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "animals",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "animal_type",
                columns: table => new
                {
                    animal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animal_type", x => x.animal_id);
                    table.ForeignKey(
                        name: "FK_animal_type_animals_animal_id",
                        column: x => x.animal_id,
                        principalTable: "animals",
                        principalColumn: "animal_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "breeds",
                columns: table => new
                {
                    animal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    breed = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_breeds", x => x.animal_id);
                    table.ForeignKey(
                        name: "FK_breeds_animals_animal_id",
                        column: x => x.animal_id,
                        principalTable: "animals",
                        principalColumn: "animal_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
