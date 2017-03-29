using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Roguelike.Objects
{
    public sealed class Slime : Enemy
    {
        private static readonly IReadOnlyDictionary<AnimationState, string> AnimTextureAssets = new Dictionary<AnimationState, string>
        {
            [AnimationState.WalkUp] = "spr_slime_walk_up",
            [AnimationState.WalkDown] = "spr_slime_walk_down",
            [AnimationState.WalkRight] = "spr_slime_walk_right",
            [AnimationState.WalkLeft] = "spr_slime_walk_left",
            [AnimationState.IdleUp] = "spr_slime_idle_up",
            [AnimationState.IdleDown] = "spr_slime_idle_down",
            [AnimationState.IdleRight] = "spr_slime_idle_right",
            [AnimationState.IdleLeft] = "spr_slime_idle_left"
        };

        public Slime(ContentManager content)
        {
            foreach (var kvp in AnimTextureAssets)
                Textures[(int)kvp.Key] = content.Load<Texture2D>($"Enemies/Slime/{kvp.Value}");

            SetSprite(Textures[(int)(AnimationState.WalkUp)], frames: 8, frameSpeed: 12);
        }
    }
}
