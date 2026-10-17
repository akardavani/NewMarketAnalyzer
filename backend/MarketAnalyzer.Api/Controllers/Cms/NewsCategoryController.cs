using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Api.Models.Cms;
using MarketAnalyzer.Cms.Application;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    [RouteInfo(DisplayName = "دسته‌بندی اخبار")]
    public class NewsCategoryController : FundControllerBase
    {

        [HttpGet]
        [RouteInfo(DisplayName = "لیست دسته‌بندی اخبار")]
        [ProducesResponseType(typeof(ApiResult<QueryResult<NewsCategoriesQueryResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParam request, CancellationToken cancellationToken)
        {
            var query = new NewsCategoriesQuery()
            {
                QueryParam = request
            };
            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }

        [HttpGet]
        [RouteInfo(DisplayName = "مشاهده جزئیات", AuthorizedRoles = [ConstStrings.TrusteeRoleName])]
        [ProducesResponseType(typeof(ApiResult<NewsCategoryQueryResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int id, CancellationToken cancellationToken)
        {
            var query = new NewsCategoryQuery()
            {
                Id = id
            };

            var result = await Mediator.ExecuteAsync(query, cancellationToken);
            return Ok(ApiResult.From(result));
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateNewsCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateNewsCategoryCommand()
            {
                Name = request.Name,
                Description = request.Description,
                OperatorId = CurrentUserId
            };
            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(UpdateNewsCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateNewsCategoryCommand()
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                OperatorId = CurrentUserId
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(RemoveNewsCategoryRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveNewsCategoryCommand()
            {
                Id = request.Id,
                OperatorId = CurrentUserId,
            };

            var result = await Mediator.ExecuteAsync(command, cancellationToken);
            return Ok(result);
        }

    }
}
