namespace Roguelike.Objects
{
    public sealed class Humanoid : Enemy
    {
        public HumanoidType HumanoidType { get; } = (HumanoidType)Rand.Next(EnumExtensions.GetCount<HumanoidType>());

        public Humanoid()
        {
            foreach (var kvp in EnemyTextureFactory.GetBaseAnimations(HumanoidType.ToString()))
                Textures[(int)kvp.Key] = kvp.Value;

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }
    }

    public enum HumanoidType
    {
        Goblin,
        Skeleton
    }
}
