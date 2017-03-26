using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.ViewportAdapters
{
    public abstract class ViewportAdapter
    {
        public GraphicsDevice GraphicsDevice { get; }

        public Viewport Viewport => GraphicsDevice.Viewport;
        public Rectangle ViewportRectangle => Viewport.Bounds;
        public Point ViewportSize => ViewportRectangle.Size;

        public abstract Point VirtualSize { get; }
        public Rectangle VirtualRectangle => new Rectangle(Point.Zero, VirtualSize);
        public Point VirtualCenter => VirtualRectangle.Center;

        protected ViewportAdapter(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public abstract Matrix GetScaleMatrix();

        public virtual Point PointToScreen(Point point)
        {
            var scaleMatrix = GetScaleMatrix();
            var invertedScaleMatrix = Matrix.Invert(scaleMatrix);
            return Vector2.Transform(point.ToVector2(), invertedScaleMatrix).ToPoint();
        }

        public virtual void Reset()
        {
        }
    }
}