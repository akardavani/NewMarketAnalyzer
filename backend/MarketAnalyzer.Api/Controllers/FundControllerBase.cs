using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Membership.Domain;
using System;
using System.Net;
using System.Text;

namespace MarketAnalyzer.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:version}/[controller]/[action]")]
    public abstract class FundControllerBase : ControllerBase
    {
        private IMediator _mediator;
        private int? _currentUserId;
        private int? _applicationId;

        public IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        public int CurrentUserId =>
            _currentUserId ??= User.GetUserId();

        public int ApplicationId =>
            _applicationId ??= User.GetApplicationId();

        protected IActionResult ReportResult(ReportResult reportResult)
        {
            ArgumentNullException.ThrowIfNull(reportResult);

            if (reportResult.ContentType == "text/plain")
            {
                return Content(Encoding.UTF8.GetString(reportResult.Data), reportResult.ContentType);
            }

            HttpContext.Response.Headers.Append("X-File-Name", reportResult.FileName);
            return File(
                reportResult.Data,
                reportResult.ContentType,
                reportResult.FileName,
                true);
        }

        protected IActionResult File(FileInfo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            string encodedFileName = WebUtility.UrlEncode(file.FileName);
            HttpContext.Response.Headers.Append("X-File-Name", encodedFileName);

            return File(
                file.Content,
                file.MimeType,
                file.FileName,
                true);
        }
    }
}
