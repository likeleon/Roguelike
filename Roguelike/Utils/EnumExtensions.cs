using System;

namespace Roguelike
{
    public static class EnumExtensions
    {
        public static int GetCount<T>() => Enum.GetValues(typeof(T)).Length;
    }
}
