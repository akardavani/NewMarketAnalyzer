using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Analysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edit3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockCandle");

            migrationBuilder.DropTable(
                name: "StockInstruments");

            migrationBuilder.DropTable(
                name: "TseTmcIndex");

            migrationBuilder.CreateTable(
                name: "TseIndexSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsCode = table.Column<string>(type: "TEXT", nullable: false),
                    SectorCode = table.Column<string>(type: "TEXT", nullable: true),
                    SectorName = table.Column<string>(type: "TEXT", nullable: true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Flow = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseIndexSymbol", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TseIndexSymbol");

            migrationBuilder.CreateTable(
                name: "StockCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCandle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockInstruments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseVol = table.Column<long>(type: "INTEGER", nullable: false),
                    BoardCode = table.Column<long>(type: "INTEGER", nullable: false),
                    CIsin = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<long>(type: "INTEGER", nullable: false),
                    EnglishName = table.Column<string>(type: "TEXT", nullable: false),
                    Flow = table.Column<long>(type: "INTEGER", nullable: false),
                    FourDigitCompanyCode = table.Column<string>(type: "TEXT", nullable: false),
                    InsCode = table.Column<long>(type: "INTEGER", nullable: false),
                    InstrumentGroupId = table.Column<string>(type: "TEXT", nullable: false),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    InstrumentMnemonic = table.Column<string>(type: "TEXT", nullable: false),
                    InstrumentType = table.Column<long>(type: "INTEGER", nullable: false),
                    MarketSegment = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SectorCode = table.Column<string>(type: "TEXT", nullable: false),
                    SharesOrBondsNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    SubSectorCode = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Valid = table.Column<int>(type: "INTEGER", nullable: false),
                    YVal = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInstruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseTmcIndex",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Flow = table.Column<long>(type: "INTEGER", nullable: false),
                    InsCode = table.Column<string>(type: "TEXT", nullable: false),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    SectorCode = table.Column<string>(type: "TEXT", nullable: true),
                    SectorName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseTmcIndex", x => x.Id);
                });
        }
    }
}
