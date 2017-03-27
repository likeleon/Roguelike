using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Torch : Object
    {
        public float Brightness { get; set; } = 1.0f;

        public Torch(ContentManager content)
        {
            SetSprite(content.Load<Texture2D>("spr_torch"), frames: 5, frameSpeed: 12);
        }

        public override void Update(GameTime gameTime)
        {
            Brightness = RandomGenerator.Next(80, 120) / 100.0f;
        }
    }
}
