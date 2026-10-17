using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// نوع فایل
    /// </summary>
    public class FileType : AuditableEntity<int>
    {
        public FileType(int id) : base(id)
        {
        }

        /// <summary>
        /// پسوند فایل
        /// </summary>
        public string Extension { get; private set; }
        /// <summary>
        /// Mime نوع
        /// </summary>
        public string MimeType { get; private set; }

        public static FileType Create(string extension, string mimeType)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"'{nameof(extension)}' cannot be null or empty.", nameof(extension));
            }

            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentException($"'{nameof(mimeType)}' cannot be null or empty.", nameof(mimeType));
            }

            return new FileType(0)
            {
                Extension = extension,
                MimeType = mimeType
            };
        }

        public void Update(string mimeType)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentException($"'{nameof(mimeType)}' cannot be null or empty.", nameof(mimeType));
            }

            MimeType = mimeType;
        }
    }
}
