using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentimeter.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreFieldForResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "VideoSentimentResult",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "CommentSentimentResult",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "VideoSentimentResult");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "CommentSentimentResult");
        }
    }
}
