using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorOfDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "adoption");

            migrationBuilder.CreateTable(
                name: "AdoptionRequests",
                schema: "adoption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Comments = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                schema: "adoption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Breed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AnimalType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImagesPath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionRequests",
                schema: "adoption");

            migrationBuilder.DropTable(
                name: "Animals",
                schema: "adoption");
        }
    }
}
