using Microsoft.EntityFrameworkCore;
using MarketAnalyzer.Abstractions;
using MarketAnalyzer.Cms.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Cms.Persistence.Navigation;

internal static class NavigationCatalogSeed
{
    public static async Task SyncAsync(IDbContext context)
    {
        var entries = FlattenCatalog();
        var menus = context.Set<Menu>();
        var existing = await menus.ToDictionaryAsync(menu => menu.Id);

        foreach (var (entry, parentId) in entries)
        {
            var parent = parentId is int id && existing.TryGetValue(id, out var parentMenu)
                ? parentMenu
                : null;

            if (!existing.TryGetValue(entry.Id, out var menu))
            {
                menu = Menu.Create(entry.Id, entry.Title, entry.Path, entry.Order, entry.Icon,
                    entry.Permission, entry.IsActive, entry.FundTypes);
                menu.SetParent(parent);
                menus.Add(menu);
                existing.Add(entry.Id, menu);
                continue;
            }

            menu.Update(entry.Title, entry.Path, entry.Order, entry.Icon, entry.Permission, entry.IsActive);
            menu.SetRequiredInvestmentTypes(entry.FundTypes);
            menu.SetParent(parent);
        }

        await context.SaveChangesAsync();
    }

    private static List<(NavigationEntry Entry, int? ParentId)> FlattenCatalog()
    {
        var result = new List<(NavigationEntry, int?)>();
        var ids = new HashSet<int>();
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Visit(NavigationCatalog.Roots, null);
        return result;

        void Visit(IReadOnlyList<NavigationEntry> entries, int? parentId)
        {
            foreach (var entry in entries)
            {
                if (entry.Id <= 0 || !ids.Add(entry.Id) || string.IsNullOrWhiteSpace(entry.Title)
                    || entry.Title.Length > 200 || entry.Icon?.Length > 20 || entry.Path?.Length > 300
                    || entry.Permission?.Length > 500 || (entry.Path is not null
                        && (!paths.Add(entry.Path) || entry.Path.Length == 0 || entry.Children.Count != 0
                            )))
                {
                    throw new InvalidOperationException($"Invalid navigation catalog entry: {entry.Id} ({entry.Title}).");
                }

                result.Add((entry, parentId));
                Visit(entry.Children, entry.Id);
            }
        }
    }
}
