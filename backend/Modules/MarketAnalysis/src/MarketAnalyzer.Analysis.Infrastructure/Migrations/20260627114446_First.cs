using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketAnalyzer.Analysis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoCandle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    SymbolCode = table.Column<string>(type: "TEXT", nullable: false),
                    Vol = table.Column<decimal>(type: "TEXT", nullable: false),
                    VolValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    QuoteCurrency = table.Column<string>(type: "TEXT", nullable: false),
                    Trading = table.Column<bool>(type: "INTEGER", nullable: false),
                    Market = table.Column<string>(type: "TEXT", nullable: false),
                    LastTradedPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoSymbol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FundNavGap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegNo = table.Column<string>(type: "TEXT", nullable: false),
                    FundInstrumentId = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PreviousNav = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    CurrentNav = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    Ratio = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundNavGap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImeCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImeCandle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImeSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    PersianName = table.Column<string>(type: "TEXT", nullable: false),
                    PersianDeliveryDate = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ParentSymbol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImeSymbol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndexCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndexCandle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFund",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegNo = table.Column<string>(type: "TEXT", nullable: false),
                    FundInstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    InitiationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FundType = table.Column<int>(type: "INTEGER", nullable: true),
                    TypeOfInvest = table.Column<string>(type: "TEXT", nullable: false),
                    InvestedUnits = table.Column<long>(type: "INTEGER", nullable: true),
                    ManagerSeoRegisterNo = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    FundPublisher = table.Column<int>(type: "INTEGER", nullable: true),
                    FundSize = table.Column<long>(type: "INTEGER", nullable: true),
                    NetAsset = table.Column<long>(type: "INTEGER", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFund", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFundChart",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegNo = table.Column<string>(type: "TEXT", nullable: false),
                    FundInstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IssuenceNav = table.Column<decimal>(type: "TEXT", nullable: false),
                    CancellationNav = table.Column<decimal>(type: "TEXT", nullable: false),
                    StatisticalNav = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFundChart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFundInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegNo = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    InitiationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FundType = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutiveManager = table.Column<string>(type: "TEXT", nullable: false),
                    ArticlesOfAssociationLink = table.Column<string>(type: "TEXT", nullable: false),
                    ProsoectusLink = table.Column<string>(type: "TEXT", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DividendIntervalPeriod = table.Column<int>(type: "INTEGER", nullable: true),
                    EstimatedEarningRate = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: true),
                    GuaranteedEarningRate = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: true),
                    MarketMaker = table.Column<string>(type: "TEXT", nullable: false),
                    NetAsset = table.Column<decimal>(type: "TEXT", nullable: false),
                    InvestedUnits = table.Column<long>(type: "INTEGER", nullable: false),
                    UnitsRedDAY = table.Column<long>(type: "INTEGER", nullable: false),
                    UnitsRedFromFirst = table.Column<long>(type: "INTEGER", nullable: false),
                    UnitsSubDAY = table.Column<long>(type: "INTEGER", nullable: false),
                    UnitsSubFromFirst = table.Column<long>(type: "INTEGER", nullable: false),
                    CancelNav = table.Column<decimal>(type: "TEXT", nullable: false),
                    IssueNav = table.Column<decimal>(type: "TEXT", nullable: false),
                    StatisticalNav = table.Column<decimal>(type: "TEXT", nullable: false),
                    TypeOfInvest = table.Column<string>(type: "TEXT", nullable: false),
                    Manager = table.Column<string>(type: "TEXT", nullable: false),
                    ManagerSeoRegisterNo = table.Column<string>(type: "TEXT", nullable: false),
                    Guarantor = table.Column<string>(type: "TEXT", nullable: false),
                    GuarantorSeoRegisterNo = table.Column<string>(type: "TEXT", nullable: false),
                    Auditor = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Custodian = table.Column<string>(type: "TEXT", nullable: false),
                    InvestmentManager = table.Column<string>(type: "TEXT", nullable: false),
                    Beta = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: true),
                    Alpha = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: true),
                    SeoRegisterDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "TEXT", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NationalId = table.Column<string>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    InsCode = table.Column<string>(type: "TEXT", nullable: false),
                    SmallSymbolName = table.Column<string>(type: "TEXT", nullable: false),
                    FundPublisher = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFundInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentFundType",
                columns: table => new
                {
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentFundType", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "MacroCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroCandle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MacroSymbol",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    PersianName = table.Column<string>(type: "TEXT", nullable: false),
                    GoldType = table.Column<int>(type: "INTEGER", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentSymbol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroSymbol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealEstateCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateCandle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealEstateRegion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Province = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Zone = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    GlobalLocationId = table.Column<long>(type: "INTEGER", nullable: false),
                    ContinueUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateRegion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockCandle",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    Open = table.Column<decimal>(type: "TEXT", nullable: false),
                    Close = table.Column<decimal>(type: "TEXT", nullable: false),
                    High = table.Column<decimal>(type: "TEXT", nullable: false),
                    Low = table.Column<decimal>(type: "TEXT", nullable: false),
                    Volume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    NetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Date = table.Column<long>(type: "INTEGER", nullable: false),
                    InsCode = table.Column<long>(type: "INTEGER", nullable: false),
                    InstrumentId = table.Column<string>(type: "TEXT", nullable: false),
                    InstrumentMnemonic = table.Column<string>(type: "TEXT", nullable: false),
                    EnglishName = table.Column<string>(type: "TEXT", nullable: false),
                    FourDigitCompanyCode = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CIsin = table.Column<string>(type: "TEXT", nullable: false),
                    InstrumentGroupId = table.Column<string>(type: "TEXT", nullable: false),
                    MarketSegment = table.Column<string>(type: "TEXT", nullable: false),
                    BoardCode = table.Column<long>(type: "INTEGER", nullable: false),
                    InstrumentType = table.Column<long>(type: "INTEGER", nullable: false),
                    SharesOrBondsNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    SectorCode = table.Column<string>(type: "TEXT", nullable: false),
                    SubSectorCode = table.Column<string>(type: "TEXT", nullable: false),
                    BaseVol = table.Column<long>(type: "INTEGER", nullable: false),
                    YVal = table.Column<long>(type: "INTEGER", nullable: true),
                    Flow = table.Column<long>(type: "INTEGER", nullable: false),
                    Valid = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInstruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timeframes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeframes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseBestLimit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    InsCode = table.Column<long>(type: "INTEGER", nullable: false),
                    BuyVolume = table.Column<long>(type: "INTEGER", nullable: false),
                    BuyQuantity = table.Column<long>(type: "INTEGER", nullable: false),
                    BuyRequestQuantity = table.Column<long>(type: "INTEGER", nullable: false),
                    SellVolume = table.Column<long>(type: "INTEGER", nullable: false),
                    SellQuantity = table.Column<long>(type: "INTEGER", nullable: false),
                    SellRequestQuantity = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseBestLimit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseStaticThreshold",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsCode = table.Column<long>(type: "INTEGER", nullable: false),
                    MaxRange = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    MinRange = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseStaticThreshold", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseTmcETFNav",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsCode = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<long>(type: "INTEGER", nullable: false),
                    Time = table.Column<long>(type: "INTEGER", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CancellationNav = table.Column<decimal>(type: "TEXT", precision: 20, scale: 6, nullable: false),
                    IssuanceNav = table.Column<decimal>(type: "TEXT", precision: 20, scale: 6, nullable: false),
                    IsClose = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseTmcETFNav", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseTmcIndex",
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
                    table.PrimaryKey("PK_TseTmcIndex", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TseTradeLastDay",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsCode = table.Column<long>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Time = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceFirst = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    Closing = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    LastPrice = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    Count = table.Column<long>(type: "INTEGER", nullable: false),
                    Volume = table.Column<long>(type: "INTEGER", nullable: false),
                    TransactionValue = table.Column<decimal>(type: "TEXT", precision: 30, scale: 4, nullable: false),
                    PriceChange = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    PriceMin = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    PriceMax = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    PriceYesterday = table.Column<decimal>(type: "TEXT", precision: 20, scale: 4, nullable: false),
                    Last = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TseTradeLastDay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnifiedSymbols",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MarketId = table.Column<int>(type: "INTEGER", nullable: false),
                    SymbolKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    InstrumentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    InsCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    PersianName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ParentSymbol = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MetadataJson = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MarketId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeframeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SymbolKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PersianDate = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                name: "CryptoCandle");

            migrationBuilder.DropTable(
                name: "CryptoSymbol");

            migrationBuilder.DropTable(
                name: "FundNavGap");

            migrationBuilder.DropTable(
                name: "ImeCandle");

            migrationBuilder.DropTable(
                name: "ImeSymbol");

            migrationBuilder.DropTable(
                name: "IndexCandle");

            migrationBuilder.DropTable(
                name: "InvestmentFund");

            migrationBuilder.DropTable(
                name: "InvestmentFundChart");

            migrationBuilder.DropTable(
                name: "InvestmentFundInfo");

            migrationBuilder.DropTable(
                name: "InvestmentFundType");

            migrationBuilder.DropTable(
                name: "MacroCandle");

            migrationBuilder.DropTable(
                name: "MacroSymbol");

            migrationBuilder.DropTable(
                name: "RealEstateCandle");

            migrationBuilder.DropTable(
                name: "RealEstateRegion");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "StockCandle");

            migrationBuilder.DropTable(
                name: "StockInstruments");

            migrationBuilder.DropTable(
                name: "TseBestLimit");

            migrationBuilder.DropTable(
                name: "TseStaticThreshold");

            migrationBuilder.DropTable(
                name: "TseTmcETFNav");

            migrationBuilder.DropTable(
                name: "TseTmcIndex");

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
