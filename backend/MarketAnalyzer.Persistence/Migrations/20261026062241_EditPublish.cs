using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditPublish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                schema: "Tools",
                table: "PublishTargets");

            migrationBuilder.CreateTable(
                name: "PublishTargetFolders",
                schema: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_PublishTargetFolders_PublishTargetId",
                schema: "Tools",
                table: "PublishTargetFolders",
                column: "PublishTargetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishTargetFolders",
                schema: "Tools");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                schema: "Tools",
                table: "PublishTargets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
