using System.Collections.Generic;

namespace MarketAnalyzer.SharedKernel
{
    public class PersonCategoryInfo
    {
        public required List<int> NaturalPerson { get; init; }
        public required List<int> LegalPerson { get; init; }
    }
}
