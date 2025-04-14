using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BannedBooks.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBan",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Illustrator",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SecondaryAuthor",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Translator",
                table: "Books",
                newName: "Genre");

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Books",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "Books",
                newName: "Translator");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBan",
                table: "Books",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "State",
                table: "Books",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
