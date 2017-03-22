using Microsoft.Xna.Framework;

namespace Roguelike.Graphics
{
    public abstract class Transformable
    {
        private float _rotation;

        public Vector2 Position { get; set; }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value % 360;
                if (_rotation < 0)
                    _rotation += 360.0f;
            }
        }

        public Vector2 Scale { get; set; } = Vector2.One;

        public Vector2 Origin { get; set; }

        public void Move(Vector2 offset) => Position += offset;

        public void Rotate(float angle) => Rotation += angle;

        public void ScaleBy(Vector2 factor) => Scale *= factor;
    }
}
