using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BannedBooks.Migrations
{
    /// <inheritdoc />
    public partial class AddIllustratorToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Illustrator",
                table: "Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryAuthor",
                table: "Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Translator",
                table: "Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Illustrator",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SecondaryAuthor",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Translator",
                table: "Books");
        }
    }
}
