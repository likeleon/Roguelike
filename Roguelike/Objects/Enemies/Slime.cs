using Microsoft.Xna.Framework;

namespace Roguelike.Objects
{
    public sealed class Slime : Enemy
    {
        public Slime()
        {
            foreach (var kvp in EnemyTextureFactory.GetBaseTextures("Slime"))
                Textures[(int)kvp.Key] = kvp.Value;

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);

            Sprite.Color = GetRandomColor();

            var scale = Rand.Next(5, 15) / 10.0f;
            Sprite.Scale = new Vector2(scale);
        }

        private static Color GetRandomColor()
        {
            return Color.FromNonPremultiplied(
                r: Rand.Next(256),
                g: Rand.Next(256),
                b: Rand.Next(256),
                a: Rand.Next(100, 256));
        }
    }
}
