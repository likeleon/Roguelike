using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Gem : Item
    {
        public int ScoreValue { get; } = 50;

        public Gem(ContentManager content)
            : base(ItemType.Gem)
        {
            SetSprite(content.Load<Texture2D>("Loot/Gem/spr_pickup_gem"), frames: 8, frameSpeed: 12);
        }
    }
}
