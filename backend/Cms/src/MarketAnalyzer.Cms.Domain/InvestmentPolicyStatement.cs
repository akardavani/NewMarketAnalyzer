using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.Cms.Domain
{
    /// <summary>
    /// بیانیه سیاست های سرمایه گذاری
    /// </summary>
    public class InvestmentPolicyStatement : AggregateRoot<int>
    {
        public InvestmentPolicyStatement(int id) : base(id)
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
        /// فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; private set; }
        /// <summary>
        /// پیوست
        /// </summary>
        public string AttachmentId { get; private set; }

        #endregion

        #region [Methods]

        public static InvestmentPolicyStatement Create(
            string description,
            bool isActive,
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

            return new InvestmentPolicyStatement(0)
            {
                Description = description,
                IsActive = isActive,
                AttachmentId = attachmentId,
                Date = date
            };
        }

        #endregion
    }
}