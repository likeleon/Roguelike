using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Roguelike.Objects
{
    public sealed class Humanoid : Enemy
    {
        private static readonly IReadOnlyDictionary<AnimationState, string> AnimTextureAssets = new Dictionary<AnimationState, string>
        {
            [AnimationState.WalkUp] = "spr_skeleton_walk_up",
            [AnimationState.WalkDown] = "spr_skeleton_walk_down",
            [AnimationState.WalkRight] = "spr_skeleton_walk_right",
            [AnimationState.WalkLeft] = "spr_skeleton_walk_left",
            [AnimationState.IdleUp] = "spr_skeleton_idle_up",
            [AnimationState.IdleDown] = "spr_skeleton_idle_down",
            [AnimationState.IdleRight] = "spr_skeleton_idle_right",
            [AnimationState.IdleLeft] = "spr_skeleton_idle_left"
        };

        public Humanoid(ContentManager content)
        {
            foreach (var kvp in AnimTextureAssets)
                Textures[(int)kvp.Key] = content.Load<Texture2D>($"Enemies/Skeleton/{kvp.Value}");

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }
    }
}
