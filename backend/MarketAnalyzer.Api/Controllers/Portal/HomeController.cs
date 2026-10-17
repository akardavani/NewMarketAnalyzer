using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.ApiClient.Models;
using MarketAnalyzer.Cms.Application;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers.Portal
{
    public class HomeController : PortalControllerBase
    {
        [HttpPost]
        [RouteIgnore]
        [InternalApi]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Contactus([FromBody] ContactusRequest request, CancellationToken cancellation)
        {
            var command = new CreateContactUsCommand()
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Subject = request.Subject,
                Text = request.Text,
                IpAddress = request.IpAddress
            };
            var result = await Mediator.ExecuteAsync(command, cancellation);
            return Ok(ApiResult.From(result));
        }
    }
}
