using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "منابع")]
    public class ResourcesController : FundControllerBase
    {
        public class SaveRequest
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Link { get; set; }
            public List<string>? Tags { get; set; }
        }
        public class IdRequest { public int Id { get; set; } }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<ResourceItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(string? search, int skip, int take, CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new ResourcesQuery { UserId = CurrentUserId, Search = search, Skip = skip, Take = take == 0 ? 20 : take }, ct)));

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<string>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Tags(string? term, CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new ResourceTagsQuery { UserId = CurrentUserId, Term = term }, ct)));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Save(SaveRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new SaveResourceCommand { UserId = CurrentUserId, Id = r.Id, Title = r.Title, Description = r.Description, Link = r.Link, Tags = r.Tags }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(IdRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemoveResourceCommand { Id = r.Id, UserId = CurrentUserId }, ct));

        /// <summary>fileId=0 → افزودن، وگرنه جایگزینی همان فایل</summary>
        [HttpPost]
        [RequestSizeLimit(21 * 1024 * 1024)]
        [ProducesResponseType(typeof(ApiResult<ResourceFileItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFile([FromForm] int resourceId, [FromForm] int fileId, IFormFile file, CancellationToken ct)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            return Ok(await Mediator.ExecuteAsync(new SaveResourceFileCommand
            {
                UserId = CurrentUserId, ResourceId = resourceId, FileId = fileId,
                FileName = file.FileName, ContentType = file.ContentType, Content = ms.ToArray()
            }, ct));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveFile(IdRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemoveResourceFileCommand { UserId = CurrentUserId, FileId = r.Id }, ct));

        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadFile(int id, CancellationToken ct)
        {
            var f = await Mediator.ExecuteAsync(new ResourceFileQuery { UserId = CurrentUserId, FileId = id }, ct);
            return f is null ? NotFound() : File(f.Content, f.ContentType, f.FileName);
        }
    }
}
