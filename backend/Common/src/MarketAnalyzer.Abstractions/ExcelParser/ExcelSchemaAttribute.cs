using System;

namespace MarketAnalyzer.Abstractions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ExcelSchemaAttribute : Attribute
    {
        /// <summary>
        /// اندیس برگه اکسل
        /// </summary>
        public required int SheetIndex { get; init; }
        /// <summary>
        /// اندیس سطر
        /// </summary>
        public required int RowIndex { get; init; }
        /// <summary>
        /// اندیس سلول
        /// </summary>
        public required int CellIndex { get; init; }
    }
}
