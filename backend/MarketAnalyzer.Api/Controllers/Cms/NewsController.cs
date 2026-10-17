using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Cms.Application;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "اخبار")]
    public class NewsController : FundControllerBase
    {
        [HttpGet]
        [RouteInfo(DisplayName = "لیست اخبار", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<QueryResult<NewsesQueruResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParam request, CancellationToken cancellationToken)
        {
            var query = new NewsesQuery()
            {
                QueryParam = request
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "مشاهده جزئیات", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<NewsQueruResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int id, CancellationToken cancellationToken)
        {
            var query = new NewsQuery()
            {
                Id = id
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateNewsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateNewsCommand()
            {
                Title = request.Title,
                Source = request.Source,
                Summary = request.Summary,
                Text = request.Text,
                IsActive = request.IsActive,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateNewsRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateNewsCommand()
            {
                Id = request.Id,
                Title = request.Title,
                Source = request.Source,
                Summary = request.Summary,
                Text = request.Text,
                OperatorId = CurrentUserId
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(RemoveNewsRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveNewsCommand()
            {
                Id = request.Id,
                OperatorId = CurrentUserId,
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(result);
        }
    }
}
