using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// خبر
    /// </summary>
    public class News : AggregateRoot<int>
    {
        public News(int id) : base(id)
        {
        }

        #region [Properties]
        /// <summary>
        /// عنوان خبر
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// منبع خبر
        /// </summary>
        public string Source { get; private set; }
        /// <summary>
        /// خلاصه خبر
        /// </summary>
        public string Summary { get; private set; }
        /// <summary>
        /// متن خبر
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// تاریخ انتشار
        /// </summary>
        public DateOnly PublishDate { get; private set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; private set; }

        private List<NewsCategory> _categories;
        public IReadOnlyCollection<NewsCategory> Categories => _categories ??= new List<NewsCategory>();

        private List<Attachment> _attachments;
        public IReadOnlyCollection<Attachment> Attachments => _attachments ??= new List<Attachment>();

        #endregion

        #region [Methods]

        public static News Create(
            string title,
            string source,
            string summary,
            string text,
            DateOnly publishDate,
            bool isActive)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
            }

            if (string.IsNullOrEmpty(summary))
            {
                throw new ArgumentException($"'{nameof(summary)}' cannot be null or empty.", nameof(summary));
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            return new News(0)
            {
                Title = title,
                Source = source,
                Summary = summary,
                Text = text,
                PublishDate = publishDate,
                IsActive = isActive
            };
        }

        public void AddCategory(NewsCategory category)
        {
            if (!Categories.Contains(category))
            {
                _categories.Add(category);
            }
        }

        public void RemoveCategory(NewsCategory category)
        {
            if (!Categories.Contains(category))
            {
                throw new Exception("Category not fund.");
            }

            _categories.Remove(category);
        }

        public void AddAttachment(Attachment attachment)
        {
            if (!Attachments.Contains(attachment))
            {
                _attachments.Add(attachment);
            }
        }

        public void RemoveAttachment(Attachment attachment)
        {
            if (!Attachments.Contains(attachment))
            {
                throw new Exception("Attachment not found in attachments.");
            }

            _attachments.Remove(attachment);
        }

        public void Update(
            string title,
            string source,
            string summary,
            string text)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
            }

            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException($"'{nameof(source)}' cannot be null or empty.", nameof(source));
            }

            if (string.IsNullOrEmpty(summary))
            {
                throw new ArgumentException($"'{nameof(summary)}' cannot be null or empty.", nameof(summary));
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            Title = title;
            Source = source;
            Summary = summary;
            Text = text;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        #endregion
    }
}

