using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Graphics
{
    public sealed class Sprite : Transformable, IDrawable
    {
        public Texture2D Texture { get; private set; }
        public Rectangle TextureRect { get; set; }
        public Color Color { get; set; } = Color.White;

        public Sprite()
        {
        }

        public Sprite(Texture2D texture)
        {
            SetTexture(texture);
        }

        public Sprite(Texture2D texture, Rectangle rectangle)
        {
            SetTexture(texture);
            TextureRect = rectangle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
                return;

            spriteBatch.Draw(Texture, Position, TextureRect, Color, 
                Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        public void SetTexture(Texture2D texture, bool resetRect = false)
        {
            if (resetRect || (Texture == null && TextureRect == default(Rectangle)))
                TextureRect = texture.Bounds;

            Texture = texture;
        }
    }
}
