using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDashboardSymbols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DashboardSymbols",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MarketKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SymbolKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardSymbols", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSymbols_UserId_DisplayOrder",
                table: "DashboardSymbols",
                columns: new[] { "UserId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardSymbols_UserId_MarketKey_SymbolKey",
                table: "DashboardSymbols",
                columns: new[] { "UserId", "MarketKey", "SymbolKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardSymbols");
        }
    }
}
