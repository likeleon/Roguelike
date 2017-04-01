using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Roguelike.Objects
{
    public abstract class Entity : Object
    {
        protected AnimationState CurrentAnimationState { get; set; } = AnimationState.WalkDown;

        private int _health;
        private int _mana;

        public int Health
        {
            get { return _health; }
            set { _health = MathHelper.Clamp(value, 0, MaxHealth); }
        }

        public int MaxHealth { get; set; }

        public int Mana
        {
            get { return _mana; }
            set { _mana = MathHelper.Clamp(value, 0, MaxMana); }
        }

        public int MaxMana { get; set; }

        public int Attack { get; set; }

        public int Defense { get; set; }

        public int Strength { get; set; }

        public int Dexterity { get; set; }

        public int Stamina { get; set; }

        public int Speed { get; set; }

        protected Vector2 Velocity { get; set; }
        protected Texture2D[] Textures { get; } = new Texture2D[EnumExtensions.GetCount<AnimationState>()];

        public override void Update(GameTime gameTime)
        {
            AnimationState animState = CurrentAnimationState;

            if (Velocity != Vector2.Zero)
            {
                if (Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
                    animState = (Velocity.X <= 0) ? AnimationState.WalkLeft : AnimationState.WalkRight;
                else
                    animState = (Velocity.Y <= 0) ? AnimationState.WalkUp : AnimationState.WalkDown;
            }

            if (Velocity == Vector2.Zero)
            {
                if (IsAnimated)
                {
                    CurrentAnimationState += 4;
                    IsAnimated = false;
                }
            }
            else
            {
                if (!IsAnimated)
                {
                    CurrentAnimationState -= 4;
                    IsAnimated = true;
                }
            }

            if (CurrentAnimationState != animState)
            {
                CurrentAnimationState = animState;
                Sprite.SetTexture(Textures[(int)CurrentAnimationState]);
            }
        }
    }
}
