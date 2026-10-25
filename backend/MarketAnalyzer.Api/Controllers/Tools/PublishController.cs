using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    /// <summary>فقط ماشین توسعه: Development یا Publish:Enabled=true</summary>
    [RouteInfo(DisplayName = "انتشار")]
    public class PublishController : FundControllerBase
    {
        public class SaveRequest
        {
            public int Id { get; set; }
            public string? Key { get; set; }
            public string? Title { get; set; }
            public string? Group { get; set; }
            public string? Path { get; set; }
            public string? OutputPath { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsActive { get; set; } = true;
        }
        public class IdRequest { public int Id { get; set; } }
        public class RunRequest { public List<string> Keys { get; set; } = new(); }

        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;
        public PublishController(IConfiguration config, IHostEnvironment env) { _config = config; _env = env; }

        private bool Allowed => _env.IsDevelopment() || _config.GetValue<bool>("Publish:Enabled");

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<PublishTargetItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => !Allowed ? NotFound() : Ok(ApiResult.From(await Mediator.ExecuteAsync(new PublishTargetsQuery(), ct)));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Save(SaveRequest r, CancellationToken ct)
            => !Allowed ? NotFound() : Ok(await Mediator.ExecuteAsync(new SavePublishTargetCommand
            {
                Id = r.Id, Key = r.Key, Title = r.Title, Group = r.Group, Path = r.Path,
                OutputPath = r.OutputPath, DisplayOrder = r.DisplayOrder, IsActive = r.IsActive
            }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(IdRequest r, CancellationToken ct)
            => !Allowed ? NotFound() : Ok(await Mediator.ExecuteAsync(new RemovePublishTargetCommand { Id = r.Id }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<PublishResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Run(RunRequest r, CancellationToken ct)
            => !Allowed ? NotFound() : Ok(await Mediator.ExecuteAsync(new RunPublishCommand { Keys = r.Keys }, ct));
    }
}
