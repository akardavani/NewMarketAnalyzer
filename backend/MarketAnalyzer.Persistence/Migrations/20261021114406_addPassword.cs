using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordEntries",
                schema: "Tools");

            migrationBuilder.DropTable(
                name: "PasswordVaults",
                schema: "Tools");
        }
    }
}
