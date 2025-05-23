using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TracePlayer.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToPlayerAndPlayerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Players_SteamId",
                table: "Players",
                column: "SteamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNames_Name",
                table: "PlayerNames",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_SteamId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_PlayerNames_Name",
                table: "PlayerNames");
        }
    }
}
