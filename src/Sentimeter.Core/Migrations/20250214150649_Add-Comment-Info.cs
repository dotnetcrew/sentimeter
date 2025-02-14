using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentimeter.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Comments");
        }
    }
}
