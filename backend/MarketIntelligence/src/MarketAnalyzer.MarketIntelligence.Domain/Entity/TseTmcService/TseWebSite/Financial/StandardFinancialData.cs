public class StandardFinancialData
{
    public long InsCode { get; set; }

    // اطلاعات گزارش
    public string? ReportTitle { get; set; }
    public int PageId { get; set; }

    // تاریخ‌ها
    public DateTime? SentDate { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? PublishDateDEven { get; set; }

    // دوره مالی
    public string? FiscalPeriodEnd { get; set; }

    // وضعیت حسابرسی
    public string? AuditStatus { get; set; }

    // نوع گزارش (ترازنامه / سود و زیان)
    public int ReportSubType { get; set; }

    // متریک‌های مالی استاندارد شده
    public Dictionary<string, decimal?> Metrics { get; set; } = new();
}
