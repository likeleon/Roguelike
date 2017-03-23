using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Graphics
{
    public abstract class ViewportAdapter
    {
        protected ViewportAdapter(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public GraphicsDevice GraphicsDevice { get; }
        public Viewport Viewport => GraphicsDevice.Viewport;

        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }

        public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);
        public Point Center => BoundingRectangle.Center;

        public abstract Matrix GetScaleMatrix();

        public virtual Point PointToScreen(Point point)
        {
            var invertedScaleMatrix = Matrix.Invert(GetScaleMatrix());
            return Vector2.Transform(point.ToVector2(), invertedScaleMatrix).ToPoint();
        }

        public virtual void Reset()
        {
        }
    }
}