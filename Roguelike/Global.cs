using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike
{
    public static class Global
    {
        public static ContentManager Content { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static SpriteFont Font { get; set; }

        public static bool PathDebugIsEnabled { get; set; }
    }
}
