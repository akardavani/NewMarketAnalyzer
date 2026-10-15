using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public interface IWorkSheet
    {
        IExcelCell GetCell(string name, int rowIndex);

        IExcelCell SetCellValue<T>(string name, int rowIndex, T value);

        T GetCellValue<T>(string address);

        void MergeCells(string from, string to, int rowIndex);

        void Calculate();

        IExcelControl AddControl(string text, ExcelControlType type);

        IEnumerable<IExcelControl> GetControls(ExcelControlType type);
    }
}
