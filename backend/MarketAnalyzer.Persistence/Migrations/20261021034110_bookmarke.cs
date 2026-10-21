using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class bookmarke : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookmarkCollections",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "BookmarkTagLinks",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "BookmarkTags",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "Bookmarks",
                schema: "Tools");
        }
    }
}
