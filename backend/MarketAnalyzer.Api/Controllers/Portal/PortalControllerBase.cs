using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MarketAnalyzer.Api.Controllers.Portal
{
    [ApiController]
    [Area("Portal")]
    [ApiVersion("1.0")]
    [Route("v{version:version}/[area]/[controller]/[action]")]
    public abstract class PortalControllerBase : FundControllerBase
    {

    }
}
