using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Roguelike.Objects
{
    public sealed class Player : Entity
    {
        private static readonly IReadOnlyDictionary<AnimationState, string> AnimTextureAssets = new Dictionary<AnimationState, string>
        {
            [AnimationState.WalkUp] = "spr_warrior_walk_up",
            [AnimationState.WalkDown] = "spr_warrior_walk_down",
            [AnimationState.WalkRight] = "spr_warrior_walk_right",
            [AnimationState.WalkLeft] = "spr_warrior_walk_left",
            [AnimationState.IdleUp] = "spr_warrior_idle_up",
            [AnimationState.IdleDown] = "spr_warrior_idle_down",
            [AnimationState.IdleRight] = "spr_warrior_idle_right",
            [AnimationState.IdleLeft] = "spr_warrior_idle_left"
        };

        public Player(ContentManager content)
        {
            foreach (var kvp in AnimTextureAssets)
                Textures[(int)kvp.Key] = content.Load<Texture2D>($"Players/Warrior/{kvp.Value}");

            SetSprite(Textures[(int)AnimationState.WalkUp], frames: 8, frameSpeed: 12);
            CurrentAnimationState = AnimationState.WalkUp;
            Sprite.Origin = new Vector2(13.0f, 18.0f);
            Sprite.Scale = new Vector2(2.0f, 2.0f);

            Health = MaxHealth = 100;
            Mana = MaxMana = 50;
            Speed = 200;

            Attack = 10;
            Defense = 10;
            Strength = 10;
            Dexterity = 10;
            Stamina = 10;
        }
    }
}
