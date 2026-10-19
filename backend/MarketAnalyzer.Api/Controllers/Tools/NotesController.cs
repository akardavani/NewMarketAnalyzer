using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "یادداشت‌ها")]
    public class NotesController : FundControllerBase
    {
        public class NoteRequest
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Content { get; set; }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<NoteItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await Mediator.ExecuteAsync(new NotesQuery { UserId = CurrentUserId }, ct);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(NoteRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new CreateNoteCommand { UserId = CurrentUserId, Title = r.Title, Content = r.Content }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(NoteRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new UpdateNoteCommand { Id = r.Id, UserId = CurrentUserId, Title = r.Title, Content = r.Content }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(NoteRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemoveNoteCommand { Id = r.Id, UserId = CurrentUserId }, ct));
    }
}
