using System;
using System.Collections;
using System.Collections.Generic;

namespace Roguelike.Utils
{
    public class Cache<T, U> : IReadOnlyDictionary<T, U>
    {
        private readonly Dictionary<T, U> _cache;
        private readonly Func<T, U> _loader;

        public Cache(Func<T, U> loader, IEqualityComparer<T> comparer)
        {
            _loader = loader;
            _cache = new Dictionary<T, U>(comparer);
        }

        public Cache(Func<T, U> loader)
            : this(loader, EqualityComparer<T>.Default)
        {
        }

        public U this[T key]
        {
            get
            {
                U result;
                if (!_cache.TryGetValue(key, out result))
                    _cache.Add(key, result = _loader(key));
                return result;
            }
        }

        public bool ContainsKey(T key) => _cache.ContainsKey(key);
        public bool TryGetValue(T key, out U value) => _cache.TryGetValue(key, out value);

        public int Count => _cache.Count;

        IEnumerable<T> IReadOnlyDictionary<T, U>.Keys => _cache.Keys;
        IEnumerable<U> IReadOnlyDictionary<T, U>.Values => _cache.Values;

        public IEnumerator<KeyValuePair<T, U>> GetEnumerator() => _cache.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
