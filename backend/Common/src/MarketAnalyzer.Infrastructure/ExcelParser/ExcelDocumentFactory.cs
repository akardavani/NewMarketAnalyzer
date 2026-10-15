using System;
using System.IO;
using OfficeOpenXml;
using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Infrastructure.ExcelParser
{
    internal class ExcelDocumentFactory : IExcelDocumentFactory
    {
        public ExcelDocumentFactory()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IExcelDocument Create(byte[] buffer)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            var stream = new MemoryStream(buffer);
            return Create(stream);
        }

        public IExcelDocument Create(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            return Create(stream);
        }

        public IExcelDocument Create(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            return new ExcelDocument(stream);
        }
    }
}
