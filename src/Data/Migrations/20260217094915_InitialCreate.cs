using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PseudocodeEditorAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PseudocodeDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PseudocodeDocuments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PseudocodeDocuments_UpdatedAt",
                table: "PseudocodeDocuments",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PseudocodeDocuments");
        }
    }
}
