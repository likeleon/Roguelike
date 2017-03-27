using System;

namespace Roguelike
{
    public static class Utils
    {
        public static int GetEnumLength<T>() => Enum.GetValues(typeof(T)).Length;
    }
}
