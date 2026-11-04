using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "membership");

            migrationBuilder.EnsureSchema(
                name: "abstractions");

            migrationBuilder.EnsureSchema(
                name: "Tools");

            migrationBuilder.EnsureSchema(
                name: "cms");

            migrationBuilder.EnsureSchema(
                name: "marketdata");

            migrationBuilder.EnsureSchema(
                name: "marketintelligence");

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicationKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SessionLifetime = table.Column<TimeSpan>(type: "time", nullable: false),
                    OtpTokenLifetime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ChangePasswordTokenLifetime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                schema: "abstractions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatorId = table.Column<int>(type: "int", nullable: false),
                    AuditType = table.Column<int>(type: "int", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryKeyValues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookmarkCollections",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookmarkCollections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookmarks",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookmarkTags",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookmarkTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilePurposes",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilePurposes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileTypes",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Extension = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FundNavGap",
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                name: "InvestmentPolicyStatements",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPolicyStatements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginResults",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDefinitions",
                schema: "marketdata",
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
                name: "Markets",
                schema: "marketintelligence",
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
                name: "MarketSettings",
                schema: "marketdata",
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
                name: "Menus",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RequiredRoute = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    RequiredInvestmentTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cms",
                        principalTable: "Menus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageServiceProviderConfigs",
                schema: "abstractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Config = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageServiceProviderConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsCategories",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingMessages",
                schema: "abstractions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MessageBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    LastAttemptTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordEntries",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Cipher = table.Column<string>(type: "nvarchar(max)", maxLength: 32000, nullable: false),
                    Iv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordVaults",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    KdfSalt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KdfIterations = table.Column<int>(type: "int", nullable: false),
                    VerifierCipher = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VerifierIv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordVaults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalSettings",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prospectus",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prospectus", x => x.Id);
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
                name: "Resources",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceTags",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Editable = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", maxLength: 8, rowVersion: true, nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Controller = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HashCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTasks",
                schema: "abstractions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Period = table.Column<long>(type: "bigint", nullable: false),
                    LastRunTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRunSucceed = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                schema: "marketintelligence",
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
                name: "Statutes",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AttachmentId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timeframes",
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                name: "Users",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnknownLoginHistories",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LoginType = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnknownLoginHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnknownLoginHistories_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "membership",
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecurityStamp = table.Column<byte[]>(type: "varbinary(256)", maxLength: 256, nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "membership",
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookmarkTagLinks",
                schema: "Tools",
                columns: table => new
                {
                    BookmarkId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookmarkTagLinks", x => new { x.BookmarkId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_BookmarkTagLinks_BookmarkTags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "Tools",
                        principalTable: "BookmarkTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookmarkTagLinks_Bookmarks_BookmarkId",
                        column: x => x.BookmarkId,
                        principalSchema: "Tools",
                        principalTable: "Bookmarks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilePurposeAcceptableTypes",
                schema: "cms",
                columns: table => new
                {
                    PurposeId = table.Column<int>(type: "int", nullable: false),
                    FileTypeId = table.Column<int>(type: "int", nullable: false),
                    MaxSize = table.Column<long>(type: "bigint", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilePurposeAcceptableTypes", x => new { x.PurposeId, x.FileTypeId });
                    table.ForeignKey(
                        name: "FK_FilePurposeAcceptableTypes_FilePurposes_PurposeId",
                        column: x => x.PurposeId,
                        principalSchema: "cms",
                        principalTable: "FilePurposes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilePurposeAcceptableTypes_FileTypes_FileTypeId",
                        column: x => x.FileTypeId,
                        principalSchema: "cms",
                        principalTable: "FileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileStores",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileTypeId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileStores_FileTypes_FileTypeId",
                        column: x => x.FileTypeId,
                        principalSchema: "cms",
                        principalTable: "FileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketSymbols",
                schema: "marketdata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketId = table.Column<int>(type: "int", nullable: false),
                    SymbolKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
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
                        principalSchema: "marketdata",
                        principalTable: "MarketDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnifiedSymbols",
                schema: "marketintelligence",
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
                        principalSchema: "marketintelligence",
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NewsAttachments",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsAttachments_News_NewsId",
                        column: x => x.NewsId,
                        principalSchema: "cms",
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsCategoryMaps",
                schema: "cms",
                columns: table => new
                {
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategoryMaps", x => new { x.CategoryId, x.NewsId });
                    table.ForeignKey(
                        name: "FK_NewsCategoryMaps_NewsCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cms",
                        principalTable: "NewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsCategoryMaps_News_NewsId",
                        column: x => x.NewsId,
                        principalSchema: "cms",
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishTargetFolders",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PublishTargetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishTargetFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishTargetFolders_PublishTargets_PublishTargetId",
                        column: x => x.PublishTargetId,
                        principalSchema: "Tools",
                        principalTable: "PublishTargets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceFiles",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceFiles_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "Tools",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceTagLinks",
                schema: "Tools",
                columns: table => new
                {
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTagLinks", x => new { x.ResourceId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ResourceTagLinks_ResourceTags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "Tools",
                        principalTable: "ResourceTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceTagLinks_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "Tools",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleRoutes",
                schema: "membership",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRoutes", x => new { x.RoleId, x.RouteId });
                    table.ForeignKey(
                        name: "FK_RoleRoutes_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "membership",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleRoutes_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "membership",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnifiedCandles",
                schema: "marketintelligence",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MarketId = table.Column<int>(type: "int", nullable: false),
                    TimeframeId = table.Column<int>(type: "int", nullable: false),
                    SymbolKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Open = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(28,8)", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(28,8)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(28,8)", nullable: true),
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
                        principalSchema: "marketintelligence",
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnifiedCandles_Timeframes_TimeframeId",
                        column: x => x.TimeframeId,
                        principalSchema: "marketintelligence",
                        principalTable: "Timeframes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactUs",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Reply = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponderId = table.Column<int>(type: "int", nullable: true),
                    ReplyDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSpam = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactUs_Users_ResponderId",
                        column: x => x.ResponderId,
                        principalSchema: "cms",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForgetPasswordTokens",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ExpiryDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgetPasswordTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForgetPasswordTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "membership",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginHistories",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginType = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    LoginResultId = table.Column<int>(type: "int", nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    Creator = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifier = table.Column<int>(type: "int", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginHistories_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "membership",
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoginHistories_LoginResults_LoginResultId",
                        column: x => x.LoginResultId,
                        principalSchema: "membership",
                        principalTable: "LoginResults",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoginHistories_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "membership",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "membership",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserIpAddresses",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIpAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserIpAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "membership",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "membership",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "membership",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "membership",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateActions",
                schema: "marketdata",
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
                        principalSchema: "marketdata",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrentOrderBookLevels",
                schema: "marketdata",
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
                        principalSchema: "marketdata",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrentSymbolStates",
                schema: "marketdata",
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
                        principalSchema: "marketdata",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FundNavPoints",
                schema: "marketdata",
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
                        principalSchema: "marketdata",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalCandles",
                schema: "marketdata",
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
                        principalSchema: "marketdata",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestorTradingActivities",
                schema: "marketdata",
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
                        principalSchema: "marketdata",
                        principalTable: "MarketSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                schema: "marketintelligence",
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
                name: "IX_Applications_ApplicationName",
                schema: "membership",
                table: "Applications",
                column: "ApplicationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkCollections_UserId_ParentId",
                schema: "Tools",
                table: "BookmarkCollections",
                columns: new[] { "UserId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId_CollectionId",
                schema: "Tools",
                table: "Bookmarks",
                columns: new[] { "UserId", "CollectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkTagLinks_TagsId",
                schema: "Tools",
                table: "BookmarkTagLinks",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkTags_UserId_Name",
                schema: "Tools",
                table: "BookmarkTags",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactUs_ResponderId",
                schema: "cms",
                table: "ContactUs",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateActions_SymbolId_ActionType_EffectiveDate",
                schema: "marketdata",
                table: "CorporateActions",
                columns: new[] { "SymbolId", "ActionType", "EffectiveDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilePurposeAcceptableTypes_FileTypeId",
                schema: "cms",
                table: "FilePurposeAcceptableTypes",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FilePurposes_Name",
                schema: "cms",
                table: "FilePurposes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileStores_FileTypeId",
                schema: "cms",
                table: "FileStores",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_Extension",
                schema: "cms",
                table: "FileTypes",
                column: "Extension",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForgetPasswordTokens_UserId",
                schema: "membership",
                table: "ForgetPasswordTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FundNavPoints_SymbolId_Date",
                schema: "marketdata",
                table: "FundNavPoints",
                columns: new[] { "SymbolId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalCandles_SymbolId_Timeframe_OpenTimeUtc",
                schema: "marketdata",
                table: "HistoricalCandles",
                columns: new[] { "SymbolId", "Timeframe", "OpenTimeUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestorTradingActivities_SymbolId_TradingDate",
                schema: "marketdata",
                table: "InvestorTradingActivities",
                columns: new[] { "SymbolId", "TradingDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistories_ApplicationId",
                schema: "membership",
                table: "LoginHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistories_LoginResultId",
                schema: "membership",
                table: "LoginHistories",
                column: "LoginResultId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistories_UserId",
                schema: "membership",
                table: "LoginHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginResults_Name",
                schema: "membership",
                table: "LoginResults",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketDefinitions_Key",
                schema: "marketdata",
                table: "MarketDefinitions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Markets_Code",
                schema: "marketintelligence",
                table: "Markets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketSettings_Scope_Key",
                schema: "marketdata",
                table: "MarketSettings",
                columns: new[] { "Scope", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketSymbols_MarketId_ExternalId",
                schema: "marketdata",
                table: "MarketSymbols",
                columns: new[] { "MarketId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketSymbols_MarketId_SymbolKey",
                schema: "marketdata",
                table: "MarketSymbols",
                columns: new[] { "MarketId", "SymbolKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                schema: "cms",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsAttachments_NewsId",
                schema: "cms",
                table: "NewsAttachments",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsCategoryMaps_NewsId",
                schema: "cms",
                table: "NewsCategoryMaps",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                schema: "Tools",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordEntries_UserId",
                schema: "Tools",
                table: "PasswordEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordVaults_UserId",
                schema: "Tools",
                table: "PasswordVaults",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublishTargetFolders_PublishTargetId",
                schema: "Tools",
                table: "PublishTargetFolders",
                column: "PublishTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishTargets_Key",
                schema: "Tools",
                table: "PublishTargets",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceFiles_ResourceId",
                schema: "Tools",
                table: "ResourceFiles",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UserId",
                schema: "Tools",
                table: "Resources",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTagLinks_TagsId",
                schema: "Tools",
                table: "ResourceTagLinks",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTags_UserId_Name",
                schema: "Tools",
                table: "ResourceTags",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleRoutes_RouteId",
                schema: "membership",
                table: "RoleRoutes",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "membership",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Code",
                schema: "marketintelligence",
                table: "Sources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timeframes_Code",
                schema: "marketintelligence",
                table: "Timeframes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedCandles_MarketId_SymbolKey_TimeframeId_Date",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                columns: new[] { "MarketId", "SymbolKey", "TimeframeId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedCandles_TimeframeId",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                column: "TimeframeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_InsCode",
                schema: "marketintelligence",
                table: "UnifiedSymbols",
                column: "InsCode");

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_InstrumentId",
                schema: "marketintelligence",
                table: "UnifiedSymbols",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_MarketId_SymbolKey",
                schema: "marketintelligence",
                table: "UnifiedSymbols",
                columns: new[] { "MarketId", "SymbolKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnifiedSymbols_Symbol",
                schema: "marketintelligence",
                table: "UnifiedSymbols",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_UnknownLoginHistories_ApplicationId",
                schema: "membership",
                table: "UnknownLoginHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "membership",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIpAddresses_UserId",
                schema: "membership",
                table: "UserIpAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "membership",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApplicationId",
                schema: "membership",
                table: "Users",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "membership",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName_Email",
                schema: "membership",
                table: "Users",
                columns: new[] { "UserName", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName_PhoneNumber",
                schema: "membership",
                table: "Users",
                columns: new[] { "UserName", "PhoneNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits",
                schema: "abstractions");

            migrationBuilder.DropTable(
                name: "BookmarkCollections",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "BookmarkTagLinks",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "ContactUs",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "CorporateActions",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "CurrentOrderBookLevels",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "CurrentSymbolStates",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "FilePurposeAcceptableTypes",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FileStores",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ForgetPasswordTokens",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "FundNavGap",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "FundNavPoints",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "HistoricalCandles",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "InvestmentFund",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "InvestmentFundChart",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "InvestmentFundInfo",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "InvestmentFundType",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "InvestmentPolicyStatements",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "InvestorTradingActivities",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "LoginHistories",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "MarketSettings",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "MessageServiceProviderConfigs",
                schema: "abstractions");

            migrationBuilder.DropTable(
                name: "NewsAttachments",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NewsCategoryMaps",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Notes",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "OutgoingMessages",
                schema: "abstractions");

            migrationBuilder.DropTable(
                name: "PasswordEntries",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "PasswordVaults",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "PortalSettings",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Prospectus",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "PublishTargetFolders",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "ResourceFiles",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "ResourceTagLinks",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "RoleRoutes",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "ScheduledTasks",
                schema: "abstractions");

            migrationBuilder.DropTable(
                name: "Sources",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "Statutes",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "TseBestLimit",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "TseClientTypeInfo",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "TseIndexSymbols",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "TseStaticThreshold",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "TseTmcETFNav",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "TseTradeLastDay",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "UnifiedCandles",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "UnifiedSymbols",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "UnknownLoginHistories",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "UserIpAddresses",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "BookmarkTags",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "Bookmarks",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FilePurposes",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FileTypes",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "MarketSymbols",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "LoginResults",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "NewsCategories",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "News",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "PublishTargets",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "ResourceTags",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Timeframes",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "Markets",
                schema: "marketintelligence");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "MarketDefinitions",
                schema: "marketdata");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "membership");
        }
    }
}
