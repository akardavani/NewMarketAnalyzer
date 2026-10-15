namespace MarketAnalyzer.Abstractions
{
    public class SelectOption : SelectOption<long>
    {
    }

    public class SelectOption<T>
    {
        public T Value { get; set; }
        public string Text { get; set; }

    }
}
