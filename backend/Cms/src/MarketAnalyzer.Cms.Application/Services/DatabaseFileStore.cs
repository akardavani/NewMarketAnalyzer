using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Application.Services
{
    internal class DatabaseFileStore : IFileStore
    {
        public const decimal KiloByte = 1024;
        public const decimal MegaByte = KiloByte * 1024;
        public const decimal GigaByte = MegaByte * 1024;

        private readonly IDbContext _dbContext;

        public DatabaseFileStore(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FileInfo> ReadAsync(ReadFileRequest request, CancellationToken cancellation = default)
        {
            //TODO: Authorize operator

            var fileInfo = await _dbContext
                .Set<FileStore>()
                .Where(e => e.Id == request.FileId)
                .Select(e => new FileInfo()
                {
                    FileName = e.Name,
                    Content = e.Content,
                    MimeType = e.FileType.MimeType
                })
                .FirstOrDefaultAsync(cancellation);           
            return fileInfo;
        }

        public async Task<string> WriteAsync(WriteFileRequest request, CancellationToken cancellation = default)
        {
            //TODO: Authorize operator

            var purpose = await _dbContext
                .Set<FilePurpose>()
                .Include(e => e.AcceptableTypes)
                    .ThenInclude(e => e.FileType)
                .FirstOrDefaultAsync(e => e.Id == request.Purpose, cancellation);

            if (purpose is null)
            {
                throw new ClientExposedException($"موضوع فایل با شناسه {request.Purpose} پیدا نشد.");
            }

            var fileType = purpose
                .AcceptableTypes
                .FirstOrDefault(e => e.FileType.Extension.ToLower() == GetFileExtension(request.FileName).ToLower());

            if (fileType is null)
            {
                var extensions = string.Join(", ", purpose.AcceptableTypes.Select(e => e.FileType.Extension));
                throw new ClientExposedException($"پسوند فایل برای موضوع '{purpose.Name}' پذیرفتنی نیست. پسوندهای مجاز: {extensions}");
            }

            if (fileType.MaxSize > 0 && request.Content.LongLength > fileType.MaxSize)
            {
                var fileSize = FormatFileSize(fileType.MaxSize);
                throw new ClientExposedException($"حجم مجاز برای فایل '{purpose.Name}' با پسوند '{fileType.FileType.Extension}'، حداکثر {fileSize} می‌باشد.");
            }

            var file = FileStore.Create(
                id: GenerateFileId(),
                name: request.FileName,
                fileSize: request.Content.LongLength,
                fileType: fileType.FileType,
                content: request.Content);

            _dbContext.Set<FileStore>().Add(file);
            await _dbContext.SaveChangesAsync(cancellation);

            return file.Id;
        }

        public async Task<bool> RemoveAsync(ReadFileRequest request, CancellationToken cancellation = default)
        {
            //TODO: Authorize operator

            var file = await _dbContext
                .Set<FileStore>()
                .Where(e => e.Id == request.FileId)                
                .FirstOrDefaultAsync(cancellation);

            _dbContext.Set<FileStore>().Remove(file);
            await _dbContext.SaveChangesAsync(cancellation);
            return true;
        }

        private static string GenerateFileId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string GetFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
            }

            return fileName.Split('.').LastOrDefault();
        }

        private static string FormatFileSize(long length)
        {
            if (length >= GigaByte)
            {
                return $"{length / GigaByte:0.#} گیگابایت";
            }

            if (length >= MegaByte)
            {
                return $"{length / MegaByte:0.#} مگابایت";
            }

            if (length >= KiloByte)
            {
                return $"{length / KiloByte:0.#} کیلوبایت";
            }

            return $"{length} بایت";
        }
    }
}
