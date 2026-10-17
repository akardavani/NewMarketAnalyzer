using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Analysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTseClientTypeInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TseClientTypeInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsCode = table.Column<string>(type: "TEXT", nullable: false),
                    RecDate = table.Column<long>(type: "INTEGER", nullable: false),
                    BuyIVolume = table.Column<double>(type: "REAL", nullable: false),
                    BuyNVolume = table.Column<double>(type: "REAL", nullable: false),
                    BuyIValue = table.Column<double>(type: "REAL", nullable: false),
                    BuyNValue = table.Column<double>(type: "REAL", nullable: false),
                    BuyNCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SellIVolume = table.Column<double>(type: "REAL", nullable: false),
                    BuyICount = table.Column<double>(type: "REAL", nullable: false),
                    SellNVolume = table.Column<double>(type: "REAL", nullable: false),
                    SellIValue = table.Column<double>(type: "REAL", nullable: false),
                    SellNValue = table.Column<double>(type: "REAL", nullable: false),
                    SellNCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SellICount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseClientTypeInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TseClientTypeInfo");
        }
    }
}
