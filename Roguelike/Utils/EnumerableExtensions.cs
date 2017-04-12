using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Utils
{
    public static class EnumerableExtensions
    {
        public static void Times(this int count, Action action)
        {
            for (int i = 0; i < count; ++i)
                action();
        }

        public static void Times(this int count, Action<int> action)
        {
            for (int i = 0; i < count; ++i)
                action(i);
        }

        public static T WithMinimum<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> func)
            where T : class
            where TKey : IComparable<TKey>
        {
            return enumerable
                .Select(e => Tuple.Create(e, func(e)))
                .Aggregate((Tuple<T, TKey>)null, (best, cur) => best == null || cur.Item2.CompareTo(best.Item2) < 0 ? cur : best)
                .Item1;
        }
    }
}
