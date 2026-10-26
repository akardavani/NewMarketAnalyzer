using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPublish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketDefinitions",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PersianName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CalendarKind = table.Column<byte>(type: "tinyint", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketSettings",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublishTargets",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OutputPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketSymbols",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketId = table.Column<int>(type: "int", nullable: false),
                    SymbolKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PersianName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetadataVersion = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketSymbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketSymbols_MarketDefinitions_MarketId",
                        column: x => x.MarketId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CorporateActions",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymbolId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<byte>(type: "tinyint", nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDateApproximate = table.Column<bool>(type: "bit", nullable: false),
                    AdjustmentFactor = table.Column<decimal>(type: "decimal(28,12)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    Origin = table.Column<byte>(type: "tinyint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateActions_MarketSymbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrentOrderBookLevels",
                schema: "marketintelligence",
                columns: table => new
                {
                    SymbolId = table.Column<int>(type: "int", nullable: false),
                    Side = table.Column<byte>(type: "tinyint", nullable: false),
                    LevelNumber = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false),
                    SnapshotTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentOrderBookLevels", x => new { x.SymbolId, x.Side, x.LevelNumber });
                    table.ForeignKey(
                        name: "FK_CurrentOrderBookLevels_MarketSymbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrentSymbolStates",
                schema: "marketintelligence",
                columns: table => new
                {
                    SymbolId = table.Column<int>(type: "int", nullable: false),
                    LastPrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    ClosePrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    OpenPrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    HighPrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    LowPrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    PreviousClosePrice = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    PriceLimitMin = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    PriceLimitMax = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    Volume = table.Column<decimal>(type: "decimal(28,8)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(28,8)", nullable: true),
                    TradeCount = table.Column<long>(type: "bigint", nullable: true),
                    SourceTimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentSymbolStates", x => x.SymbolId);
                    table.ForeignKey(
                        name: "FK_CurrentSymbolStates_MarketSymbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundNavPoints",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymbolId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IssuanceNav = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    CancellationNav = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    StatisticalNav = table.Column<decimal>(type: "decimal(28,10)", nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundNavPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundNavPoints_MarketSymbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalCandles",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymbolId = table.Column<int>(type: "int", nullable: false),
                    Timeframe = table.Column<byte>(type: "tinyint", nullable: false),
                    OpenTimeUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(28,10)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(28,8)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(28,8)", nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalCandles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricalCandles_MarketSymbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestorTradingActivities",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SymbolId = table.Column<int>(type: "int", nullable: false),
                    TradingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IndividualBuyVolume = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    IndividualBuyValue = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    IndividualBuyCount = table.Column<long>(type: "bigint", nullable: false),
                    IndividualSellVolume = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    IndividualSellValue = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    IndividualSellCount = table.Column<long>(type: "bigint", nullable: false),
                    InstitutionalBuyVolume = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    InstitutionalBuyValue = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    InstitutionalBuyCount = table.Column<long>(type: "bigint", nullable: false),
                    InstitutionalSellVolume = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    InstitutionalSellValue = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    InstitutionalSellCount = table.Column<long>(type: "bigint", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestorTradingActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestorTradingActivities_MarketSymbols_SymbolId",
                        column: x => x.SymbolId,
                        principalSchema: "marketintelligence",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporateActions_SymbolId_ActionType_EffectiveDate",
                schema: "marketintelligence",
                table: "CorporateActions",
                columns: new[] { "SymbolId", "ActionType", "EffectiveDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FundNavPoints_SymbolId_Date",
                schema: "marketintelligence",
                table: "FundNavPoints",
                columns: new[] { "SymbolId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalCandles_SymbolId_Timeframe_OpenTimeUtc",
                schema: "marketintelligence",
                table: "HistoricalCandles",
                columns: new[] { "SymbolId", "Timeframe", "OpenTimeUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestorTradingActivities_SymbolId_TradingDate",
                schema: "marketintelligence",
                table: "InvestorTradingActivities",
                columns: new[] { "SymbolId", "TradingDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketDefinitions_Key",
                schema: "marketintelligence",
                table: "MarketDefinitions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketSettings_Scope_Key",
                schema: "marketintelligence",
                table: "MarketSettings",
                columns: new[] { "Scope", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketSymbols_MarketId_ExternalId",
                schema: "marketintelligence",
                table: "MarketSymbols",
                columns: new[] { "MarketId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketSymbols_MarketId_SymbolKey",
                schema: "marketintelligence",
                table: "MarketSymbols",
                columns: new[] { "MarketId", "SymbolKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublishTargets_Key",
                schema: "Tools",
                table: "PublishTargets",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporateActions",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "CurrentOrderBookLevels",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "CurrentSymbolStates",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "FundNavPoints",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "HistoricalCandles",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "InvestorTradingActivities",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "MarketSettings",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "PublishTargets",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "MarketSymbols",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "MarketDefinitions",
                schema: "marketintelligence");
        }
    }
}
