using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Cms.Domain
{
    public class Menu : AggregateRoot<int>
    {
        public const string ParentId = nameof(ParentId);

        public Menu(int id) : base(id)
        {
        }

        #region [Properties]

        /// <summary>
        /// عنوان
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// مسیر در کلاینت
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// مجوز مسیر موردنیاز
        /// <para>اگر نمایش این منو در واسط کاربری به داشتن مجوز یک مسیر نیاز دارد
        /// باید آدرس مسیر اینجا قرار گیرد در غیر اینصورت نال است</para>
        /// </summary>
        public string RequiredRoute { get; private set; }
        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public int DisplayOrder { get; private set; }
        /// <summary>
        /// آیکون
        /// </summary>
        public string Icon { get; private set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
        public bool IsActive { get; private set; }
        /// <summary>
        /// منوی سرشاخه
        /// </summary>
        public Menu Parent { get; private set; }

        private List<Menu> _subMenus;
        public IReadOnlyCollection<Menu> SubMenus => _subMenus ??= [];

        private List<int> _requiredInvestmentTypes;
        /// <summary>
        /// فعال در نوع صندوق
        /// </summary>
        public List<int> RequiredInvestmentTypes => _requiredInvestmentTypes ??= [];

        #endregion

        #region [Methods]

        public static Menu Create(
            int id,
            string title,
            string path,
            int displayOrder,
            string icon,
            string requiredRoute,
            bool isActive,
            IReadOnlyCollection<int> requiredInvestmentTypes)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
            }

            return new Menu(id)
            {
                Title = title,
                Path = path,
                DisplayOrder = displayOrder,
                Icon = icon,
                RequiredRoute = requiredRoute,
                IsActive = isActive,
                _requiredInvestmentTypes = requiredInvestmentTypes?.ToList()
            };
        }

        public void Update(
            string title,
             string path,
             int displayOrder,
             string icon,
             string requiredRoute,
             bool isActive)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));
            }

            Title = title;
            Path = path;
            DisplayOrder = displayOrder;
            Icon = icon;
            RequiredRoute = requiredRoute;
            IsActive = isActive;
        }

        public void AddSubMenu(Menu subMenu)
        {
            ArgumentNullException.ThrowIfNull(subMenu);

            if (!SubMenus.Contains(subMenu))
            {
                _subMenus.Add(subMenu);
            }
        }

        public void RemoveSubMenu(Menu subMenu)
        {
            ArgumentNullException.ThrowIfNull(subMenu);

            if (!SubMenus.Contains(subMenu))
            {
                throw new Exception($"'{nameof(subMenu.Title)}' submenu not founds in submenus.");
            }

            _subMenus.Remove(subMenu);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        #endregion
    }
}
