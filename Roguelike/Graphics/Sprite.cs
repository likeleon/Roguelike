using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Graphics
{
    public sealed class Sprite : Transformable, IDrawable
    {
        private readonly Texture2D _texture;

        public Rectangle LocalBounds => _texture.Bounds;
        public Color Color { get; set; } = Color.White;

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, LocalBounds, Color, 
                Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
