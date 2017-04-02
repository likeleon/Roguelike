using System.Collections.Generic;

namespace Roguelike.Utils
{
    public static class IDictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value = defaultValue;
            dict.TryGetValue(key, out value);
            return value;
        }
    }
}
