namespace MarketAnalyzer.SharedKernel
{
    public class IntrumentTypeAccountsRequest
    {
        public IntrumentTypeAccountsRequest()
        {

        }

        /// <summary>
        /// شناسه نوع نماد
        /// </summary>
        public required int InstrumentTypeId { get; init; }
        /// <summary>
        /// شناسه بورس محل معامله برای صندوق‌های قابل معامله
        /// <para>در صندوق‌های غیرقابل معامله نال است</para>
        /// </summary>
        public required int? ExchangeId { get; init; }

        public override bool Equals(object obj)
        {
            return obj is IntrumentTypeAccountsRequest other &&
                InstrumentTypeId == other.InstrumentTypeId &&
                ExchangeId == other.ExchangeId;
        }

        public override int GetHashCode()
        {
            return InstrumentTypeId.GetHashCode() ^ (ExchangeId?.GetHashCode() ?? 0);
        }

        public static bool operator ==(IntrumentTypeAccountsRequest entity, IntrumentTypeAccountsRequest other)
        {
            if (entity is null && other is null)
            {
                return true;
            }

            if (entity is null || other is null)
            {
                return false;
            }

            return entity.GetHashCode() == other.GetHashCode();
        }

        public static bool operator !=(IntrumentTypeAccountsRequest entity, IntrumentTypeAccountsRequest other)
        {
            return !(entity == other);
        }
    }
}
