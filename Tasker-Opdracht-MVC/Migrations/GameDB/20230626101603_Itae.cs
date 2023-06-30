using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tasker_Opdracht_MVC.Migrations.GameDB
{
    /// <inheritdoc />
    public partial class Itae : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    User2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Board = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerWon = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PlayerWonEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeFinished = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Board", "GameId", "PlayerWon", "PlayerWonEmail", "TimeFinished", "User1", "User2" },
                values: new object[,]
                {
                    { 1, "X,X,,O,O,O,,X,", "95276d1e-5b26-439d-b0bc-cec99754781b", "user3", "user4@example.com", new DateTime(2023, 6, 26, 10, 16, 3, 240, DateTimeKind.Utc).AddTicks(2377), "user3", "user4" },
                    { 2, "X,X,,O,O,O,,X,", "0540fe66-b10b-4d28-b7c9-3036120220db", "user1", "user1@example.com", new DateTime(2023, 6, 26, 10, 16, 3, 240, DateTimeKind.Utc).AddTicks(2372), "user1", "user2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
