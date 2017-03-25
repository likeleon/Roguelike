using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Graphics
{
    public sealed class Sprite : Transformable, IDrawable
    {
        public Texture2D Texture { get; }
        public Color Color { get; set; } = Color.White;

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            TextureRect = texture.Bounds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, TextureRect, Color, 
                Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        public Rectangle TextureRect { get; set; }
    }
}
