using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.SharedKernel
{
    public interface IUserAlertCompanyBankAccountsService
    {
        Task<List<UserAlertResult>> GetUserAlertAsync(UserAlertRequest request, CancellationToken cancellation);
    }
}
