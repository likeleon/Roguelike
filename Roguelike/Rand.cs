//#define FIXED_SEED

using System;

namespace Roguelike
{
    public static class Rand
    {
#if FIXED_SEED
        private static readonly int _seed = 0;
#else
        private static readonly int _seed = Guid.NewGuid().GetHashCode();
#endif
        private static readonly Lazy<Random> _random = new Lazy<Random>(() => new Random(_seed));

        public static int Next() => _random.Value.Next();
        public static int Next(int maxValue) => _random.Value.Next(maxValue);
        public static int Next(int minValue, int maxValue) => _random.Value.Next(minValue, maxValue);
    }
}
