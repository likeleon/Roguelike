using Microsoft.Xna.Framework;

namespace Roguelike
{
    public static class ExtensionMethods
    {
        public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
    }
}
