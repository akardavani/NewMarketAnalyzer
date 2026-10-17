using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// امیدنامه
    /// </summary>
    public class Prospectus : AggregateRoot<int>
    {
        public Prospectus(int id) : base(id)
        {
        }

        #region [Properties]


        /// <summary>
        /// شرح
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// تاریخ 
        /// </summary>
        public DateOnly Date { get; private set; }
        /// <summary>
        /// پیوست
        /// </summary>
        public string AttachmentId { get; private set; }

        #endregion

        #region [Methods]

        public static Prospectus Create(
            string description,
            string attachmentId,
            DateOnly date)
        {

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
            }

            if (string.IsNullOrEmpty(attachmentId))
            {
                throw new ArgumentException($"'{nameof(attachmentId)}' cannot be null or empty.", nameof(attachmentId));
            }

            return new Prospectus(0)
            {
                Description = description,
                AttachmentId = attachmentId,
                Date = date
            };
        }
        #endregion
    }
}
