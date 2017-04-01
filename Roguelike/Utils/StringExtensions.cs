namespace Roguelike.Utils
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        public static string F(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
