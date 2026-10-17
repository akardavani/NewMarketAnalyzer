namespace MarketAnalyzer.Api.Models
{
    public class UpdateRoleRequest
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام نمایشی
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// فعال
        /// </summary>
        public bool IsActive { get; set; }
    }
}
