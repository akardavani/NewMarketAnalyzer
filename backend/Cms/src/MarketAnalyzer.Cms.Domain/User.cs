using MarketAnalyzer.Abstractions;

namespace MarketAnalyzer.Cms.Domain
{
    public class User : AuditableEntity<int>
    {
        public User(int id) : base(id)
        {
        }

        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// نام
        /// </summary>
        public string FullName { get; private set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; private set; }
        /// <summary>
        /// حذف شده
        /// </summary>
        public bool Deleted { get; private set; }

        public static User Create(int id, string userName, string fullName)
        {
            return new User(id)
            {
                UserName = userName,
                FullName = fullName,
                IsActive = true,
                Deleted = false
            };
        }
    }
}
