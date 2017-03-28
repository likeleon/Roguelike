using System;

namespace Roguelike
{
    public static class EnumExtensions
    {
        public static int GetEnumLength<T>() => Enum.GetValues(typeof(T)).Length;
    }
}
