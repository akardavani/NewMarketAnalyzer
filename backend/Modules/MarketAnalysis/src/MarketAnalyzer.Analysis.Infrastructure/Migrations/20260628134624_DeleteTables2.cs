using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Analysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImeSymbol");

            migrationBuilder.DropTable(
                name: "MacroSymbol");

            migrationBuilder.DropTable(
                name: "RealEstateRegion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImeSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeliveryDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ParentSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    PersianDeliveryDate = table.Column<string>(type: "TEXT", nullable: false),
                    PersianName = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImeSymbol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MacroSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoldType = table.Column<int>(type: "INTEGER", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    PersianName = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroSymbol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealEstateRegion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContinueUrl = table.Column<string>(type: "TEXT", nullable: false),
                    GlobalLocationId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Province = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Zone = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateRegion", x => x.Id);
                });
        }
    }
}
