using System;

namespace Roguelike
{
    public static class RandomGenerator
    {
        private static readonly Lazy<Random> _random = new Lazy<Random>();

        public static int Next() => _random.Value.Next();
        public static int Next(int maxValue) => _random.Value.Next(maxValue);
        public static int Next(int minValue, int maxValue) => _random.Value.Next(minValue, maxValue);
    }
}
