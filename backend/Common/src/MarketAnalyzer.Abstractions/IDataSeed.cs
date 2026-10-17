using System.Threading.Tasks;

namespace MarketAnalyzer.Abstractions
{
    public interface IDataSeed
    {
        /// <summary>
        /// ترتیب اجرا
        /// </summary>
        int Order { get; }
        /// <summary>
        /// کاشت داده‌ها
        /// </summary>
        /// <returns></returns>
        Task SeedAsync();
    }
}
