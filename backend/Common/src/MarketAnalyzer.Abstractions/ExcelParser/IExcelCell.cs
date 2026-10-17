using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public interface IExcelCell
    {
        object Value { get; set; }
        string StyleName { get; set; }
        string Formula { get; set; }

        IExcelCell SetValue(object value);

        IExcelCell SetStyleFrom(int baseRowIndex, bool alignmentFormat = false);
        IExcelCell SetBaseStyleFrom(bool alignmentFormat = false);
        IExcelCell SetNumberFormat(string format = "#,##0_);[Red](#,##0)");
        IExcelCell SetPercentageFormat(string format = "0.00%");
        IExcelCell IsBold(bool bold = true);

        IExcelCell WrapText(bool wrap);
        IExcelCell Border();
        IExcelCell AddListValidation(IEnumerable<string> items, string errorTitle = null, string error = null);
    }
}
