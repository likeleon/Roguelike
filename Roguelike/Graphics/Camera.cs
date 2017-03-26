using Microsoft.Xna.Framework;

namespace Roguelike.Graphics
{
    public sealed class Camera
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1.0f;
        public float Rotation { get; set; }
        public Point ViewportSize { get; }
        public Vector2 ViewportCenter => ViewportSize.ToVector2() / 2.0f;

        public Camera(Point viewportSize)
        {
            ViewportSize = viewportSize;
        }

        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }

        public void Move(Vector2 offset) => Position += offset;

        public void Rotate(float angle) => Rotation += angle;

        public void AdjustZoom(float amount) => Zoom += amount;
    }
}
