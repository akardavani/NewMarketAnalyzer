using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Tools.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.IO;

namespace MarketAnalyzer.Tools.Application
{
    public class ResourceFileItem { public int Id { get; init; } public string FileName { get; init; } = ""; public string ContentType { get; init; } = ""; public long Size { get; init; } }

    public class ResourceItem
    {
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
        public string? Link { get; init; }
        public List<string> Tags { get; init; } = new();
        public List<ResourceFileItem> Files { get; init; } = new();
        public DateTime LastModificationTime { get; init; }
    }

    // ---- list (جدیدترین اول، صفحه‌بندی برای اسکرول)
    public class ResourcesQuery : IRequest<List<ResourceItem>>
    {
        public required int UserId { get; init; }
        public string? Search { get; init; }
        public int Skip { get; init; }
        public int Take { get; init; } = 20;
    }

    internal class ResourcesQueryHandler : IRequestHandler<ResourcesQuery, List<ResourceItem>>
    {
        private readonly IDbContext _db;
        public ResourcesQueryHandler(IDbContext db) => _db = db;

        public async Task<List<ResourceItem>> HandleAsync(ResourcesQuery r, CancellationToken ct = default)
        {
            var q = _db.Set<Resource>().Where(e => e.UserId == r.UserId);
            var s = r.Search?.Trim();
            if (!string.IsNullOrEmpty(s)) q = q.Where(e => e.Title.Contains(s) || e.Description.Contains(s));
            var rows = await q.OrderByDescending(e => e.LastModificationTime).ThenByDescending(e => e.Id)
                .Skip(Math.Max(0, r.Skip)).Take(Math.Clamp(r.Take, 1, 100))
                .Select(e => new
                {
                    e.Id, e.Title, e.Description, e.Link, e.LastModificationTime,
                    Tags = e.Tags.Select(t => t.Name).ToList(),
                    Files = e.Files.Select(f => new ResourceFileItem { Id = f.Id, FileName = f.FileName, ContentType = f.ContentType, Size = f.Size }).ToList()
                }).ToListAsync(ct);
            return rows.Select(e => new ResourceItem { Id = e.Id, Title = e.Title, Description = e.Description, Link = e.Link, LastModificationTime = e.LastModificationTime, Tags = e.Tags, Files = e.Files }).ToList();
        }
    }

    // ---- tag suggestions
    public class ResourceTagsQuery : IRequest<List<string>>
    {
        public required int UserId { get; init; }
        public string? Term { get; init; }
    }

    internal class ResourceTagsQueryHandler : IRequestHandler<ResourceTagsQuery, List<string>>
    {
        private readonly IDbContext _db;
        public ResourceTagsQueryHandler(IDbContext db) => _db = db;
        public Task<List<string>> HandleAsync(ResourceTagsQuery r, CancellationToken ct = default)
        {
            var t = (r.Term ?? "").Trim();
            return _db.Set<ResourceTag>().Where(e => e.UserId == r.UserId && e.Name.Contains(t))
                .OrderBy(e => e.Name).Select(e => e.Name).Take(10).ToListAsync(ct);
        }
    }

    // ---- save (Id=0 → جدید). فایل‌ها دست نمی‌خورند.
    public class SaveResourceCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public int Id { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? Link { get; init; }
        public List<string>? Tags { get; init; }
    }

    internal class SaveResourceCommandHandler : IRequestHandler<SaveResourceCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public SaveResourceCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<int>> HandleAsync(SaveResourceCommand r, CancellationToken ct = default)
        {
            var title = (r.Title ?? "").Trim();
            if (title.Length == 0) return ApiResult.FromError<int>("عنوان الزامی است.");
            if (title.Length > 200) title = title[..200];
            var desc = (r.Description ?? "").Trim();
            if (desc.Length > 2000) desc = desc[..2000];
            var link = (r.Link ?? "").Trim();
            if (link.Length > 0 && !(Uri.TryCreate(link, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps)))
                return ApiResult.FromError<int>("لینک معتبر نیست (باید با http یا https شروع شود).");

            var names = (r.Tags ?? new()).Select(x => x.Trim().ToLowerInvariant()).Where(x => x.Length > 0)
                .Select(x => x.Length > 50 ? x[..50] : x).Distinct().Take(20).ToList();
            var existing = await _db.Set<ResourceTag>().Where(t => t.UserId == r.UserId && names.Contains(t.Name)).ToListAsync(ct);
            foreach (var n in names.Where(n => existing.All(t => t.Name != n)))
            {
                var t = ResourceTag.Create(r.UserId, n);
                _db.Set<ResourceTag>().Add(t);
                existing.Add(t);
            }

            Resource? res;
            if (r.Id == 0)
            {
                res = Resource.Create(r.UserId);
                _db.Set<Resource>().Add(res);
            }
            else
            {
                res = await _db.Set<Resource>().Include(e => e.Tags).FirstOrDefaultAsync(e => e.Id == r.Id && e.UserId == r.UserId, ct);
                if (res is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            }
            res.Update(title, desc, link.Length == 0 ? null : link, existing);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(res.Id);
        }
    }

