using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Analysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTables1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoSymbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    LastTradedPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Market = table.Column<string>(type: "TEXT", nullable: false),
                    QuoteCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    SymbolCode = table.Column<string>(type: "TEXT", nullable: false),
                    Trading = table.Column<bool>(type: "INTEGER", nullable: false),
                    Vol = table.Column<decimal>(type: "TEXT", nullable: false),
                    VolValue = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoSymbol", x => x.Id);
                });
        }
    }
}
