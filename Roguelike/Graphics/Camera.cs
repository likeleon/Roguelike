using Microsoft.Xna.Framework;
using Roguelike.ViewportAdapters;

namespace Roguelike.Graphics
{
    public sealed class Camera
    {
        private readonly ViewportAdapter _viewportAdapter;

        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1.0f;
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }

        public Camera(ViewportAdapter viewportAdapter)
        {
            _viewportAdapter = viewportAdapter;

            Origin = viewportAdapter.VirtualSize.ToVector2() / 2.0f;
        }

        public Matrix GetViewMatrix()
        {
            return GetVirutalViewMatrix() * _viewportAdapter.GetScaleMatrix();
        }

        private Matrix GetVirutalViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            var viewport = _viewportAdapter.Viewport;
            var position = worldPosition + viewport.Bounds.Location.ToVector2();
            return Vector2.Transform(position, GetViewMatrix());
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            var viewport = _viewportAdapter.Viewport;
            var position = screenPosition - viewport.Bounds.Location.ToVector2();
            return Vector2.Transform(position, GetInverseViewMatrix());
        }
    }
}
