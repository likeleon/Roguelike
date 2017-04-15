using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using Roguelike.Order;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Objects
{
    public sealed class Player : Entity
    {
        private static readonly int BaseStatPoints = 50;
        private static readonly int PlayerTraitCount = 2;

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

        private static readonly TimeSpan AttackDelay = TimeSpan.FromMilliseconds(250);
        private static readonly TimeSpan TakeDamageDelay = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan ManaRegenDelay = TimeSpan.FromMilliseconds(200);

        private readonly OrderManager _orderManager;
        private readonly PlayerTrait[] _traits = new PlayerTrait[PlayerTraitCount];

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

        public PlayerClass Class { get; }
        public IEnumerable<PlayerTrait> Traits => _traits;

        public Player(ContentManager content, OrderManager orderManager)
        {
            _orderManager = orderManager;

            Class = (PlayerClass)Rand.Next(EnumExtensions.GetCount<PlayerClass>());

            var classStatValue = Rand.Next(0, 6);
            string className = string.Empty;
            switch (Class)
            {
                case PlayerClass.Warrior:
                    Strength = classStatValue;
                    className = "Warrior";
                    break;

                case PlayerClass.Mage:
                    Defense = classStatValue;
                    className = "Mage";
                    break;

                case PlayerClass.Archer:
                    Dexterity = classStatValue;
                    className = "Archer";
                    break;

                case PlayerClass.Thief:
                    Stamina = classStatValue;
                    className = "Thief";
                    break;

                default:
                    throw new InvalidOperationException($"Unknown player class '{Class}");
            }

            foreach (var kvp in AnimTextureAssets)
            {
                var assetName = kvp.Value.F(className.ToLower());
                Textures[(int)kvp.Key] = content.Load<Texture2D>($"Players/{className}/{assetName}");
            }

            SetSprite(Textures[(int)AnimationState.WalkUp], frames: 8, frameSpeed: 12);
            CurrentAnimationState = AnimationState.WalkUp;
            Sprite.Origin = new Vector2(13.0f, 18.0f);

            SetRandomTraits();

            Health = MaxHealth = 100;
            Mana = MaxMana = 50;
            Speed = 200;

            var attackBias = Rand.Next(101);
            var defenseBias = Rand.Next(101);
            var strengthBias = Rand.Next(101);
            var dexterityBias = Rand.Next(101);
            var staminaBias = Rand.Next(101);

            var total = attackBias + defenseBias + strengthBias + dexterityBias + staminaBias;

            Attack += BaseStatPoints * attackBias / total;
            Defense += BaseStatPoints * defenseBias / total;
            Strength += BaseStatPoints * strengthBias / total;
            Dexterity += BaseStatPoints * dexterityBias / total;
            Stamina += BaseStatPoints * staminaBias / total;
        }

        private void SetRandomTraits()
        {
            if (PlayerTraitCount > EnumExtensions.GetCount<PlayerTrait>())
            {
                var msg = $"{nameof(PlayerTraitCount)} should be greater or equal than number of {typeof(PlayerTrait)}";
                throw new InvalidOperationException(msg);
            }

            PlayerTraitCount.Times(i =>
            {
                PlayerTrait trait;
                do
                {
                    trait = (PlayerTrait)Rand.Next(EnumExtensions.GetCount<PlayerTrait>());
                } while (_traits.Contains(trait));

                var traitStatBuff = Rand.Next(5, 11);
                switch (trait)
                {
                    case PlayerTrait.Attack:
                        Attack += traitStatBuff;
                        break;

                    case PlayerTrait.Defense:
                        Defense += traitStatBuff;
                        break;

                    case PlayerTrait.Strength:
                        Strength += traitStatBuff;
                        break;

                    case PlayerTrait.Dexterity:
                        Dexterity += traitStatBuff;
                        break;

                    case PlayerTrait.Stamina:
                        Stamina += traitStatBuff;
                        break;
                }
                _traits[i] = trait;
            });
        }

        public void Update(GameTime gameTime, Level level, Camera camera)
        {
            var timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var movementSpeed = Vector2.Zero;
            var previousPosition = Position;

            var animState = CurrentAnimationState;

            if (_orderManager.IsOrderIssued(OrderType.MoveLeft))
            {
                movementSpeed.X = -Speed * timeDelta;
                animState = AnimationState.WalkLeft;
            }
            else if (_orderManager.IsOrderIssued(OrderType.MoveRight))
            {
                movementSpeed.X = Speed * timeDelta;
                animState = AnimationState.WalkRight;
            }

            if (_orderManager.IsOrderIssued(OrderType.MoveUp))
            {
                movementSpeed.Y = -Speed * timeDelta;
                animState = AnimationState.WalkUp;
            }
            else if (_orderManager.IsOrderIssued(OrderType.MoveDown))
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
            if (_attackDelta > AttackDelay && _orderManager.IsOrderIssued(OrderType.Attack))
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

            return overlappingTiles.Any(tile => level.IsSolid(tile.Index));
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            CanTakeDamage = false;
        }
    }
}
