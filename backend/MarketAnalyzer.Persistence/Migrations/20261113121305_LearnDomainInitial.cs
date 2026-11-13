using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LearnDomainInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "learn");

            migrationBuilder.CreateTable(
                name: "LearningCollections",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningCollections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LearningExportHistories",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    SelectedItemIdsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningExportHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningExportHistories_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningImportSources",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    SourceHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ImportedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningImportSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningImportSources_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningSections",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    ParentSectionId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningSections_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningSections_LearningSections_ParentSectionId",
                        column: x => x.ParentSectionId,
                        principalSchema: "learn",
                        principalTable: "LearningSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    ItemsPerBatch = table.Column<int>(type: "int", nullable: true),
                    TargetDaysPerBatch = table.Column<int>(type: "int", nullable: true),
                    DailyTimeBudgetMinutes = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPlans_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySessions",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FinishedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessions_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningItems",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    ExternalNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CurrentRevisionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningItems_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningItems_LearningSections_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "learn",
                        principalTable: "LearningSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StudyBatches",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyPlanId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyBatches_StudyPlans_StudyPlanId",
                        column: x => x.StudyPlanId,
                        principalSchema: "learn",
                        principalTable: "StudyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningContentRevisions",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LearningItemId = table.Column<int>(type: "int", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    OriginalContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningContentRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningContentRevisions_LearningItems_LearningItemId",
                        column: x => x.LearningItemId,
                        principalSchema: "learn",
                        principalTable: "LearningItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningReviewHistories",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    LearningItemId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    NextReviewAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningReviewHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningReviewHistories_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningReviewHistories_LearningItems_LearningItemId",
                        column: x => x.LearningItemId,
                        principalSchema: "learn",
                        principalTable: "LearningItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearningReviewSchedules",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    LearningItemId = table.Column<int>(type: "int", nullable: false),
                    NextReviewAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    EaseFactor = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningReviewSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningReviewSchedules_LearningCollections_CollectionId",
                        column: x => x.CollectionId,
                        principalSchema: "learn",
                        principalTable: "LearningCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LearningReviewSchedules_LearningItems_LearningItemId",
                        column: x => x.LearningItemId,
                        principalSchema: "learn",
                        principalTable: "LearningItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudySessionItems",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudySessionId = table.Column<int>(type: "int", nullable: false),
                    LearningItemId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StudiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessionItems_LearningItems_LearningItemId",
                        column: x => x.LearningItemId,
                        principalSchema: "learn",
                        principalTable: "LearningItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudySessionItems_StudySessions_StudySessionId",
                        column: x => x.StudySessionId,
                        principalSchema: "learn",
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyBatchItems",
                schema: "learn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyBatchId = table.Column<int>(type: "int", nullable: false),
                    LearningItemId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyBatchItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyBatchItems_LearningItems_LearningItemId",
                        column: x => x.LearningItemId,
                        principalSchema: "learn",
                        principalTable: "LearningItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudyBatchItems_StudyBatches_StudyBatchId",
                        column: x => x.StudyBatchId,
                        principalSchema: "learn",
                        principalTable: "StudyBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningCollections_OwnerUserId_Status_Title",
                schema: "learn",
                table: "LearningCollections",
                columns: new[] { "OwnerUserId", "Status", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningContentRevisions_LearningItemId_ContentHash",
                schema: "learn",
                table: "LearningContentRevisions",
                columns: new[] { "LearningItemId", "ContentHash" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningContentRevisions_LearningItemId_RevisionNumber",
                schema: "learn",
                table: "LearningContentRevisions",
                columns: new[] { "LearningItemId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningExportHistories_CollectionId",
                schema: "learn",
                table: "LearningExportHistories",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningExportHistories_OwnerUserId_CollectionId_CreatedAtUtc",
                schema: "learn",
                table: "LearningExportHistories",
                columns: new[] { "OwnerUserId", "CollectionId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningImportSources_CollectionId_SourceHash",
                schema: "learn",
                table: "LearningImportSources",
                columns: new[] { "CollectionId", "SourceHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningItems_CollectionId_DisplayOrder",
                schema: "learn",
                table: "LearningItems",
                columns: new[] { "CollectionId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningItems_CollectionId_ExternalNumber",
                schema: "learn",
                table: "LearningItems",
                columns: new[] { "CollectionId", "ExternalNumber" },
                unique: true,
                filter: "[ExternalNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LearningItems_SectionId",
                schema: "learn",
                table: "LearningItems",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewHistories_CollectionId",
                schema: "learn",
                table: "LearningReviewHistories",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewHistories_LearningItemId",
                schema: "learn",
                table: "LearningReviewHistories",
                column: "LearningItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewHistories_OwnerUserId_CollectionId_LearningItemId_ReviewedAtUtc",
                schema: "learn",
                table: "LearningReviewHistories",
                columns: new[] { "OwnerUserId", "CollectionId", "LearningItemId", "ReviewedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewSchedules_CollectionId",
                schema: "learn",
                table: "LearningReviewSchedules",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewSchedules_LearningItemId",
                schema: "learn",
                table: "LearningReviewSchedules",
                column: "LearningItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewSchedules_OwnerUserId_CollectionId_LearningItemId",
                schema: "learn",
                table: "LearningReviewSchedules",
                columns: new[] { "OwnerUserId", "CollectionId", "LearningItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningReviewSchedules_OwnerUserId_NextReviewAtUtc",
                schema: "learn",
                table: "LearningReviewSchedules",
                columns: new[] { "OwnerUserId", "NextReviewAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningSections_CollectionId_DisplayOrder",
                schema: "learn",
                table: "LearningSections",
                columns: new[] { "CollectionId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningSections_ParentSectionId",
                schema: "learn",
                table: "LearningSections",
                column: "ParentSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyBatches_ScheduledDate",
                schema: "learn",
                table: "StudyBatches",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_StudyBatches_StudyPlanId_Sequence",
                schema: "learn",
                table: "StudyBatches",
                columns: new[] { "StudyPlanId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyBatchItems_LearningItemId",
                schema: "learn",
                table: "StudyBatchItems",
                column: "LearningItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyBatchItems_StudyBatchId_LearningItemId",
                schema: "learn",
                table: "StudyBatchItems",
                columns: new[] { "StudyBatchId", "LearningItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_CollectionId",
                schema: "learn",
                table: "StudyPlans",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_OwnerUserId_CollectionId_Status",
                schema: "learn",
                table: "StudyPlans",
                columns: new[] { "OwnerUserId", "CollectionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudySessionItems_LearningItemId",
                schema: "learn",
                table: "StudySessionItems",
                column: "LearningItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessionItems_StudySessionId_LearningItemId",
                schema: "learn",
                table: "StudySessionItems",
                columns: new[] { "StudySessionId", "LearningItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_CollectionId",
                schema: "learn",
                table: "StudySessions",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_OwnerUserId_CollectionId_StartedAtUtc",
                schema: "learn",
                table: "StudySessions",
                columns: new[] { "OwnerUserId", "CollectionId", "StartedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningContentRevisions",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningExportHistories",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningImportSources",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningReviewHistories",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningReviewSchedules",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "StudyBatchItems",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "StudySessionItems",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "StudyBatches",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningItems",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "StudySessions",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "StudyPlans",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningSections",
                schema: "learn");

            migrationBuilder.DropTable(
                name: "LearningCollections",
                schema: "learn");
        }
    }
}
