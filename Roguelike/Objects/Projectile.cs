using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Projectile : Object
    {
        private readonly Vector2 _velocity;

        public Projectile(Texture2D texture, Vector2 origin, Vector2 target)
        {
            SetSprite(texture);
            Sprite.Position = origin;
            Position = origin;

            _velocity = Vector2.Normalize(target - origin);
        }

        public override void Update(GameTime gameTime)
        {
            var timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Sprite.Rotation += 400.0f * timeDelta;

            Sprite.Position += _velocity * 500.0f * timeDelta;
            Position = Sprite.Position;
        }
    }
}
