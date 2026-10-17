using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Api.Models;
using MarketAnalyzer.Infrastructure.Authentication.ApiKey;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers.Portal
{
    [RouteInfo(DisplayName = "مدیریت فایل")]
    public class FileStoreController : PortalControllerBase
    {
        private readonly IFileStore _fileStore;
        private readonly Regex _fileRegex = new("data:(.*);base64,(.*)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));

        public FileStoreController(IFileStore fileStore)
        {
            _fileStore = fileStore;
        }

        [HttpGet]
        [InternalApi]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Load(string id, CancellationToken cancellationToken)
        {
            var file = await _fileStore.ReadAsync(new ReadFileRequest()
            {
                FileId = id,
                OperatorId = 0
            }, cancellationToken);

            if (file is null)
            {
                return NotFound();
            }

            return File(file);
        }

        [HttpPost]
        [RouteIgnore]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload([FromBody] UploadFileRequest request, CancellationToken cancellationToken)
        {
            var match = _fileRegex.Match(request.Content);
            if (match is null)
            {
                return Ok(ApiResult.FromError<string>("محتوای فایل معتبر نمیباشد."));
            }

            var fileId = await _fileStore.WriteAsync(new WriteFileRequest()
            {
                Purpose = request.Purpose,
                Content = Convert.FromBase64String(match.Groups[2].Value),
                FileName = request.FileName,
                OperatorId = CurrentUserId
            }, cancellationToken);

            return Ok(ApiResult.From(fileId));
        }
    }
}
