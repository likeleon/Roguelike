namespace Roguelike.Objects
{
    public sealed class Slime : Enemy
    {
        public Slime()
        {
            foreach (var kvp in EnemyTextureFactory.GetBaseTextures("Slime"))
                Textures[(int)kvp.Key] = kvp.Value;

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }
    }
}
