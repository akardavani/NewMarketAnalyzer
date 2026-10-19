//using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Tools.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Ganss.Xss;

namespace MarketAnalyzer.Tools.Application
{
    public class NoteItem
    {
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string Content { get; init; } = "";
        public DateTime LastModificationTime { get; init; }
    }

    // ---- list
    public class NotesQuery : IRequest<List<NoteItem>>
    {
        public required int UserId { get; init; }
    }

    internal class NotesQueryHandler : IRequestHandler<NotesQuery, List<NoteItem>>
    {
        private readonly IDbContext _db;
        public NotesQueryHandler(IDbContext db) => _db = db;

        public Task<List<NoteItem>> HandleAsync(NotesQuery request, CancellationToken cancellationToken = default)
            => _db.Set<Note>()
                .Where(e => e.UserId == request.UserId)
                .OrderByDescending(e => e.LastModificationTime)
                .Select(e => new NoteItem { Id = e.Id, Title = e.Title, Content = e.Content, LastModificationTime = e.LastModificationTime })
                .ToListAsync(cancellationToken);
    }

    // ---- create
    public class CreateNoteCommand : IRequest<ApiResult<int>>
    {
        public required int UserId { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
    }

    internal class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public CreateNoteCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<int>> HandleAsync(CreateNoteCommand request, CancellationToken cancellationToken = default)
        {
            var note = Note.Create(request.UserId, NoteSanitizer.Title(request.Title), NoteSanitizer.Html(request.Content));
            _db.Set<Note>().Add(note);
            await _db.SaveChangesAsync(cancellationToken);
            return ApiResult.From(note.Id);
        }
    }

    // ---- update (autosave)
    public class UpdateNoteCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required int UserId { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
    }

    internal class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public UpdateNoteCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<int>> HandleAsync(UpdateNoteCommand request, CancellationToken cancellationToken = default)
        {
            var note = await _db.Set<Note>().FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == request.UserId, cancellationToken);
            if (note is null) return ApiResult.FromError<int>("یادداشت وجود ندارد.");
            note.Update(NoteSanitizer.Title(request.Title), NoteSanitizer.Html(request.Content));
            await _db.SaveChangesAsync(cancellationToken);
            return ApiResult.From(note.Id);
        }
    }

    // ---- remove
    public class RemoveNoteCommand : IRequest<ApiResult<int>>
    {
        public required int Id { get; init; }
        public required int UserId { get; init; }
    }

    internal class RemoveNoteCommandHandler : IRequestHandler<RemoveNoteCommand, ApiResult<int>>
    {
        private readonly IDbContext _db;
        public RemoveNoteCommandHandler(IDbContext db) => _db = db;

        public async Task<ApiResult<int>> HandleAsync(RemoveNoteCommand request, CancellationToken cancellationToken = default)
        {
            var note = await _db.Set<Note>().FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == request.UserId, cancellationToken);
            if (note is null) return ApiResult.FromError<int>("یادداشت وجود ندارد.");
            _db.Set<Note>().Remove(note);
            await _db.SaveChangesAsync(cancellationToken);
            return ApiResult.From(note.Id);
        }
    }

    internal static class NoteSanitizer
    {
        private static readonly HtmlSanitizer Sanitizer = Create();

        private static HtmlSanitizer Create()
        {
            var s = new HtmlSanitizer();
            s.AllowedTags.Clear();
            foreach (var t in new[] { "p", "br", "strong", "b", "em", "i", "u", "s", "h1", "h2", "h3", "ul", "ol", "li", "a", "blockquote", "span", "div", "input", "label" })
                s.AllowedTags.Add(t);
            s.AllowedAttributes.Clear();
            foreach (var a in new[] { "href", "target", "rel", "dir", "data-type", "data-checked", "type", "checked", "disabled" })
                s.AllowedAttributes.Add(a);
            s.AllowedSchemes.Clear();
            foreach (var a in new[] { "http", "https", "mailto" }) s.AllowedSchemes.Add(a);
            return s;
        }

        public static string Html(string? html) => string.IsNullOrWhiteSpace(html) ? "" : Sanitizer.Sanitize(html);

        public static string Title(string? t)
        {
            t = (t ?? "").Trim();
            return t.Length > 200 ? t[..200] : t;
        }
    }
}
