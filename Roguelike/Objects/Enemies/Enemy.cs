using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Objects
{
    public abstract class Enemy : Entity
    {
        private List<Vector2> _targetPositions = new List<Vector2>();

        public Enemy()
        {
            Health = Rand.Next(40, 121);
            Attack = Rand.Next(4, 11);
            Defense = Rand.Next(4, 11);
            Strength = Rand.Next(4, 11);
            Dexterity = Rand.Next(4, 11);
            Stamina = Rand.Next(4, 11);

            Speed = Rand.Next(50, 201);
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

            _targetPositions.AddRange(path.Select(tile => level.GetActualTileLocation(tile.Index)));
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
    }
}
