using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Objects
{
    public abstract class Enemy : Entity
    {
        private readonly List<Vector2> _targetPositions = new List<Vector2>();
        private readonly Sprite _pathSprite;

        public Enemy()
        {
            Health = Rand.Next(40, 121);
            Attack = Rand.Next(4, 11);
            Defense = Rand.Next(4, 11);
            Strength = Rand.Next(4, 11);
            Dexterity = Rand.Next(4, 11);
            Stamina = Rand.Next(4, 11);

            Speed = Rand.Next(50, 201);

            _pathSprite = new Sprite(Global.Content.Load<Texture2D>("spr_path"));
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public bool IsDead => Health <= 0;

        public void UpdatePathFinding(Level level, Vector2 playerPosition)
        {
            _targetPositions.Clear();

            var start = level.GetTile(Position);
            var goal = level.GetTile(playerPosition);
            var path = level.GetShortestPath(start, goal);

            if (path.Length <= 1)
                return;

            var targetPositions = path.Skip(1).Select(tile => level.GetActualTileLocation(tile.Index));
            _targetPositions.AddRange(targetPositions);
        }

        public override void Update(GameTime gameTime)
        {
            if (_targetPositions.Count > 0)
            {
                var targetPosition = _targetPositions[0];
                var distance = targetPosition - Position;
                if (Math.Abs(distance.X) < 10.0f && Math.Abs(distance.Y) < 10.0f)
                {
                    _targetPositions.RemoveAt(0);
                }
                else
                {
                    Velocity = Vector2.Normalize(distance);
                    Position += Velocity * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (Global.PathDebugIsEnabled)
            {
                for (int i = 0; i < _targetPositions.Count; ++i)
                {
                    var targetPosition = _targetPositions[i];
                    _pathSprite.Position = targetPosition;
                    _pathSprite.Draw(spriteBatch);

                    var text = $"{i}";
                    var textSize = Global.Font.MeasureString(text);
                    spriteBatch.DrawString(Global.Font, $"{i}", targetPosition, Color.White,
                        rotation: 0.0f,
                        origin: textSize / 2.0f,
                        scale: 1.0f,
                        effects: SpriteEffects.None,
                        layerDepth: 0.0f);
                }
            }
        }
    }
}
