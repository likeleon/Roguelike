using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike
{
    /// <summary>
    /// Drawable representation of a texture 
    /// </summary>
    public sealed class Sprite
    {
        public Texture2D Texture { get; }

        /// <summary>
        /// Position of the sprite
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Local origin of the sprite
        /// </summary>
        public Vector2 Origin { get; set; }

        public Sprite(Game game, Texture2D texture)
        {
            Texture = texture;
        }
    }
}
