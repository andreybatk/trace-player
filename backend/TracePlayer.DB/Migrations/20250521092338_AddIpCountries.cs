using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TracePlayer.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddIpCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ip_country",
                columns: table => new
                {
                    ip_from = table.Column<long>(type: "bigint", nullable: false),
                    ip_to = table.Column<long>(type: "bigint", nullable: false),
                    country_code = table.Column<string>(type: "char(2)", nullable: false),
                    country_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ip_country", x => new { x.ip_from, x.ip_to });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ip_country");
        }
    }
}
