using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Heart : Item
    {
        public int Health { get; } = 15;

        public Heart(ContentManager content) 
            : base(ItemType.Heart)
        {
            SetSprite(content.Load<Texture2D>("Loot/Heart/spr_pickup_heart"), frames:8, frameSpeed: 12);
        }
    }
}
