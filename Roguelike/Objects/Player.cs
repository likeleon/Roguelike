using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private static readonly TimeSpan AttackDelay = TimeSpan.FromMilliseconds(250);
        private static readonly TimeSpan TakeDamageDelay = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan ManaRegenDelay = TimeSpan.FromMilliseconds(200);

        private TimeSpan _attackDelta;
        private bool _isAttacking;
        private TimeSpan _takeDamageDelta;
        private bool _canTakeDamage = true;
        private TimeSpan _manaRegenDelta;

        public bool IsAttacking
        {
            get { return _isAttacking; }
            set { _isAttacking = value; _attackDelta = default(TimeSpan); }
        }

        public bool CanTakeDamage
        {
            get { return _canTakeDamage; }
            set { _canTakeDamage = value; _takeDamageDelta = default(TimeSpan); }
        }

        public Player(ContentManager content)
        {
            foreach (var kvp in AnimTextureAssets)
                Textures[(int)kvp.Key] = content.Load<Texture2D>($"Players/Warrior/{kvp.Value}");

            SetSprite(Textures[(int)AnimationState.WalkUp], frames: 8, frameSpeed: 12);
            CurrentAnimationState = AnimationState.WalkUp;
            Sprite.Origin = new Vector2(13.0f, 18.0f);

            Health = MaxHealth = 100;
            Mana = MaxMana = 50;
            Speed = 200;

            Attack = 10;
            Defense = 10;
            Strength = 10;
            Dexterity = 10;
            Stamina = 10;
        }

        public void Update(GameTime gameTime, Level level, Camera camera)
        {
            var timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var movementSpeed = Vector2.Zero;
            var previousPosition = Position;

            var animState = CurrentAnimationState;

            if (Order.IsOrderIssued(OrderType.MoveLeft))
            {
                movementSpeed.X = -Speed * timeDelta;
                animState = AnimationState.WalkLeft;
            }
            else if (Order.IsOrderIssued(OrderType.MoveRight))
            {
                movementSpeed.X = Speed * timeDelta;
                animState = AnimationState.WalkRight;
            }

            if (Order.IsOrderIssued(OrderType.MoveUp))
            {
                movementSpeed.Y = -Speed * timeDelta;
                animState = AnimationState.WalkUp;
            }
            else if (Order.IsOrderIssued(OrderType.MoveDown))
            {
                movementSpeed.Y = Speed * timeDelta;
                animState = AnimationState.WalkDown;
            }

            if (movementSpeed.X != 0.0f && !CausesCollision(new Vector2(movementSpeed.X, 0.0f), level))
                Position += new Vector2(movementSpeed.X, 0.0f);

            if (movementSpeed.Y != 0.0f && !CausesCollision(new Vector2(0.0f, movementSpeed.Y), level))
                Position += new Vector2(0.0f, movementSpeed.Y);

            if (CurrentAnimationState != animState)
            {
                CurrentAnimationState = animState;
                Sprite.SetTexture(Textures[(int)CurrentAnimationState]);
            }

            if (Velocity == Vector2.Zero)
            {
                if (IsAnimated)
                {
                    CurrentAnimationState += 4;
                    Sprite.SetTexture(Textures[(int)CurrentAnimationState]);
                    IsAnimated = false;
                }
            }
            else
            {
                if (!IsAnimated)
                {
                    CurrentAnimationState -= 4;
                    Sprite.SetTexture(Textures[(int)CurrentAnimationState]);
                    IsAnimated = true;
                }
            }

            _attackDelta += gameTime.ElapsedGameTime;
            if (_attackDelta > AttackDelay && Order.IsOrderIssued(OrderType.Attack))
                IsAttacking = true;

            if (!CanTakeDamage)
            {
                _takeDamageDelta += gameTime.ElapsedGameTime;
                if (_takeDamageDelta > TakeDamageDelay)
                    CanTakeDamage = true;
            }

            _manaRegenDelta += gameTime.ElapsedGameTime;
            if (_manaRegenDelta > ManaRegenDelay)
            {
                Mana += 1;
                _manaRegenDelta = default(TimeSpan);
            }
        }

        private bool CausesCollision(Vector2 movement, Level level)
        {
            var newPosition = Position + movement;

            var overlappingTiles = new Tile[]
            {
                level.GetTile(newPosition + new Vector2(-14.0f, -14.0f)),
                level.GetTile(newPosition + new Vector2(+14.0f, -14.0f)),
                level.GetTile(newPosition + new Vector2(-14.0f, +14.0f)),
                level.GetTile(newPosition + new Vector2(+14.0f, +14.0f))
            };

            return overlappingTiles.Any(tile => level.IsSolid(tile.ColumnIndex, tile.RowIndex));
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            CanTakeDamage = false;
        }
    }
}
