using System.Reflection;

namespace MarketIntelligence.Domain
{

    public static class LookupInfoExtensions
    {
        public static string GetCode<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            return value.GetLookupInfo().Code;
        }

        public static string GetName<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            return value.GetLookupInfo().Name;
        }

        private static LookupInfoAttribute GetLookupInfo<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();

            var attribute = member?
                .GetCustomAttribute<LookupInfoAttribute>();

            if (attribute is null)
                throw new InvalidOperationException(
                    $"{typeof(TEnum).Name} '{value}' does not have {nameof(LookupInfoAttribute)}.");

            return attribute;
        }
    }
}


