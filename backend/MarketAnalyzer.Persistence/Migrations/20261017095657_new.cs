using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundNavGap",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundInstrumentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousNav = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    CurrentNav = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    Ratio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundNavGap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFund",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundInstrumentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitiationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FundType = table.Column<int>(type: "int", nullable: true),
                    TypeOfInvest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvestedUnits = table.Column<long>(type: "bigint", nullable: true),
                    ManagerSeoRegisterNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    FundPublisher = table.Column<int>(type: "int", nullable: true),
                    FundSize = table.Column<long>(type: "bigint", nullable: true),
                    NetAsset = table.Column<long>(type: "bigint", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFund", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFundChart",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundInstrumentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuenceNav = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CancellationNav = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatisticalNav = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFundChart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFundInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitiationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FundType = table.Column<int>(type: "int", nullable: false),
                    ExecutiveManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArticlesOfAssociationLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProsoectusLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DividendIntervalPeriod = table.Column<int>(type: "int", nullable: true),
                    EstimatedEarningRate = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: true),
                    GuaranteedEarningRate = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: true),
                    MarketMaker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetAsset = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvestedUnits = table.Column<long>(type: "bigint", nullable: false),
                    UnitsRedDAY = table.Column<long>(type: "bigint", nullable: false),
                    UnitsRedFromFirst = table.Column<long>(type: "bigint", nullable: false),
                    UnitsSubDAY = table.Column<long>(type: "bigint", nullable: false),
                    UnitsSubFromFirst = table.Column<long>(type: "bigint", nullable: false),
                    CancelNav = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssueNav = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatisticalNav = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TypeOfInvest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerSeoRegisterNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guarantor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuarantorSeoRegisterNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Auditor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Custodian = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvestmentManager = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beta = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: true),
                    Alpha = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: true),
                    SeoRegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    InsCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmallSymbolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundPublisher = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFundInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFundType",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFundType", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timeframes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeframes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseBestLimit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    InsCode = table.Column<long>(type: "bigint", nullable: false),
                    BuyVolume = table.Column<long>(type: "bigint", nullable: false),
                    BuyQuantity = table.Column<long>(type: "bigint", nullable: false),
                    BuyRequestQuantity = table.Column<long>(type: "bigint", nullable: false),
                    SellVolume = table.Column<long>(type: "bigint", nullable: false),
                    SellQuantity = table.Column<long>(type: "bigint", nullable: false),
                    SellRequestQuantity = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseBestLimit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseClientTypeInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InsCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecDate = table.Column<long>(type: "bigint", nullable: false),
                    BuyIVolume = table.Column<double>(type: "float", nullable: false),
                    BuyNVolume = table.Column<double>(type: "float", nullable: false),
                    BuyIValue = table.Column<double>(type: "float", nullable: false),
                    BuyNValue = table.Column<double>(type: "float", nullable: false),
                    BuyNCount = table.Column<int>(type: "int", nullable: false),
                    SellIVolume = table.Column<double>(type: "float", nullable: false),
                    BuyICount = table.Column<double>(type: "float", nullable: false),
                    SellNVolume = table.Column<double>(type: "float", nullable: false),
                    SellIValue = table.Column<double>(type: "float", nullable: false),
                    SellNValue = table.Column<double>(type: "float", nullable: false),
                    SellNCount = table.Column<int>(type: "int", nullable: false),
                    SellICount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseClientTypeInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseIndexSymbols",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstrumentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flow = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseIndexSymbols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseStaticThreshold",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InsCode = table.Column<long>(type: "bigint", nullable: false),
                    MaxRange = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    MinRange = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseStaticThreshold", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseTmcETFNav",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InsCode = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<long>(type: "bigint", nullable: false),
                    Time = table.Column<long>(type: "bigint", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancellationNav = table.Column<decimal>(type: "decimal(20,6)", precision: 20, scale: 6, nullable: false),
                    IssuanceNav = table.Column<decimal>(type: "decimal(20,6)", precision: 20, scale: 6, nullable: false),
                    IsClose = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseTmcETFNav", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseTradeLastDay",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InsCode = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<int>(type: "int", nullable: false),
                    PriceFirst = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    Closing = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    LastPrice = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    TransactionValue = table.Column<decimal>(type: "decimal(30,4)", precision: 30, scale: 4, nullable: false),
                    PriceChange = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    PriceMin = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    PriceMax = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    PriceYesterday = table.Column<decimal>(type: "decimal(20,4)", precision: 20, scale: 4, nullable: false),
                    Last = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseTradeLastDay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnifiedSymbols",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MarketId = table.Column<int>(type: "int", nullable: false),
                    SymbolKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstrumentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InsCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PersianName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ParentSymbol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnifiedSymbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnifiedSymbols_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnifiedCandles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MarketId = table.Column<int>(type: "int", nullable: false),
                    TimeframeId = table.Column<int>(type: "int", nullable: false),
                    SymbolKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersianDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnifiedCandles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnifiedCandles_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnifiedCandles_Timeframes_TimeframeId",
                        column: x => x.TimeframeId,
                        principalTable: "Timeframes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Markets",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Tse", true, "بورس تهران" },
                    { 2, "Macro", true, "اقتصاد کلان" },
                    { 3, "Crypto", true, "رمزارز" },
                    { 4, "Ime", true, "بورس کالا" },
                    { 5, "InvestmentFund", true, "صندوق سرمایه‌گذاری" },
                    { 6, "RealEstate", true, "مسکن" },
                    { 7, "TseIndex", true, "شاخص بورس" },
                    { 8, "CryptoToman", true, "کریپتو تومان" }
                });

            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 10, "Nahayatnegar", "نهایت نگر" },
                    { 11, "Tse", "بورس تهران" },
                    { 20, "Kucoin", "کوکوین" },
                    { 21, "Binance", "بایننس" },
                    { 30, "tgju", "tgju" },
                    { 40, "Nobitex", "نوبیتکس" },
                    { 41, "Wallex", "والکس" },
                    { 50, "Kilid", "کلید" },
                    { 70, "Fipiran", "فیپ ایران" },
                    { 80, "Ime", "کالا" }
                });

            migrationBuilder.InsertData(
                table: "Timeframes",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "Monthly", "ماهانه" },
                    { 2, "Weekly", "هفتگی" },
                    { 3, "Daily", "روزانه" },
                    { 4, "4H", "۴ ساعته" },
                    { 5, "1H", "۱ ساعته" },
                    { 6, "15M", "۱۵ دقیقه" },
                    { 7, "5M", "۵ دقیقه" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Markets_Code",
                table: "Markets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Code",
                table: "Sources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timeframes_Code",
                table: "Timeframes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedCandles_MarketId_SymbolKey_TimeframeId_Date",
                table: "UnifiedCandles",
                columns: new[] { "MarketId", "SymbolKey", "TimeframeId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedCandles_TimeframeId",
                table: "UnifiedCandles",
                column: "TimeframeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_InsCode",
                table: "UnifiedSymbols",
                column: "InsCode");

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_InstrumentId",
                table: "UnifiedSymbols",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_MarketId_SymbolKey",
                table: "UnifiedSymbols",
                columns: new[] { "MarketId", "SymbolKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_Symbol",
                table: "UnifiedSymbols",
                column: "Symbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundNavGap");

            migrationBuilder.DropTable(
                name: "InvestmentFund");

            migrationBuilder.DropTable(
                name: "InvestmentFundChart");

            migrationBuilder.DropTable(
                name: "InvestmentFundInfo");

            migrationBuilder.DropTable(
                name: "InvestmentFundType");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "TseBestLimit");

            migrationBuilder.DropTable(
                name: "TseClientTypeInfo");

            migrationBuilder.DropTable(
                name: "TseIndexSymbols");

            migrationBuilder.DropTable(
                name: "TseStaticThreshold");

            migrationBuilder.DropTable(
                name: "TseTmcETFNav");

            migrationBuilder.DropTable(
                name: "TseTradeLastDay");

            migrationBuilder.DropTable(
                name: "UnifiedCandles");

            migrationBuilder.DropTable(
                name: "UnifiedSymbols");

            migrationBuilder.DropTable(
                name: "Timeframes");

            migrationBuilder.DropTable(
                name: "Markets");
        }
    }
}