    // ---- remove
    public class RemoveResourceCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required int UserId { get; init; }
    }

    internal class RemoveResourceCommandHandler : IRequestHandler<RemoveResourceCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemoveResourceCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RemoveResourceCommand r, CancellationToken ct = default)
        {
            var e = await _db.Set<Resource>().FirstOrDefaultAsync(x => x.Id == r.Id && x.UserId == r.UserId, ct);
            if (e is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            _db.Set<Resource>().Remove(e);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(e.Id);
        }
    }

    // ---- files
    /// <summary>FileId=0 → افزودن؛ در غیر این صورت جایگزینی همان فایل (فقط بعد از دریافت کامل فایل جدید)</summary>
    public class SaveResourceFileCommand : IRequest<ApiResult<ResourceFileItem>>
    {
        public required int UserId { get; init; }
        public required int ResourceId { get; init; }
        public int FileId { get; init; }
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required byte[] Content { get; init; }
    }

    internal class SaveResourceFileCommandHandler : IRequestHandler<SaveResourceFileCommand, ApiResult<ResourceFileItem>>
    {
        public const int MaxBytes = 20 * 1024 * 1024;
        private readonly IDbContext _db;
        public SaveResourceFileCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<ResourceFileItem>> HandleAsync(SaveResourceFileCommand r, CancellationToken ct = default)
        {
            if (r.Content.Length == 0) return ApiResult.FromError<ResourceFileItem>("فایل خالی است.");
            if (r.Content.Length > MaxBytes) return ApiResult.FromError<ResourceFileItem>("حجم فایل بیش از ۲۰ مگابایت است.");
            var extension = Path.GetExtension(r.FileName).ToLowerInvariant();
            var blockedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".exe", ".dll", ".bat", ".cmd", ".com", ".scr", ".msi", ".msp", ".ps1", ".psm1",
                ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh", ".hta", ".reg", ".lnk", ".cpl"
            };
            if (blockedExtensions.Contains(extension))
                return ApiResult.FromError<ResourceFileItem>($"آپلود فایل با پسوند {extension} مجاز نیست.");
            if (!await _db.Set<Resource>().AnyAsync(e => e.Id == r.ResourceId && e.UserId == r.UserId, ct))
                return ApiResult.FromError<ResourceFileItem>("مورد وجود ندارد.");
            var name = Path.GetFileName(r.FileName);
            if (name.Length > 260) name = name[^260..];
            var type = string.IsNullOrWhiteSpace(r.ContentType) ? "application/octet-stream" : r.ContentType;

            ResourceFile f;
            if (r.FileId == 0)
            {
                f = ResourceFile.Create(r.ResourceId, name, type, r.Content);
                _db.Set<ResourceFile>().Add(f);
            }
            else
            {
                f = (await _db.Set<ResourceFile>().FirstOrDefaultAsync(x => x.Id == r.FileId && x.ResourceId == r.ResourceId, ct))!;
                if (f is null) return ApiResult.FromError<ResourceFileItem>("فایل وجود ندارد.");
                f.Replace(name, type, r.Content);
            }
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(new ResourceFileItem { Id = f.Id, FileName = f.FileName, ContentType = f.ContentType, Size = f.Size });
        }
    }

    public class RemoveResourceFileCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public required int FileId { get; init; }
    }

    internal class RemoveResourceFileCommandHandler : IRequestHandler<RemoveResourceFileCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemoveResourceFileCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RemoveResourceFileCommand r, CancellationToken ct = default)
        {
            var f = await _db.Set<ResourceFile>().FirstOrDefaultAsync(x => x.Id == r.FileId && _db.Set<Resource>().Any(e => e.Id == x.ResourceId && e.UserId == r.UserId), ct);
            if (f is null) return ApiResult.FromError<int>("فایل وجود ندارد.");
            _db.Set<ResourceFile>().Remove(f);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(f.Id);
        }
    }

    public class ResourceFileDownload { public string FileName { get; init; } = ""; public string ContentType { get; init; } = ""; public byte[] Content { get; init; } = Array.Empty<byte>(); }

    public class ResourceFileQuery : IRequest<ResourceFileDownload?>
    {
        public required int UserId { get; init; }
        public required int FileId { get; init; }
    }

    internal class ResourceFileQueryHandler : IRequestHandler<ResourceFileQuery, ResourceFileDownload?>
    {
        private readonly IDbContext _db;
        public ResourceFileQueryHandler(IDbContext db) => _db = db;
        public Task<ResourceFileDownload?> HandleAsync(ResourceFileQuery r, CancellationToken ct = default)
            => _db.Set<ResourceFile>().Where(x => x.Id == r.FileId && _db.Set<Resource>().Any(e => e.Id == x.ResourceId && e.UserId == r.UserId))
                .Select(x => new ResourceFileDownload { FileName = x.FileName, ContentType = x.ContentType, Content = x.Content })
                .FirstOrDefaultAsync(ct);
    }
}
