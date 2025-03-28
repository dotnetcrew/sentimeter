using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentimeter.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisDateForResultTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "VideoSentimentResult",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "CommentSentimentResult",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "VideoSentimentResult");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "CommentSentimentResult");
        }
    }
}
