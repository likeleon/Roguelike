using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Potion : Item
    {
        public int Attack { get; }
        public int Defense { get; }
        public int Strength { get; }
        public int Dexterity { get; }
        public int Stamina { get; }

        public Potion(ContentManager content)
            : base(ItemType.Potion)
        {
            SetSprite(content.Load<Texture2D>("Loot/Potions/spr_potion_stamina"), 8, 12);
        }
    }
}
