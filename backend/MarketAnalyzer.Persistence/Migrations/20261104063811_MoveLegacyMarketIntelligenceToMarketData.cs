using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveLegacyMarketIntelligenceToMarketData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UnifiedSymbols",
                schema: "marketintelligence",
                newName: "UnifiedSymbols",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "UnifiedCandles",
                schema: "marketintelligence",
                newName: "UnifiedCandles",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "TseTradeLastDay",
                schema: "marketintelligence",
                newName: "TseTradeLastDay",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "TseTmcETFNav",
                schema: "marketintelligence",
                newName: "TseTmcETFNav",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "TseStaticThreshold",
                schema: "marketintelligence",
                newName: "TseStaticThreshold",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "TseIndexSymbols",
                schema: "marketintelligence",
                newName: "TseIndexSymbols",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "TseClientTypeInfo",
                schema: "marketintelligence",
                newName: "TseClientTypeInfo",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "TseBestLimit",
                schema: "marketintelligence",
                newName: "TseBestLimit",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "Timeframes",
                schema: "marketintelligence",
                newName: "Timeframes",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "Sources",
                schema: "marketintelligence",
                newName: "Sources",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "Markets",
                schema: "marketintelligence",
                newName: "Markets",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "InvestmentFundType",
                schema: "marketintelligence",
                newName: "InvestmentFundType",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "InvestmentFundInfo",
                schema: "marketintelligence",
                newName: "InvestmentFundInfo",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "InvestmentFundChart",
                schema: "marketintelligence",
                newName: "InvestmentFundChart",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "InvestmentFund",
                schema: "marketintelligence",
                newName: "InvestmentFund",
                newSchema: "marketdata");

            migrationBuilder.RenameTable(
                name: "FundNavGap",
                schema: "marketintelligence",
                newName: "FundNavGap",
                newSchema: "marketdata");

            migrationBuilder.Sql(
                "IF SCHEMA_ID(N'marketintelligence') IS NOT NULL " +
                "AND NOT EXISTS (SELECT 1 FROM sys.objects WHERE schema_id = SCHEMA_ID(N'marketintelligence')) " +
                "DROP SCHEMA [marketintelligence];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "UnifiedSymbols",
                schema: "marketdata",
                newName: "UnifiedSymbols",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "UnifiedCandles",
                schema: "marketdata",
                newName: "UnifiedCandles",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "TseTradeLastDay",
                schema: "marketdata",
                newName: "TseTradeLastDay",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "TseTmcETFNav",
                schema: "marketdata",
                newName: "TseTmcETFNav",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "TseStaticThreshold",
                schema: "marketdata",
                newName: "TseStaticThreshold",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "TseIndexSymbols",
                schema: "marketdata",
                newName: "TseIndexSymbols",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "TseClientTypeInfo",
                schema: "marketdata",
                newName: "TseClientTypeInfo",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "TseBestLimit",
                schema: "marketdata",
                newName: "TseBestLimit",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "Timeframes",
                schema: "marketdata",
                newName: "Timeframes",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "Sources",
                schema: "marketdata",
                newName: "Sources",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "Markets",
                schema: "marketdata",
                newName: "Markets",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "InvestmentFundType",
                schema: "marketdata",
                newName: "InvestmentFundType",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "InvestmentFundInfo",
                schema: "marketdata",
                newName: "InvestmentFundInfo",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "InvestmentFundChart",
                schema: "marketdata",
                newName: "InvestmentFundChart",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "InvestmentFund",
                schema: "marketdata",
                newName: "InvestmentFund",
                newSchema: "marketintelligence");

            migrationBuilder.RenameTable(
                name: "FundNavGap",
                schema: "marketdata",
                newName: "FundNavGap",
                newSchema: "marketintelligence");
        }
    }
}
