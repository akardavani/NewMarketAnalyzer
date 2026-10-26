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
        public List<PublishFolderItem> Folders { get; init; } = new();
        public string OutputPath { get; init; } = "";
        public int DisplayOrder { get; init; }
        public bool IsActive { get; init; }
        public bool Exists { get; init; }
    }

    public class PublishFolderItem { public string Name { get; init; } = ""; public string Path { get; init; } = ""; }

    public class PublishResult { public string Path { get; init; } = ""; public long Size { get; init; } }

    // ---- list
    public class PublishTargetsQuery : IRequest<List<PublishTargetItem>> { }

    internal class PublishTargetsQueryHandler : IRequestHandler<PublishTargetsQuery, List<PublishTargetItem>>
    {
        private readonly IDbContext _db;
        public PublishTargetsQueryHandler(IDbContext db) => _db = db;

        public async Task<List<PublishTargetItem>> HandleAsync(PublishTargetsQuery r, CancellationToken ct = default)
        {
            var rows = await _db.Set<PublishTarget>().Include(e => e.Folders).OrderBy(e => e.DisplayOrder).ThenBy(e => e.Title).ToListAsync(ct);
            return rows.Select(e => new PublishTargetItem
            {
                Id = e.Id, Key = e.Key, Title = e.Title, Folders = e.Folders.Select(f => new PublishFolderItem { Name = f.Name, Path = f.Path }).ToList(),
                OutputPath = e.OutputPath, DisplayOrder = e.DisplayOrder, IsActive = e.IsActive,
                Exists = e.Folders.All(f => Directory.Exists(f.Path))
            }).ToList();
        }
    }

    // ---- save (Id=0 → جدید)
    public class SavePublishTargetCommand : IRequest<ApiResult<int>>
    {
        public int Id { get; init; }
        public string? Key { get; init; }
        public string? Title { get; init; }
        public List<PublishFolderItem>? Folders { get; init; }
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
            var output = (r.OutputPath ?? "").Trim();
            if (key.Length == 0 || title.Length == 0 || output.Length == 0)
                return ApiResult.FromError<int>("کلید، عنوان و مسیر خروجی الزامی است.");
            var folders = (r.Folders ?? new()).Where(f => f != null)
                .Select(f => (Name: (f.Name ?? "").Trim(), Path: (f.Path ?? "").Trim()))
                .Where(f => f.Name.Length > 0 || f.Path.Length > 0).ToList();
            if (folders.Count == 0) return ApiResult.FromError<int>("حداقل یک پوشه‌ی پروژه لازم است.");
            if (folders.Any(f => f.Name.Length == 0 || f.Name.Length > 100 || f.Name == "." || f.Name == ".." || f.Name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0))
                return ApiResult.FromError<int>("نام پوشه خالی یا نامعتبر است (فقط نام ساده مثل api).");
            if (folders.Select(f => f.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != folders.Count)
                return ApiResult.FromError<int>("نام پوشه‌ها باید یکتا باشد.");
            if (folders.Any(f => f.Path.Length > 500 || !System.IO.Path.IsPathRooted(f.Path) || f.Path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0))
                return ApiResult.FromError<int>("مسیر پوشه باید کامل و معتبر باشد (مثلا D:\\...).");
            if (key.Length > 64 || title.Length > 200 || output.Length > 500)
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
                e = await _db.Set<PublishTarget>().Include(x => x.Folders).FirstOrDefaultAsync(x => x.Id == r.Id, ct)!;
                if (e is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            }
            e.Update(key, title, output, r.DisplayOrder, r.IsActive, folders);
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
            var e = await _db.Set<PublishTarget>().Include(x => x.Folders).FirstOrDefaultAsync(x => x.Id == r.Id, ct);
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

            var selected = await _db.Set<PublishTarget>().Include(t => t.Folders)
                .Where(t => keys.Contains(t.Key) && t.IsActive).OrderBy(t => t.DisplayOrder).ToListAsync(ct);
            if (selected.Count != keys.Count) return ApiResult.FromError<PublishResult>("مورد نامعتبر یا غیرفعال است.");

            var missing = new List<string>();
            foreach (var t in selected)
                foreach (var f in t.Folders)
                    if (!Directory.Exists(f.Path)) missing.Add(f.Path);
            if (missing.Count > 0) return ApiResult.FromError<PublishResult>("مسیر یافت نشد: " + string.Join("، ", missing));

            var outDir = selected[0].OutputPath;
            var zipPath = System.IO.Path.Combine(outDir, $"Publish_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
            var jobs = selected.Select(t => (t.Key, Folders: t.Folders.Select(f => (f.Name, f.Path)).ToList())).ToList();

            await Gate.WaitAsync(ct);
            try
            {
                await Task.Run(() =>
                {
                    Directory.CreateDirectory(outDir);
                    using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                    foreach (var j in jobs)
                        foreach (var f in j.Folders)
                            AddFolder(zip, f.Path, $"{j.Key}/{f.Name}");
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
