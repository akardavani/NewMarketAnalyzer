using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Membership.Application
{
    public class ResendSejamKycQuery : IRequest<int>
    {
        /// <summary>
        /// کد ملی
        /// </summary>
        public required string NationalCode { get; init; }
    }
}
