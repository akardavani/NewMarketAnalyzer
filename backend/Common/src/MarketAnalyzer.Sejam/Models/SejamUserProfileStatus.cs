namespace MarketAnalyzer.Sejam.Models
{
    public enum SejamUserProfileStatus
    {
        /// <summary>
        /// وضعیت اولیه
        /// </summary>
        Init,
        /// <summary>
        /// پرداخت موفق
        /// </summary>
        SuccessPayment,
        /// <summary>
        /// تعهدات
        /// </summary>
        PolicyAccepted,
        /// <summary>
        /// استعلام اطلاعات
        /// </summary>
        PendingValidation,
        /// <summary>
        /// اطلاعات نامعتبر
        /// </summary>
        InvalidInformation,
        /// <summary>
        /// کد پیگیری
        /// </summary>
        TraceCode,
        /// <summary>
        /// سجامی
        /// </summary>
        Sejami,
        /// <summary>
        /// سجامی ناقص منقضی شده
        /// </summary>
        Suspend,
        /// <summary>
        /// فوت شده
        /// </summary>
        Dead,
        /// <summary>
        /// سجامی ناقص به دلیل نقص مدارک / احراز هویت
        /// </summary>
        SemiSejami
    }
}