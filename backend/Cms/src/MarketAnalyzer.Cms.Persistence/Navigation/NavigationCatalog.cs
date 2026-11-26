using System.Collections.Generic;

namespace MarketAnalyzer.Cms.Persistence.Navigation;

internal static class NavigationCatalog
{
    // Keep the Id when renaming or moving a menu. Set isActive: false to retire it.
    public static IReadOnlyList<NavigationEntry> Roots { get; } =
    [
                Page(1, "صفحه اصلی", 0, "/", null, icon: NavigationIcons.Home),

                Page(2, "تنظیمات داشبورد", 1, "dashboard/settings", null, icon: NavigationIcons.Settings),

                Group(100, "تحلیل بازار", 100, icon: "", children:
                [
                    Page(101, "سهم‌های برتر بازار", 0, "market-intelligence/iran-equity-main", "Strategy/GetAll"),
                    Page(102, "برگشت از کف", 1, "market-intelligence/bottom-reversal", "Strategy/GetAll"),
                    Page(103, "صف خرید", 2, "market-intelligence/iran-queue", "Strategy/GetAll"),
                    Page(104, "واگرایی اندیکاتورها", 3, "market-intelligence/multi-indicator-divergence", "Strategy/GetAll"),
                    Page(105, "نسبت نمادها", 4, "market-intelligence/ratio-candles", "Strategy/GetAll"),
                    Page(106, "YTM", 5, "market-intelligence/ytm-future", "Strategy/GetAll"),
                    Page(107, "واگرایی مثبت RSI", 6, "market-intelligence/rsi-divergence", "Strategy/GetAll")
                ]),

                Group(400, "بورس", 400, icon: "", children:
                [
                    Page(404, "اختیار", 404, "bourse/options", "Derivatives/Options"),
                    Page(413, "نمادها", 413, "bourse/instrumnets", "Instrument/GetAll")
                ]),

                Group(500, "سرمایه گذاران", 500, icon: "", children:
                [
                    Page(501, "گردش حساب مشتریان", 0, "assets/investor-account-turnover", "InvestorPortfolio/GetTurnover")
                ]),

                    Page(600, "فعالیت های جاری", 600, null, null, isActive: false),

                Group(1000, "مدیریت محتوا", 1000, icon: "", children:
                [
                    Page(1001, "اخبار", 0, "cms/news", "News/GetAll"),
                    Page(1002, "دسته بندی اخبار", 0, "cms/newsCategory", "NewsCategory/GetAll"),
                    Page(1003, "مدیریت صفحات پرتال", 0, "cms/portal-pages-management", "NewsCategory/GetAll")
                ]),

                Group(2000, "یادگیری", 1400, icon: "", children:
                [
                    Page(2001, "مجموعه‌های یادگیری", 0, "learn/collections", null),
                    Page(2002, "نمای کلی یادگیری", 1, "learn/overview", null)
                ]),

                Group(1500, "امنیت و دسترسی", 1500, icon: "", children:
                [
                    Page(801, "کاربران", 0, "membership/users", "User/GetAll"),
                    Page(802, "نقش‌ها", 0, "membership/roles", "Role/GetAll"),
                    Page(803, "گزارش ورود و خروج کاربران", 0, "membership/users-login-history", "Account/UsersLoginHistory"),
                    Page(804, "گزارش ورود و خروج ناشناس", 0, "membership/unknown-login-history", "Account/UnknownLoginHistory"),
                    Page(805, "تنظیمات دریافت داده‌های بازار", 4, "market-intelligence/settings", "MarketDataSettings/Overview"),
                    Page(108, "مانیتورینگ داده‌ها", 5, "market-intelligence/data-monitoring", "DataMonitoring/Overview")
                ])
    ];

    private static NavigationEntry Group(int id, string title, int order,
        string icon = null, bool isActive = true, int[] fundTypes = null, NavigationEntry[] children = null)
        => new(id, title, order, null, null, icon, isActive, fundTypes ?? [], children ?? []);

    private static NavigationEntry Page(int id, string title, int order, string path, string permission,
        string icon = null, bool isActive = true, int[] fundTypes = null)
        => new(id, title, order, path, permission, icon, isActive, fundTypes ?? [], []);
}

internal sealed record NavigationEntry(
    int Id, string Title, int Order, string Path, string Permission, string Icon, bool IsActive,
    IReadOnlyList<int> FundTypes, IReadOnlyList<NavigationEntry> Children);

