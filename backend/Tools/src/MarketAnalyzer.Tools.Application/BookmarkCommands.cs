using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Tools.Domain;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace MarketAnalyzer.Tools.Application
{
    public class BookmarkItem
    {
        public int Id { get; init; }
        public int? CollectionId { get; init; }
        public string Url { get; init; } = "";
        public string Title { get; init; } = "";
        public string Note { get; init; } = "";
        public string? ImageUrl { get; init; }
        public BookmarkKind Kind { get; init; }
        public bool IsFavorite { get; init; }
        public List<string> Tags { get; init; } = new();
        public DateTime LastModificationTime { get; init; }
    }
    public class CollectionItem { public int Id { get; init; } public int? ParentId { get; init; } public string Name { get; init; } = ""; public int Count { get; init; } }
    public class TagItem { public string Name { get; init; } = ""; public int Count { get; init; } }

    // ---- list
    public class BookmarksQuery : IRequest<List<BookmarkItem>>
    {
        public required int UserId { get; init; }
        public string? Search { get; init; }
        public string? Tag { get; init; }
        public int? CollectionId { get; init; }
        public bool FavoritesOnly { get; init; }
        public BookmarkKind? Kind { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
        public int Skip { get; init; }
        public int Take { get; init; } = 20;
    }

    internal class BookmarksQueryHandler : IRequestHandler<BookmarksQuery, List<BookmarkItem>>
    {
        private readonly IDbContext _db;
        public BookmarksQueryHandler(IDbContext db) => _db = db;

        public async Task<List<BookmarkItem>> HandleAsync(BookmarksQuery r, CancellationToken ct = default)
        {
            var q = _db.Set<Bookmark>().Where(e => e.UserId == r.UserId);
            var s = r.Search?.Trim();
            if (!string.IsNullOrEmpty(s)) q = q.Where(e => e.Title.Contains(s) || e.Note.Contains(s) || e.Url.Contains(s) || e.Tags.Any(t => t.Name.Contains(s)));
            if (!string.IsNullOrWhiteSpace(r.Tag)) { var tg = r.Tag.Trim().ToLowerInvariant(); q = q.Where(e => e.Tags.Any(t => t.Name == tg)); }
            if (r.FavoritesOnly) q = q.Where(e => e.IsFavorite);
            if (r.Kind.HasValue) q = q.Where(e => e.Kind == r.Kind);
            if (r.From.HasValue) q = q.Where(e => e.CreationTime >= r.From);
            if (r.To.HasValue) { var to = r.To.Value.Date.AddDays(1); q = q.Where(e => e.CreationTime < to); }
            if (r.CollectionId.HasValue)
            {
                var all = await _db.Set<BookmarkCollection>().Where(c => c.UserId == r.UserId).Select(c => new { c.Id, c.ParentId }).ToListAsync(ct);
                var ids = Descendants(all.Select(a => (a.Id, a.ParentId)).ToList(), r.CollectionId.Value);
                q = q.Where(e => e.CollectionId != null && ids.Contains(e.CollectionId.Value));
            }
            return await q.OrderByDescending(e => e.CreationTime).ThenByDescending(e => e.Id)
                .Skip(Math.Max(0, r.Skip)).Take(Math.Clamp(r.Take, 1, 100))
                .Select(e => new BookmarkItem
                {
                    Id = e.Id, CollectionId = e.CollectionId, Url = e.Url, Title = e.Title, Note = e.Note, ImageUrl = e.ImageUrl,
                    Kind = e.Kind, IsFavorite = e.IsFavorite, LastModificationTime = e.LastModificationTime,
                    Tags = e.Tags.Select(t => t.Name).ToList()
                }).ToListAsync(ct);
        }

        internal static List<int> Descendants(List<(int Id, int? ParentId)> all, int root)
        {
            var res = new List<int> { root };
            for (var i = 0; i < res.Count; i++) res.AddRange(all.Where(a => a.ParentId == res[i] && !res.Contains(a.Id)).Select(a => a.Id));
            return res;
        }
    }

    // ---- sidebar: collections + tags
    public class CollectionsQuery : IRequest<List<CollectionItem>> { public required int UserId { get; init; } }
    internal class CollectionsQueryHandler : IRequestHandler<CollectionsQuery, List<CollectionItem>>
    {
        private readonly IDbContext _db;
        public CollectionsQueryHandler(IDbContext db) => _db = db;
        public async Task<List<CollectionItem>> HandleAsync(CollectionsQuery r, CancellationToken ct = default)
        {
            var counts = await _db.Set<Bookmark>().Where(b => b.UserId == r.UserId && b.CollectionId != null)
                .GroupBy(b => b.CollectionId!.Value).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.N, ct);
            var list = await _db.Set<BookmarkCollection>().Where(c => c.UserId == r.UserId).OrderBy(c => c.Name).ToListAsync(ct);
            return list.Select(c => new CollectionItem { Id = c.Id, ParentId = c.ParentId, Name = c.Name, Count = counts.GetValueOrDefault(c.Id) }).ToList();
        }
    }

    public class BookmarkTagsQuery : IRequest<List<TagItem>> { public required int UserId { get; init; } public string? Term { get; init; } }
    internal class BookmarkTagsQueryHandler : IRequestHandler<BookmarkTagsQuery, List<TagItem>>
    {
        private readonly IDbContext _db;
        public BookmarkTagsQueryHandler(IDbContext db) => _db = db;
        public async Task<List<TagItem>> HandleAsync(BookmarkTagsQuery r, CancellationToken ct = default)
        {
            var t = (r.Term ?? "").Trim().ToLowerInvariant();
            var tags = await _db.Set<BookmarkTag>().Where(e => e.UserId == r.UserId && e.Name.Contains(t)).OrderBy(e => e.Name).Take(200).ToListAsync(ct);
            var ids = tags.Select(x => x.Id).ToList();
            var counts = await _db.Set<Bookmark>().Where(b => b.UserId == r.UserId)
                .SelectMany(b => b.Tags.Where(x => ids.Contains(x.Id)).Select(x => x.Id)).GroupBy(x => x).Select(g => new { g.Key, N = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.N, ct);
            return tags.Select(x => new TagItem { Name = x.Name, Count = counts.GetValueOrDefault(x.Id) }).ToList();
        }
    }

    // ---- save
    public class SaveBookmarkCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public int Id { get; init; }
        public string? Url { get; init; }
        public string? Title { get; init; }
        public string? Note { get; init; }
        public string? ImageUrl { get; init; }
        public BookmarkKind Kind { get; init; }
        public int? CollectionId { get; init; }
        public List<string>? Tags { get; init; }
    }

    internal class SaveBookmarkCommandHandler : IRequestHandler<SaveBookmarkCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public SaveBookmarkCommandHandler(IDbContext db) => _db = db;

        private static bool IsHttp(string s) => Uri.TryCreate(s, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);

        public async Task<ApiResult<int>> HandleAsync(SaveBookmarkCommand r, CancellationToken ct = default)
        {
            var url = (r.Url ?? "").Trim();
            if (!IsHttp(url) || url.Length > 2000) return ApiResult.FromError<int>("آدرس معتبر نیست (باید با http یا https شروع شود).");
            var title = (r.Title ?? "").Trim();
            if (title.Length == 0) title = new Uri(url).Host;
            if (title.Length > 300) title = title[..300];
            var note = (r.Note ?? "").Trim(); if (note.Length > 4000) note = note[..4000];
            var img = (r.ImageUrl ?? "").Trim(); if (img.Length == 0 || img.Length > 2000 || !IsHttp(img)) img = null;

            if (r.CollectionId.HasValue && !await _db.Set<BookmarkCollection>().AnyAsync(c => c.Id == r.CollectionId && c.UserId == r.UserId, ct))
                return ApiResult.FromError<int>("کالکشن وجود ندارد.");

            var tags = await BookmarkTagHelper.GetOrCreate(_db, r.UserId, r.Tags, ct);
            Bookmark? b;
            if (r.Id == 0) { b = Bookmark.Create(r.UserId); _db.Set<Bookmark>().Add(b); }
            else
            {
                b = await _db.Set<Bookmark>().Include(e => e.Tags).FirstOrDefaultAsync(e => e.Id == r.Id && e.UserId == r.UserId, ct);
                if (b is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            }
            b.Update(url, title, note, img, r.Kind, r.CollectionId, tags);
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(b.Id);
        }
    }

    internal static class BookmarkTagHelper
    {
        public static async Task<List<BookmarkTag>> GetOrCreate(IDbContext db, int userId, IEnumerable<string>? raw, CancellationToken ct)
        {
            var names = (raw ?? Array.Empty<string>()).Select(x => x.Trim().ToLowerInvariant()).Where(x => x.Length > 0)
                .Select(x => x.Length > 50 ? x[..50] : x).Distinct().Take(20).ToList();
            var existing = await db.Set<BookmarkTag>().Where(t => t.UserId == userId && names.Contains(t.Name)).ToListAsync(ct);
            foreach (var n in names.Where(n => existing.All(t => t.Name != n)))
            { var t = BookmarkTag.Create(userId, n); db.Set<BookmarkTag>().Add(t); existing.Add(t); }
            return existing;
        }
    }

    public class SetBookmarkFavoriteCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public int Id { get; init; } public bool Value { get; init; } }
    internal class SetBookmarkFavoriteCommandHandler : IRequestHandler<SetBookmarkFavoriteCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public SetBookmarkFavoriteCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(SetBookmarkFavoriteCommand r, CancellationToken ct = default)
        {
            var b = await _db.Set<Bookmark>().FirstOrDefaultAsync(e => e.Id == r.Id && e.UserId == r.UserId, ct);
            if (b is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            b.SetFavorite(r.Value); await _db.SaveChangesAsync(ct); return ApiResult.From(b.Id);
        }
    }

    public class RemoveBookmarkCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public int Id { get; init; } }
    internal class RemoveBookmarkCommandHandler : IRequestHandler<RemoveBookmarkCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemoveBookmarkCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RemoveBookmarkCommand r, CancellationToken ct = default)
        {
            var b = await _db.Set<Bookmark>().FirstOrDefaultAsync(e => e.Id == r.Id && e.UserId == r.UserId, ct);
            if (b is null) return ApiResult.FromError<int>("مورد وجود ندارد.");
            _db.Set<Bookmark>().Remove(b); await _db.SaveChangesAsync(ct); return ApiResult.From(r.Id);
        }
    }

    // ---- collections CRUD
    public class SaveCollectionCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public int Id { get; init; } public string? Name { get; init; } public int? ParentId { get; init; } }
    internal class SaveCollectionCommandHandler : IRequestHandler<SaveCollectionCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public SaveCollectionCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(SaveCollectionCommand r, CancellationToken ct = default)
        {
            var name = (r.Name ?? "").Trim();
            if (name.Length == 0) return ApiResult.FromError<int>("نام کالکشن الزامی است.");
            if (name.Length > 100) name = name[..100];
            var all = await _db.Set<BookmarkCollection>().Where(c => c.UserId == r.UserId).ToListAsync(ct);
            if (r.ParentId.HasValue && all.All(c => c.Id != r.ParentId)) return ApiResult.FromError<int>("کالکشن والد وجود ندارد.");
            BookmarkCollection? c;
            if (r.Id == 0) { c = BookmarkCollection.Create(r.UserId, name, r.ParentId); _db.Set<BookmarkCollection>().Add(c); }
            else
            {
                c = all.FirstOrDefault(x => x.Id == r.Id);
                if (c is null) return ApiResult.FromError<int>("کالکشن وجود ندارد.");
                if (r.ParentId.HasValue && BookmarksQueryHandler.Descendants(all.Select(a => (a.Id, a.ParentId)).ToList(), c.Id).Contains(r.ParentId.Value))
                    return ApiResult.FromError<int>("نمی‌توان کالکشن را داخل خودش یا زیرمجموعه‌اش برد.");
                c.Update(name, r.ParentId);
            }
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(c.Id);
        }
    }

    /// <summary>حذف کالکشن و زیرمجموعه‌ها؛ لینک‌ها حذف نمی‌شوند و بدون کالکشن می‌مانند</summary>
    public class RemoveCollectionCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public int Id { get; init; } }
    internal class RemoveCollectionCommandHandler : IRequestHandler<RemoveCollectionCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemoveCollectionCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RemoveCollectionCommand r, CancellationToken ct = default)
        {
            var all = await _db.Set<BookmarkCollection>().Where(c => c.UserId == r.UserId).ToListAsync(ct);
            if (all.All(c => c.Id != r.Id)) return ApiResult.FromError<int>("کالکشن وجود ندارد.");
            var ids = BookmarksQueryHandler.Descendants(all.Select(a => (a.Id, a.ParentId)).ToList(), r.Id);
            var bms = await _db.Set<Bookmark>().Include(b => b.Tags).Where(b => b.UserId == r.UserId && b.CollectionId != null && ids.Contains(b.CollectionId.Value)).ToListAsync(ct);
            foreach (var b in bms) b.Update(b.Url, b.Title, b.Note, b.ImageUrl, b.Kind, null, b.Tags.ToList());
            _db.Set<BookmarkCollection>().RemoveRange(all.Where(c => ids.Contains(c.Id)));
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(r.Id);
        }
    }

    // ---- tags: rename (ادغام در صورت وجود) / delete
    public class RenameBookmarkTagCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public string? Old { get; init; } public string? New { get; init; } }
    internal class RenameBookmarkTagCommandHandler : IRequestHandler<RenameBookmarkTagCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RenameBookmarkTagCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RenameBookmarkTagCommand r, CancellationToken ct = default)
        {
            var o = (r.Old ?? "").Trim().ToLowerInvariant(); var n = (r.New ?? "").Trim().ToLowerInvariant();
            if (n.Length == 0 || n.Length > 50) return ApiResult.FromError<int>("نام تگ معتبر نیست.");
            var tags = await _db.Set<BookmarkTag>().Where(t => t.UserId == r.UserId && (t.Name == o || t.Name == n)).ToListAsync(ct);
            var src = tags.FirstOrDefault(t => t.Name == o);
            if (src is null) return ApiResult.FromError<int>("تگ وجود ندارد.");
            var dst = tags.FirstOrDefault(t => t.Name == n);
            if (dst is null || dst == src) { src.Rename(n); }
            else
            {
                var bms = await _db.Set<Bookmark>().Include(b => b.Tags).Where(b => b.UserId == r.UserId && b.Tags.Any(t => t.Id == src.Id)).ToListAsync(ct);
                foreach (var b in bms) { b.Tags.RemoveAll(t => t.Id == src.Id); if (b.Tags.All(t => t.Id != dst.Id)) b.Tags.Add(dst); }
                _db.Set<BookmarkTag>().Remove(src);
            }
            await _db.SaveChangesAsync(ct);
            return ApiResult.From(1);
        }
    }

    public class RemoveBookmarkTagCommand : IRequest<ApiResult<int>> { public required int UserId { get; init; } public string? Name { get; init; } }
    internal class RemoveBookmarkTagCommandHandler : IRequestHandler<RemoveBookmarkTagCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemoveBookmarkTagCommandHandler(IDbContext db) => _db = db;
        public async Task<ApiResult<int>> HandleAsync(RemoveBookmarkTagCommand r, CancellationToken ct = default)
        {
            var n = (r.Name ?? "").Trim().ToLowerInvariant();
            var t = await _db.Set<BookmarkTag>().FirstOrDefaultAsync(x => x.UserId == r.UserId && x.Name == n, ct);
            if (t is null) return ApiResult.FromError<int>("تگ وجود ندارد.");
            _db.Set<BookmarkTag>().Remove(t); await _db.SaveChangesAsync(ct);
            return ApiResult.From(1);
        }
    }

    // ---- preview
    public class UrlMetadataQuery : IRequest<ApiResult<UrlMetadata>> { public string? Url { get; init; } }
    internal class UrlMetadataQueryHandler : IRequestHandler<UrlMetadataQuery, ApiResult<UrlMetadata>>
    {
        public async Task<ApiResult<UrlMetadata>> HandleAsync(UrlMetadataQuery r, CancellationToken ct = default)
        {
            var m = await UrlMetadataFetcher.FetchAsync((r.Url ?? "").Trim(), ct);
            return m is null ? ApiResult.FromError<UrlMetadata>("آدرس معتبر نیست.") : ApiResult.From(m);
        }
    }
}
