using MarketAnalyzer.Abstractions;
using System.Collections.Generic;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// پیوست‌
    /// </summary>
    public class Attachment : ValueObject
    {
        public Attachment(string subject, string fileId, string mimeType)
        {
            Subject = subject;
            FileId = fileId;
            MimeType = mimeType;
        }

        public string Subject { get; }
        public string FileId { get; }
        public string MimeType { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Subject;
            yield return FileId;
            yield return MimeType;
        }
    }
}

