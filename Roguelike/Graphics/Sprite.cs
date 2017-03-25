using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Graphics
{
    public sealed class Sprite : Transformable, IDrawable
    {
        public Texture2D Texture { get; }
        public Rectangle LocalBounds => Texture.Bounds;
        public Color Color { get; set; } = Color.White;

        public Sprite(Texture2D texture)
        {
            Texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, LocalBounds, Color, 
                Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
