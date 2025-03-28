using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sentimeter.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddResultsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentSentimentResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentSentimentResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentSentimentResult_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoSentimentResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoSentimentResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoSentimentResult_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentSentimentResult_CommentId",
                table: "CommentSentimentResult",
                column: "CommentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoSentimentResult_VideoId",
                table: "VideoSentimentResult",
                column: "VideoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentSentimentResult");

            migrationBuilder.DropTable(
                name: "VideoSentimentResult");
        }
    }
}
