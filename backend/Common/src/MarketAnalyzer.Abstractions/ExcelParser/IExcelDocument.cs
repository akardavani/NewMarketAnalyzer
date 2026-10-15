using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IExcelDocument : IDisposable
    {
        List<T> ReadAs<T>(int worksheetIndex, int skipRows = 1) where T : new();
        List<T> ReadAs<T>(string worksheetName, int skipRows = 1) where T : new();

        IWorkSheet WithWorkSheet(int index);

        Task<byte[]> RenderAsync(CancellationToken cancellation);

        Task SaveAsAsync(Stream stream, CancellationToken cancellation);
    }
}
