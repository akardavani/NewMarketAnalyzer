using ClosedXML.Excel;
using MarketIntelligence.Infrastructure;
using System;

namespace MarketIntelligence.Application
{
    public static class ExcelReader
    {
        public static List<ExcelStockRow> Read(string filePath)
        {
            using var workbook =
                new XLWorkbook(filePath);

            var sheet =
                workbook.Worksheet(1);

            var headers =
                BuildHeaderMap(sheet);

            var result = new List<ExcelStockRow>();

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                result.Add(
                    MapRow(row, headers));
            }

            return result;
        }

        private static Dictionary<string, int> BuildHeaderMap(IXLWorksheet sheet)
        {
            var map =
                new Dictionary<string, int>();

            foreach (var cell in sheet.Row(1).CellsUsed())
            {
                map[cell.GetString().Trim()] =
                    cell.Address.ColumnNumber;
            }

            return map;
        }

        private static ExcelStockRow MapRow(IXLRow row, Dictionary<string, int> h)
        {
            return new ExcelStockRow
            {
                Symbol =
                    GetString(row, h, "نماد").NormalizeArabicName(),

                VolumeToday =
                    GetDecimal(row, h, "حجم معاملات"),

                TradeCount =
                    GetDecimal(row, h, "تعداد معاملات"),

                TradeValue =
                    GetDecimal(row, h, "ارزش معاملات"),

                FirstPrice =
                    GetDecimal(row, h, "اولین قیمت"),

                LastPrice =
                    GetDecimal(row, h, "آخرین قیمت"),

                LastPricePercent =
                    GetDecimal(row, h, "%آخرین قیمت"),

                ClosingPrice =
                    GetDecimal(row, h, "قیمت پایانی"),

                ClosingPricePercent =
                    GetDecimal(row, h, "%قیمت پایانی"),

                DemandVolume =
                    GetDecimal(row, h, "حجم تقاضا"),

                DemandPrice =
                    GetDecimal(row, h, "قیمت تقاضا"),

                SupplyPrice =
                    GetDecimal(row, h, "قیمت عرضه"),

                SupplyVolume =
                    GetDecimal(row, h, "حجم عرضه"),

                UpperThreshold =
                    GetDecimal(row, h, "آستانه بالا"),

                LowerThreshold =
                    GetDecimal(row, h, "آستانه پایین"),

                High =
                    GetDecimal(row, h, "بالاترین"),

                Low =
                    GetDecimal(row, h, "پایین‌ترین"),

                RealBuy =
                    GetDecimal(row, h, "خرید حقیقی"),

                RealBuyPercent =
                    GetDecimal(row, h, "خرید حقیقی%"),

                LegalBuy =
                    GetDecimal(row, h, "خرید حقوقی"),

                LegalBuyPercent =
                    GetDecimal(row, h, "خرید حقوقی%"),

                RealSell =
                    GetDecimal(row, h, "فروش حقیقی"),

                RealSellPercent =
                    GetDecimal(row, h, "فروش حقیقی%"),

                LegalSell =
                    GetDecimal(row, h, "فروش حقوقی"),

                LegalSellPercent =
                    GetDecimal(row, h, "فروش حقوقی%"),

                Bubble =
                    GetDecimal(row, h, "حباب"),

                EPS =
                    GetDecimal(row, h, "EPS"),

                PE =
                    GetDecimal(row, h, "P/E"),

                DPSPercent =
                    GetDecimal(row, h, "DPS%"),

                PB =
                    GetDecimal(row, h, "P/B"),

                PS =
                    GetDecimal(row, h, "P/S"),

                PNAV =
                    GetDecimal(row, h, "P/NAV"),

                Return1Month =
                    GetDecimal(row, h, "بازدهی1ماه"),

                Return3Month =
                    GetDecimal(row, h, "بازدهی3ماه"),

                Return6Month =
                    GetDecimal(row, h, "بازدهی6ماه"),

                Return1Year =
                    GetDecimal(row, h, "بازدهی1سال"),

                AvgVolume30 =
                    GetDecimal(row, h, "حجم30روزه"),

                AvgVolume90 =
                    GetDecimal(row, h, "حجم90روزه"),

                Beta =
                    GetDecimal(row, h, "ضریب بتا"),

                RSI =
                    GetDecimal(row, h, "RSI"),

                MFI =
                    GetDecimal(row, h, "MFI"),

                SMA20 =
                    GetDecimal(row, h, "SMA20d"),

                SMA50 =
                    GetDecimal(row, h, "SMA50d"),

                SMA200 =
                    GetDecimal(row, h, "SMA200d")
            };
        }

        private static string GetString(IXLRow row, Dictionary<string, int> headers, string column)
        {
            if (!headers.TryGetValue(column, out var index))
                return string.Empty;

            return row.Cell(index)
                .GetString()
                .Trim();
        }

        private static decimal GetDecimal(IXLRow row, Dictionary<string, int> headers, string column)
        {
            if (!headers.TryGetValue(column, out var index))
                return 0;

            var text =
                row.Cell(index)
                    .GetString()
                    .Replace(",", "")
                    .Trim();

            if (decimal.TryParse(
                    text,
                    out var value))
            {
                return value;
            }

            return 0;
        }
    }
}
