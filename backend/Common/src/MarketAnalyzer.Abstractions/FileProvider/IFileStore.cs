using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IFileStore
    {
        Task<FileInfo> ReadAsync(ReadFileRequest request, CancellationToken cancellation = default);
        Task<string> WriteAsync(WriteFileRequest request, CancellationToken cancellation = default);

        Task<bool> RemoveAsync(ReadFileRequest request, CancellationToken cancellation = default);
    }
}
