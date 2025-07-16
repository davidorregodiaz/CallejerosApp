using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipAnimalWithOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "animals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_animals_OwnerId",
                table: "animals",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_animals_AspNetUsers_OwnerId",
                table: "animals",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_animals_AspNetUsers_OwnerId",
                table: "animals");

            migrationBuilder.DropIndex(
                name: "IX_animals_OwnerId",
                table: "animals");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "animals");
        }
    }
}
