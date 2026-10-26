using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarketAnalyzer.MarketIntelligence.Persistence
{
    /// <summary>
    /// تشخیص نقض ایندکس یکتا در SQL Server. وقتی دو نویسندهٔ همزمان یک رکورد جدید یکسان را درج کنند،
    /// یکی از آن‌ها این خطا را می‌گیرد؛ Storeها در این حالت یک بار دوباره تلاش می‌کنند (این بار رکورد را موجود می‌بینند).
    /// </summary>
    internal static class UniqueViolation
    {
        /// <summary>کد خطای SQL Server برای تکرار کلید یکتا در ایندکس (2601) و در constraint (2627).</summary>
        private const int DuplicateIndexRow = 2601;
        private const int DuplicateKeyConstraint = 2627;

        /// <summary>آیا این استثنا ناشی از نقض کلید یکتا است؟</summary>
        public static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sql
                   && (sql.Number == DuplicateIndexRow || sql.Number == DuplicateKeyConstraint);
        }
    }
}
