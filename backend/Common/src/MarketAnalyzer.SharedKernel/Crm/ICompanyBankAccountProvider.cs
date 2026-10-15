using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.SharedKernel
{
    public interface ICompanyBankAccountProvider
    {
        Task<List<CompanyBankAccountInfo>> GetAsync(CancellationToken cancellation = default);
        Task<CompanyBankAccountInfo> GetByIdAsync(int bankId, CancellationToken cancellation = default);
    }
}
