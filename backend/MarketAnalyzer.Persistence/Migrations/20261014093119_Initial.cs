using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "membership");

            migrationBuilder.EnsureSchema(
                name: "abstractions");

            migrationBuilder.EnsureSchema(
                name: "cms");

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
                    Period = table.Column<TimeSpan>(type: "time", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationName",
                schema: "membership",
                table: "Applications",
                column: "ApplicationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactUs_ResponderId",
                schema: "cms",
                table: "ContactUs",
                column: "ResponderId");

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
                name: "ContactUs",
                schema: "cms");

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
                name: "InvestmentPolicyStatements",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "LoginHistories",
                schema: "membership");

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
                name: "OutgoingMessages",
                schema: "abstractions");

            migrationBuilder.DropTable(
                name: "PortalSettings",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Prospectus",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "RoleRoutes",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "ScheduledTasks",
                schema: "abstractions");

            migrationBuilder.DropTable(
                name: "Statutes",
                schema: "cms");

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
                name: "Users",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FilePurposes",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FileTypes",
                schema: "cms");

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
                name: "Routes",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "membership");
        }
    }
}
