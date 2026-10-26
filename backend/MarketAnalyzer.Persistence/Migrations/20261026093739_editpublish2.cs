using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class editpublish2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                schema: "Tools",
                table: "PublishTargets");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Tools",
                table: "PublishTargetFolders");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                schema: "Tools",
                table: "PublishTargetFolders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                schema: "Tools",
                table: "PublishTargetFolders");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                schema: "Tools",
                table: "PublishTargets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Tools",
                table: "PublishTargetFolders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
