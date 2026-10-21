using Microsoft.AspNetCore.Mvc;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Abstractions.Routes;
using MarketAnalyzer.Tools.Application;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Api.Controllers
{
    /// <summary>مدیریت رمز: سرور فقط داده‌ی رمزشده (ciphertext) و پارامترهای KDF را نگه می‌دارد.</summary>
    [RouteInfo(DisplayName = "مدیریت رمز")]
    public class PasswordsController : FundControllerBase
    {
        public class InitRequest { public string? Salt { get; set; } public int Iterations { get; set; } public string? VerifierCipher { get; set; } public string? VerifierIv { get; set; } }
        public class SaveRequest { public int Id { get; set; } public string? Cipher { get; set; } public string? Iv { get; set; } }
        public class IdRequest { public int Id { get; set; } }
        public class RekeyRequest : InitRequest { public List<PasswordRekeyItem>? Entries { get; set; } }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<VaultStatus>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Status(CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new VaultStatusQuery { UserId = CurrentUserId }, ct)));

        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<PasswordEntryItem>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(ApiResult.From(await Mediator.ExecuteAsync(new PasswordEntriesQuery { UserId = CurrentUserId }, ct)));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Init(InitRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new InitVaultCommand { UserId = CurrentUserId, Salt = r.Salt ?? "", Iterations = r.Iterations, VerifierCipher = r.VerifierCipher ?? "", VerifierIv = r.VerifierIv ?? "" }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Save(SaveRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new SavePasswordEntryCommand { UserId = CurrentUserId, Id = r.Id, Cipher = r.Cipher ?? "", Iv = r.Iv ?? "" }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(IdRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RemovePasswordEntryCommand { UserId = CurrentUserId, Id = r.Id }, ct));

        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Rekey(RekeyRequest r, CancellationToken ct)
            => Ok(await Mediator.ExecuteAsync(new RekeyVaultCommand { UserId = CurrentUserId, Salt = r.Salt ?? "", Iterations = r.Iterations, VerifierCipher = r.VerifierCipher ?? "", VerifierIv = r.VerifierIv ?? "", Entries = r.Entries ?? new() }, ct));
    }
}
