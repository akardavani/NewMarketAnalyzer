using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Sejam;
using MarketAnalyzer.Sejam.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace MarketAnalyzer.Membership.Application
{
    internal class ResendSejamKycQueryHandler : IRequestHandler<ResendSejamKycQuery, int>
    {
        private readonly SejamClient _sejamClient;

        public ResendSejamKycQueryHandler(SejamClient sejamClient)
        {
            _sejamClient = sejamClient;
        }

        public async Task<int> HandleAsync(ResendSejamKycQuery request, CancellationToken cancellationToken = default)
        {
            await _sejamClient.GetKycCodeAsync(new KycCodeRequest()
            {
                UniqueIdentifier = request.NationalCode
            }, cancellationToken);

            return 120000;//TODO: get lifeTime
        }
    }
}
