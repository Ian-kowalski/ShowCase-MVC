using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowCase_MVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerBoard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackingBoard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentTurnPlayerId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStates");
        }
    }
}
