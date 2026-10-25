using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Tools.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Tools.Application
{
    public class PublishTargetItem
    {
        public int Id { get; init; }
        public string Key { get; init; } = "";
        public string Title { get; init; } = "";
        public string? Group { get; init; }
        public string Path { get; init; } = "";
        public string OutputPath { get; init; } = "";
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
        public bool Exists { get; init; }
    }

    public class PublishResult { public string Path { get; init; } = ""; public long Size { get; init; } }

    // ---- list
    public class PublishTargetsQuery : IRequest<List<PublishTargetItem>> { }

    internal class PublishTargetsQueryHandler : IRequestHandler<PublishTargetsQuery, List<PublishTargetItem>>
    {
        private readonly IDbContext _db;
        public PublishTargetsQueryHandler(IDbContext db) => _db = db;

        public async Task<List<PublishTargetItem>> HandleAsync(PublishTargetsQuery r, CancellationToken ct = default)
        {
            var rows = await _db.Set<PublishTarget>().OrderBy(e => e.DisplayOrder).ThenBy(e => e.Title).ToListAsync(ct);
            return rows.Select(e => new PublishTargetItem
            {
                Id = e.Id, Key = e.Key, Title = e.Title, Group = e.Group, Path = e.Path,
                OutputPath = e.OutputPath, DisplayOrder = e.DisplayOrder, IsActive = e.IsActive,
                Exists = Directory.Exists(e.Path)
            }).ToList();
        }
    }

    // ---- save (Id=0 → جدید)
    public class SavePublishTargetCommand : IRequest<ApiResult<int>>
    {
        public int Id { get; init; }
        public string? Key { get; init; }
        public string? Title { get; init; }
        public string? Group { get; init; }
        public string? Path { get; init; }
        public string? OutputPath { get; init; }
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; } = true;
    }

    internal class SavePublishTargetCommandHandler : IRequestHandler<SavePublishTargetCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public SavePublishTargetCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<int>> HandleAsync(SavePublishTargetCommand r, CancellationToken ct = default)
        {
            var key = (r.Key ?? "").Trim();
            var title = (r.Title ?? "").Trim();
            var path = (r.Path ?? "").Trim();
            var output = (r.OutputPath ?? "").Trim();
            if (key.Length == 0 || title.Length == 0 || path.Length == 0 || output.Length == 0)
                return ApiResult.FromError<int>("کلید، عنوان، مسیر و مسیر خروجی الزامی است.");
            if (key.Length > 64 || title.Length > 200 || path.Length > 500 || output.Length > 500)
                return ApiResult.FromError<int>("طول یکی از فیلدها بیش از حد مجاز است.");

            if (await _db.Set<PublishTarget>().AnyAsync(e => e.Key == key && e.Id != r.Id, ct))
                return ApiResult.FromError<int>("این کلید تکراری است.");

            PublishTarget e;
            if (r.Id == 0)
            {
                e = PublishTarget.Create();
                _db.Set<PublishTarget>().Add(e);
            }
            else
            {
                e = await _db.Set<PublishTarget>().FirstOrDefaultAsync(x => x.Id == r.Id, ct)!;
                if (e is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            }
            var g = r.Group?.Trim();
            e.Update(key, title, string.IsNullOrEmpty(g) ? null : g, path, output, r.DisplayOrder, r.IsActive);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(e.Id);
        }
    }

    // ---- remove
    public class RemovePublishTargetCommand : IRequest<ApiResult<int>> { public required int Id { get; init; } }

    internal class RemovePublishTargetCommandHandler : IRequestHandler<RemovePublishTargetCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemovePublishTargetCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<int>> HandleAsync(RemovePublishTargetCommand r, CancellationToken ct = default)
        {
            var e = await _db.Set<PublishTarget>().FirstOrDefaultAsync(x => x.Id == r.Id, ct);
            if (e is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            _db.Set<PublishTarget>().Remove(e);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(r.Id);
        }
    }

    // ---- run: zip انتخاب‌شده‌ها در OutputPath اولین مورد
    public class RunPublishCommand : IRequest<ApiResult<PublishResult>>
    {
        public required List<string> Keys { get; init; }
    }

    internal class RunPublishCommandHandler : IRequestHandler<RunPublishCommand, ApiResult<PublishResult>>
    {
        private static readonly SemaphoreSlim Gate = new(1, 1);
        private readonly IDbContext _db;
        public RunPublishCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<PublishResult>> HandleAsync(RunPublishCommand r, CancellationToken ct = default)
        {
            var keys = (r.Keys ?? new()).Distinct().ToList();
            if (keys.Count == 0) return ApiResult.FromError<PublishResult>("هیچ پوشه‌ای انتخاب نشده است.");

            var selected = await _db.Set<PublishTarget>().Where(t => keys.Contains(t.Key) && t.IsActive)
                .OrderBy(t => t.DisplayOrder).ToListAsync(ct);
            if (selected.Count != keys.Count) return ApiResult.FromError<PublishResult>("پوشه نامعتبر یا غیرفعال است.");

            var missing = selected.Where(s => !Directory.Exists(s.Path)).Select(s => s.Title).ToList();
            if (missing.Count > 0) return ApiResult.FromError<PublishResult>("مسیر یافت نشد: " + string.Join("، ", missing));

            var outDir = selected[0].OutputPath;
            var zipPath = System.IO.Path.Combine(outDir, $"Publish_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
            var items = selected.Select(s => (s.Path, s.Key)).ToList();

            await Gate.WaitAsync(ct);
            try
            {
                await Task.Run(() =>
                {
                    Directory.CreateDirectory(outDir);
                    using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                    foreach (var (root, prefix) in items) AddFolder(zip, root, prefix);
                }, ct);
            }
            finally { Gate.Release(); }

            return ApiResult.From(new PublishResult { Path = zipPath, Size = new System.IO.FileInfo(zipPath).Length });
        }

        private static void AddFolder(ZipArchive zip, string root, string prefix)
        {
            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                var rel = System.IO.Path.GetRelativePath(root, file).Replace('\\', '/');
                var name = System.IO.Path.GetFileName(file);
                if (name.StartsWith("appsettings", StringComparison.OrdinalIgnoreCase) && name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;
                if (name.EndsWith(".log", StringComparison.OrdinalIgnoreCase)) continue;
                var first = rel.Split('/')[0];
                if (rel.Contains('/') && (first.Equals("logs", StringComparison.OrdinalIgnoreCase) || first.Equals("log", StringComparison.OrdinalIgnoreCase))) continue;
                zip.CreateEntryFromFile(file, $"{prefix}/{rel}", CompressionLevel.Optimal);
            }
        }
    }
}
