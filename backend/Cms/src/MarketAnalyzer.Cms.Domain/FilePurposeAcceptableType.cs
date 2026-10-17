using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// نوع قابل پذیرش برای موضوع فایل
    /// </summary>
    public class FilePurposeAcceptableType : AuditableEntity
    {
        public const string PurposeId = nameof(PurposeId);
        public const string FileTypeId = nameof(FileTypeId);

        /// <summary>
        /// نوع فایل
        /// </summary>
        public FileType FileType { get; private set; }
        /// <summary>
        /// حداکثر حجم فایل
        /// <para>مقدار 0 و کوچکتر برابر نامحدود در نظر گرفته میشود</para>
        /// </summary>
        public long MaxSize { get; private set; }

        internal static FilePurposeAcceptableType Create(FileType fileType, long maxSize)
        {
            if (fileType is null)
            {
                throw new ArgumentNullException(nameof(fileType));
            }

            return new FilePurposeAcceptableType()
            {
                FileType = fileType,
                MaxSize = maxSize
            };
        }

        protected override bool Equals(EntityBase other)
        {
            return other is FilePurposeAcceptableType entity &&
                FileType.Equals(entity.FileType) &&
                MaxSize == entity.MaxSize;
        }

        protected override int GetHashCodeCore()
        {
            return FileType.GetHashCode() ^ MaxSize.GetHashCode();
        }
    }
}
