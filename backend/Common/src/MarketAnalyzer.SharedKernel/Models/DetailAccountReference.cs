using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public sealed class DetailAccountReference
    {
        public required DetailAccountTypeIndicator Type { get; init; }

        public required IReadOnlyCollection<string> Identifiers { get; init; }
    }
}