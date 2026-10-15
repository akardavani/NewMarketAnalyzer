using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// دسته‌بندی اخبار
    /// </summary>
    public class NewsCategory : AggregateRoot<int>
    {
        public NewsCategory(int id) : base(id)
        {
        }

        #region [Properties]

        public string Name { get; private set; }
        public string Description { get; private set; }

        private List<News> _news;
        public IReadOnlyCollection<News> News => _news ??= new List<News>();

        #endregion

        #region [Methods]

        public static NewsCategory Create(
            string name,
            string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            return new(0)
            {
                Name = name,
                Description = description
            };
        }

        public void Update(
            string name,
            string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }
            Name= name;
            Description= description;
        }

        #endregion
    }
}
