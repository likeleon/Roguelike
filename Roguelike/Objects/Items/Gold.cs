using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Gold : Item
    {
        public int GoldValue { get; } = Rand.Next(5, 26);

        public Gold(ContentManager content)
            : base(ItemType.Gold)
        {
            var valueSize = (GoldValue < 9) ? "small" : (GoldValue >= 16) ? "large" : "medium";
            var texture = content.Load<Texture2D>($"Loot/Gold/spr_pickup_gold_{valueSize}");
            SetSprite(texture, frames: 8, frameSpeed: 12);
        }
    }
}
