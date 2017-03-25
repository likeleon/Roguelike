using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;

namespace Roguelike
{
    public abstract class Object
    {
        private readonly Sprite _sprite;

        public Object(Texture2D texture)
        {
            _sprite = new Sprite(texture)
            {
                Origin = texture.Bounds.Size.ToVector2() / 2.0f
            };
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _sprite.Draw(spriteBatch);
        }

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }
    }
}
