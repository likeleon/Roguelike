using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Objects
{
    public sealed class Potion : Item
    {
        public PotionType PotionType { get; }

        public int Attack { get; }
        public int Defense { get; }
        public int Strength { get; }
        public int Dexterity { get; }
        public int Stamina { get; }

        public Potion(ContentManager content)
            : base(ItemType.Potion)
        {
            PotionType = (PotionType)Rand.Next(EnumExtensions.GetEnumLength<ItemType>());

            var statValue = Rand.Next(5, 16);
            switch (PotionType)
            {
                case PotionType.Attack:
                    Attack = statValue;
                    break;

                case PotionType.Defense:
                    Defense = statValue;
                    break;

                case PotionType.Strength:
                    Strength = statValue;
                    break;

                case PotionType.Dexterity:
                    Dexterity = statValue;
                    break;

                case PotionType.Stamina:
                    Stamina = statValue;
                    break;
            }

            var potionTypeName = PotionType.ToString().ToLower();
            var texture = content.Load<Texture2D>($"Loot/Potions/spr_potion_{potionTypeName}");
            SetSprite(texture, frames: 8, frameSpeed: 12);
        }
    }

    public enum PotionType
    {
        Attack,
        Defense,
        Strength,
        Dexterity,
        Stamina
    }
}
