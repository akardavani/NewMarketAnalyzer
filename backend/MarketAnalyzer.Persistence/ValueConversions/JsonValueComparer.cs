using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Text.Json;

namespace MarketAnalyzer.Persistence.ValueConversions
{
    internal class JsonValueComparer<T> : ValueComparer<T>
    {
        private static string Json(T instance)
        {
            return JsonSerializer.Serialize(instance);
        }

        private static T DoGetSnapshot(T instance)
        {
            if (instance is ICloneable cloneable)
            {
                return (T)cloneable.Clone();
            }

            return (T)JsonSerializer
                .Deserialize(Json(instance), typeof(T));
        }

        private static int DoGetHashCode(T instance)
        {
            if (instance is IEquatable<T>)
            {
                return instance.GetHashCode();
            }

            return Json(instance).GetHashCode();
        }

        private static bool DoEquals(T left, T right)
        {
            if (left is IEquatable<T> equatable)
            {
                return equatable.Equals(right);
            }

            return Json(left).Equals(Json(right));
        }

        public JsonValueComparer() : base(
          (t1, t2) => DoEquals(t1, t2),
          t => DoGetHashCode(t),
          t => DoGetSnapshot(t))
        {
        }
    }
}

