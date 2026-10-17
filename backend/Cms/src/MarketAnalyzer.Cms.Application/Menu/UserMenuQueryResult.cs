using System.Collections.Generic;

namespace MarketAnalyzer.Cms.Application
{
    public class UserMenuQueryResult
    {
        /// <summary>
        /// عنوان منو
        /// </summary>
        public required string Label { get; init; }
        /// <summary>
        /// آدرس کامپوننت
        /// </summary>
        public required string Path { get; init; }
        /// <summary>
        /// آیکون
        /// </summary>
        public required string Icon { get; init; }
        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public required int DisplayOrder { get; init; }
        /// <summary>
        /// زیرمنوها
        /// </summary>
        public required IReadOnlyCollection<UserMenuQueryResult> Children { get; init; }
    }
}
