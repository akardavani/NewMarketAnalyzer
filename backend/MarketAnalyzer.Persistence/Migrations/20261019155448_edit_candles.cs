using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class edit_candles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(28,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(28,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(28,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(28,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(28,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(28,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(18,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(18,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Open",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Low",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "High",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Close",
                schema: "marketintelligence",
                table: "UnifiedCandles",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)");
        }
    }
}
