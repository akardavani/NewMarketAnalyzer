using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using MarketAnalyzer.Tools.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "لینک‌ها")]
    public class BookmarksController : FundControllerBase
    {
        public class SaveRequest
        {
            public int Id { get; set; }
            public string? Url { get; set; }
            public string? Title { get; set; }
            public string? Note { get; set; }
            public string? ImageUrl { get; set; }
            public BookmarkKind Kind { get; set; }
            public int? CollectionId { get; set; }
            public List<string>? Tags { get; set; }
        }
        public class IdRequest { public int Id { get; set; } }
        public class FavoriteRequest { public int Id { get; set; } public bool Value { get; set; } }
        public class CollectionRequest { public int Id { get; set; } public string? Name { get; set; } public int? ParentId { get; set; } }
        public class RenameTagRequest { public string? Old { get; set; } public string? New { get; set; } }
        public class TagRequest { public string? Name { get; set; } }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<BookmarkItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(string? search, string? tag, int? collectionId, bool favoritesOnly, BookmarkKind? kind, DateTime? from, DateTime? to, int skip, int take, CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new BookmarksQuery
            { UserId = CurrentUserId, Search = search, Tag = tag, CollectionId = collectionId, FavoritesOnly = favoritesOnly, Kind = kind, From = from, To = to, Skip = skip, Take = take == 0 ? 20 : take }, ct)));

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<CollectionItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Collections(CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new CollectionsQuery { UserId = CurrentUserId }, ct)));

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<TagItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Tags(string? term, CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new BookmarkTagsQuery { UserId = CurrentUserId, Term = term }, ct)));

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<UrlMetadata>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Preview(string url, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new UrlMetadataQuery { Url = url }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Save(SaveRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new SaveBookmarkCommand { UserId = CurrentUserId, Id = r.Id, Url = r.Url, Title = r.Title, Note = r.Note, ImageUrl = r.ImageUrl, Kind = r.Kind, CollectionId = r.CollectionId, Tags = r.Tags }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetFavorite(FavoriteRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new SetBookmarkFavoriteCommand { UserId = CurrentUserId, Id = r.Id, Value = r.Value }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(IdRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemoveBookmarkCommand { UserId = CurrentUserId, Id = r.Id }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveCollection(CollectionRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new SaveCollectionCommand { UserId = CurrentUserId, Id = r.Id, Name = r.Name, ParentId = r.ParentId }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveCollection(IdRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemoveCollectionCommand { UserId = CurrentUserId, Id = r.Id }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RenameTag(RenameTagRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RenameBookmarkTagCommand { UserId = CurrentUserId, Old = r.Old, New = r.New }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveTag(TagRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemoveBookmarkTagCommand { UserId = CurrentUserId, Name = r.Name }, ct));
    }
}
