using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// فایل
    /// </summary>
    public class FileStore : Entity<string>, IAggregateRoot
    {
        public const string FileTypeId = nameof(FileTypeId);

        public FileStore(string id) : base(id)
        {
        }

        /// <summary>
        /// نام فایل
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// حجم فایل به بایت
        /// </summary>
        public long FileSize { get; private set; }
        /// <summary>
        /// نوع فایل
        /// </summary>
        public FileType FileType { get; private set; }
        /// <summary>
        /// محتوای فایل
        /// </summary>
        public byte[] Content { get; private set; }

        public static FileStore Create(string id, string name, long fileSize, FileType fileType, byte[] content)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            ArgumentNullException.ThrowIfNull(fileType);
            ArgumentNullException.ThrowIfNull(content);

            return new FileStore(id)
            {
                Name = name,
                FileSize = fileSize,
                FileType = fileType,
                Content = content
            };
        }

        public void Update(
            string name,
            long fileSize,
            FileType fileType,
            byte[] content)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            ArgumentNullException.ThrowIfNull(fileType);
            ArgumentNullException.ThrowIfNull(content);

            Name = name;
            FileSize = fileSize;
            FileType = fileType;
            Content = content;
        }
    }
}
