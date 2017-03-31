using System;

namespace Roguelike.Utils
{
    public static class EnumerableExtensions
    {
        public static void Times(this int count, Action action)
        {
            for (int i = 0; i < count; ++i)
                action();
        }
    }
}
