using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Key : Item
    {
        public Key(ContentManager content)
            : base(ItemType.Key)
        {
            SetSprite(content.Load<Texture2D>("Loot/Key/spr_pickup_key"), frames: 8, frameSpeed: 12);
            SetName("Key");
        }
    }
}
