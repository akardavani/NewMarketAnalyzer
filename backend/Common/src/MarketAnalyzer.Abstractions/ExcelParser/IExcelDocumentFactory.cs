using System.IO;

namespace MarketAnalyzer.Abstractions
{
    public interface IExcelDocumentFactory
    {
        IExcelDocument Create(byte[] buffer);
        IExcelDocument Create(Stream stream);
        IExcelDocument Create(string filePath);
    }
}
