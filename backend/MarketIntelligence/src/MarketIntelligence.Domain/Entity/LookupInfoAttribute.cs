namespace MarketIntelligence.Domain
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LookupInfoAttribute : Attribute
    {
        public LookupInfoAttribute(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; }
        public string Name { get; }
    }

}
