using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Utils;
using System.Collections.Generic;

namespace Roguelike.Objects
{
    public sealed class Humanoid : Enemy
    {
        private static readonly IReadOnlyDictionary<AnimationState, string> AnimTextureAssets = new Dictionary<AnimationState, string>
        {
            [AnimationState.WalkUp] = "spr_{0}_walk_up",
            [AnimationState.WalkDown] = "spr_{0}_walk_down",
            [AnimationState.WalkRight] = "spr_{0}_walk_right",
            [AnimationState.WalkLeft] = "spr_{0}_walk_left",
            [AnimationState.IdleUp] = "spr_{0}_idle_up",
            [AnimationState.IdleDown] = "spr_{0}_idle_down",
            [AnimationState.IdleRight] = "spr_{0}_idle_right",
            [AnimationState.IdleLeft] = "spr_{0}_idle_left"
        };

        public HumanoidType HumanoidType { get; }

        public Humanoid(ContentManager content)
        {
            HumanoidType = (HumanoidType)Rand.Next(EnumExtensions.GetCount<HumanoidType>());

            foreach (var kvp in AnimTextureAssets)
            {
                var humanoidName = HumanoidType.ToString();
                var textureName = "Enemies/{0}/{1}".F(humanoidName, kvp.Value.F(humanoidName.ToLower()));
                Textures[(int)kvp.Key] = content.Load<Texture2D>(textureName);
            }

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }
    }

    public enum HumanoidType
    {
        Goblin,
        Skeleton
    }
}
