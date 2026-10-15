using OfficeOpenXml;
using OfficeOpenXml.Drawing.Controls;
using MarketAnalyzer.Abstractions;
using Stimulsoft.System.Windows.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Infrastructure.ExcelParser
{
    internal class ExcelDocument : IExcelDocument
    {
        private bool _disposed;
        private readonly Stream _stream;
        private readonly ExcelPackage _package;

        public ExcelDocument(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _package = new ExcelPackage(stream);
        }

        public IWorkSheet WithWorkSheet(int index)
        {
            return new WorkSheet(_package.Workbook.Worksheets[index]);
        }

        public List<T> ReadAs<T>(int worksheetIndex, int skipRows = 1) where T : new()
        {
            ThrowIfDisposed();
            var workSheet = _package.Workbook.Worksheets[worksheetIndex];
            return ReadAs<T>(workSheet, skipRows);
        }

        public List<T> ReadAs<T>(string worksheetName, int skipRows = 1) where T : new()
        {
            ThrowIfDisposed();
            var workSheet = _package.Workbook.Worksheets[worksheetName];
            return ReadAs<T>(workSheet, skipRows);
        }

        private List<T> ReadAs<T>(ExcelWorksheet worksheet, int skipRows) where T : new()
        {
            var columns = typeof(T)
                .GetProperties()
                .Where(static p => p.IsDefined(typeof(ExcelColumnAttribute)))
                .Select(static p => new
                {
                    Property = p,
                    p.GetCustomAttribute<ExcelColumnAttribute>().Order
                });

            return worksheet
                .Cells
                .Select(e => e.Start.Row)
                .Distinct()
                .Order()
                .Skip(skipRows)
                .Select(ReadItem)
                .ToList();

            T ReadItem(int rowIndex)
            {
                var item = new T();
                foreach (var column in columns)
                {
                    var propertyType = column.Property.PropertyType;
                    var cell = worksheet.Cells[rowIndex, column.Order];
                    if (propertyType == typeof(string))
                    {
                        column.Property.SetValue(item, cell.GetValue<string>());
                    }
                    else if (propertyType == typeof(int))
                    {
                        column.Property.SetValue(item, cell.GetValue<int>());
                    }
                    else if (propertyType == typeof(int?))
                    {
                        column.Property.SetValue(item, cell.GetValue<int?>());
                    }
                    else if (propertyType == typeof(long))
                    {
                        column.Property.SetValue(item, cell.GetValue<long>());
                    }
                    else if (propertyType == typeof(long?))
                    {
                        column.Property.SetValue(item, cell.GetValue<long?>());
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        column.Property.SetValue(item, cell.GetValue<decimal>());
                    }
                    else if (propertyType == typeof(decimal?))
                    {
                        column.Property.SetValue(item, cell.GetValue<decimal?>());
                    }
                    else if (propertyType == typeof(double))
                    {
                        column.Property.SetValue(item, cell.GetValue<double>());
                    }
                    else if (propertyType == typeof(double?))
                    {
                        column.Property.SetValue(item, cell.GetValue<double?>());
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        column.Property.SetValue(item, cell.GetValue<DateTime>());
                    }
                    else if (propertyType == typeof(DateTime?))
                    {
                        column.Property.SetValue(item, cell.GetValue<DateTime?>());
                    }
                    else if (propertyType == typeof(DateOnly))
                    {
                        column.Property.SetValue(item, cell.GetValue<DateOnly>());
                    }
                    else if (propertyType == typeof(DateOnly?))
                    {
                        column.Property.SetValue(item, cell.GetValue<DateOnly?>());
                    }
                    else
                    {
                        throw new NotSupportedException($"The property type [{propertyType.FullName}] is not supported.");
                    }
                }
                return item;
            }
        }

        private void VisitObject(object model, ExcelRowContext context)
        {
            if (model is null)
            {
                return;
            }

            var valueTypes = new Type[]
            {
                typeof(string),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateOnly),
                typeof(DateOnly?)
            };

            var schema = model
                .GetType()
                .GetProperties()
                .Where(static p => p.IsDefined(typeof(ExcelSchemaAttribute)))
                .Select(static property =>
                {
                    var attribute = property.GetCustomAttribute<ExcelSchemaAttribute>();
                    return new
                    {
                        Property = property,
                        attribute.SheetIndex,
                        attribute.RowIndex,
                        attribute.CellIndex
                    };
                })
                .OrderBy(e => e.RowIndex);

            foreach (var entry in schema)
            {
                var childContext = new ExcelRowContext()
                {
                    CellIndex = entry.CellIndex,
                    SheetIndex = entry.SheetIndex,
                    RowIndex = entry.RowIndex + context.RowIndex
                };
                var value = entry.Property.GetValue(model);
                if (valueTypes.Contains(entry.Property.PropertyType))
                {
                    VisitScalar(value, childContext);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(entry.Property.PropertyType))
                {
                    VisitEnumerable((IEnumerable)value, childContext);
                }
                else
                {
                    VisitObject(value, context);
                }
            }
        }

        private void VisitEnumerable(IEnumerable collection, ExcelRowContext context)
        {
            foreach (var item in collection)
            {
                VisitObject(item, context);
                context.RowIndex++;
            }
        }

        private void VisitScalar(object value, ExcelRowContext context)
        {
            _package
                .Workbook
                .Worksheets[context.SheetIndex]
                .Cells[context.RowIndex, context.CellIndex]
                .Value = value;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _stream.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        public Task SaveAsAsync(Stream stream, CancellationToken cancellation)
        {
            return _package.SaveAsAsync(stream, cancellation);
        }

        public async Task<byte[]> RenderAsync(CancellationToken cancellation)
        {
            using var ms = new MemoryStream();
            await SaveAsAsync(ms, cancellation);
            return ms.ToArray();
        }

        private class ExcelRowContext
        {
            public int RowIndex { get; set; }
            public required int CellIndex { get; set; }
            public required int SheetIndex { get; init; }
        }

        private class WorkSheet : IWorkSheet
        {
            private readonly ExcelWorksheet _worksheet;

            public WorkSheet(ExcelWorksheet worksheet)
            {
                _worksheet = worksheet;
            }

            public T GetCellValue<T>(string address) => (T)_worksheet.Cells[address].Value;

            public void Calculate() => _worksheet.Calculate();

            public IExcelCell GetCell(string name, int rowIndex)
            {
                return new Cell(_worksheet, name, rowIndex);
            }

            public void MergeCells(string from, string to, int rowIndex)
            {
                _worksheet.Cells[$"{from}{rowIndex}:{to}{rowIndex}"].Merge = true;
            }

            public IExcelCell SetCellValue<T>(string name, int rowIndex, T value)
            {
                var cell = GetCell(name, rowIndex);
                cell.Value = value;
                return cell;
            }

            public IExcelControl AddControl(string text, ExcelControlType type)
            {
                var control = _worksheet.Drawings.AddControl(text, (eControlType)type);
                return new ExcelControlProxy(_worksheet, control);
            }

            public IEnumerable<IExcelControl> GetControls(ExcelControlType type)
            {
                return _worksheet.Drawings
                     .OfType<ExcelControl>()
                     .Where(e => e.ControlType == (eControlType)type)
                     .Select(control => new ExcelControlProxy(_worksheet, control));
            }
        }

        private class Cell : IExcelCell
        {
            private readonly string _name;
            private readonly int _rowIndex;
            private readonly ExcelRange _cell;
            private readonly ExcelWorksheet _worksheet;

            public Cell(ExcelWorksheet worksheet, string name, int rowIndex)
            {
                _name = name;
                _worksheet = worksheet;
                _rowIndex = rowIndex;
                _cell = worksheet.Cells[$"{name}{rowIndex}"];
            }

            public string StyleName
            {
                get => _cell.StyleName;
                set => _cell.StyleName = value;
            }

            public string Formula
            {
                get => _cell.Formula;
                set => _cell.Formula = value;
            }

            public object Value
            {
                get => _cell.Value;
                set => _cell.Value = value;
            }

            public IExcelCell SetValue(object value)
            {
                Value = value;
                return this;
            }

            public IExcelCell IsBold(bool bold)
            {
                _cell.Style.Font.Bold = bold;
                return this;
            }

            public IExcelCell WrapText(bool wrap)
            {
                _cell.Style.WrapText = wrap;
                return this;
            }

            public IExcelCell AddListValidation(IEnumerable<string> items, string errorTitle = null, string error = null)
            {
                ClearDataValidations();
                var validation = _worksheet.DataValidations.AddListValidation(_cell.Address);
                foreach (var item in items)
                {
                    validation.Formula.Values.Add(item);
                }
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = errorTitle ?? "مقدار نامعتبر";
                validation.Error = error ?? "لطفاً فقط یکی از مقادیر مجاز را انتخاب کنید.";

                return this;
            }

            public IExcelCell SetBaseStyleFrom(bool alignmentFormat)
            {
                var baseCell = _worksheet.Cells[$"A1"];

                _cell.Style.Font.Name = baseCell.Style.Font.Name;
                _cell.Style.Font.Size = baseCell.Style.Font.Size;
                _cell.Style.Font.Bold = baseCell.Style.Font.Bold;
                _cell.Style.VerticalAlignment = baseCell.Style.VerticalAlignment;
                _cell.Style.HorizontalAlignment = baseCell.Style.HorizontalAlignment;
                _cell.Style.Numberformat.Format = baseCell.Style.Numberformat.Format;
                IsBold(false);
                if (alignmentFormat == true)
                {
                    _cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    _cell.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.RightToLeft;
                }

                return this;
            }

            /// <summary>
            /// برای سلول های جمع استفاده میکنیم
            /// </summary>
            public IExcelCell Border()
            {
                var border = _cell.Style.Border;
                border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;      // تک‌خطی بالا
                border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double; // دوتایی پایین
                return this;
            }

            public IExcelCell SetStyleFrom(int baseRowIndex, bool alignmentFormat)
            {
                var baseCell = _worksheet.Cells[$"{_name}{baseRowIndex}"];

                _cell.Style.Font.Name = baseCell.Style.Font.Name;
                _cell.Style.Font.Size = baseCell.Style.Font.Size;
                _cell.Style.Font.Bold = baseCell.Style.Font.Bold;
                _cell.Style.VerticalAlignment = baseCell.Style.VerticalAlignment;
                _cell.Style.HorizontalAlignment = baseCell.Style.HorizontalAlignment;
                _cell.Style.Numberformat.Format = baseCell.Style.Numberformat.Format;

                if (alignmentFormat == true)
                {
                    _cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    _cell.Style.ReadingOrder = OfficeOpenXml.Style.ExcelReadingOrder.RightToLeft;
                }

                return this;
            }

            public IExcelCell SetNumberFormat(string format)
            {
                SetBaseStyleFrom(false);
                _cell.Style.Numberformat.Format = format;
                return this;
            }

            public IExcelCell SetPercentageFormat(string format)
            {
                SetBaseStyleFrom(false);
                _cell.Style.Numberformat.Format = format;
                return this;
            }

            private void ClearDataValidations()
            {
                var existingValidations = _worksheet
                    .DataValidations
                    .Where(e => e.Address.Address == _cell.Address)
                    .ToList();

                foreach (var val in existingValidations)
                {
                    _worksheet.DataValidations.Remove(val);
                }
            }
        }

        private class ExcelControlProxy : IExcelControl
        {
            private readonly ExcelWorksheet _worksheet;
            private readonly ExcelControl _control;

            public ExcelControlProxy(ExcelWorksheet worksheet, ExcelControl control)
            {
                _worksheet = worksheet;
                _control = control;
            }

            public string Name
            {
                get => _control.Name;
                set => _control.Name = value;
            }

            public string AlternativeText
            {
                get => _control.AlternativeText;
                set => _control.AlternativeText = value;
            }

            public string LinkedCell
            {
                get => _control.LinkedCell?.FullAddress;
                set => _control.LinkedCell = string.IsNullOrEmpty(value) ? null : _worksheet.Cells[value];
            }

            public void AdjustPositionAndSize() => _control.AdjustPositionAndSize();

            public void SetSize(int width, int height) => _control.SetSize(width, height);

            public void SetPosition(int pixelTop, int pixelLeft)
                => _control.SetPosition(pixelTop, pixelLeft);

            public void SetPosition(int row, int rowOffsetPixels, int column, int columnOffsetPixels)
                => _control.SetPosition(row, rowOffsetPixels, column, columnOffsetPixels);
        }
    }
}
