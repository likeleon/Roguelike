using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Gold : Item
    {
        public int GoldValue { get; } = 15;

        public Gold(ContentManager content)
            : base(ItemType.Gold)
        {
            SetSprite(content.Load<Texture2D>("Loot/Gold/spr_pickup_gold_medium"), frames: 8, frameSpeed: 12);
        }
    }
}
