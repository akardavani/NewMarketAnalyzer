using System.Linq;

namespace MarketAnalyzer.Api.Models
{
    public class RolesQueryRequest : Pagination
    {
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام نمایشی
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
