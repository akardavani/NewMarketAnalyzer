using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace System
{
    public static class SqlDbFunctions
    {
        public static DateOnly Convert(this DbFunctions _, DateTime dateTime)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Convert)));
        }

        public static DateOnly? Convert(this DbFunctions _, DateTime? dateTime)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Convert)));
        }

        public static T Coalesce<T>(this DbFunctions _, params T[] values)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Coalesce)));
        }

        public static long RowNumber(this DbFunctions _, object orderBy, SortType sortType)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RowNumber)));
        }

        public static string Stuff(this DbFunctions _, string characterExpression, int start, int length, string replaceWithExpression)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Stuff)));
        }

        public static decimal? Cast(this DbFunctions _, decimal? expression, int precision, int scale)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Cast)));
        }

        public static decimal SumOver(this DbFunctions _, decimal expression)
        {
            throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(SumOver)));
        }
    }
}
