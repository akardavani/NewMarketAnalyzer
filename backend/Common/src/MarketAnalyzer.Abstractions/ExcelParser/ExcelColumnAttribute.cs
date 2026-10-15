using System;

namespace MarketAnalyzer.Abstractions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ExcelColumnAttribute : Attribute
    {
        public ExcelColumnAttribute(int order)
        {
            Order = order;
        }

        public ExcelColumnAttribute(string name)
        {
            Name = name;
        }

        public int Order { get; }
        public string Name { get; }
        public bool Required { get; set; }
    }
}
