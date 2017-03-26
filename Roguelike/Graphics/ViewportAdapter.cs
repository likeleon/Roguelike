using Microsoft.Xna.Framework;

namespace Roguelike.Graphics
{
    public abstract class ViewportAdapter
    {
        public Point ScreenSize { get; }
        public Point VirtualSize { get; }
        public Rectangle VirtualRectangle => new Rectangle(Point.Zero, VirtualSize);

        protected ViewportAdapter(Point screenSize, Point virtualSize)
        {
            ScreenSize = screenSize;
            VirtualSize = virtualSize;
        }

        public abstract Matrix GetScaleMatrix();

        public virtual Point PointToScreen(Point point)
        {
            var invertedScaleMatrix = Matrix.Invert(GetScaleMatrix());
            return Vector2.Transform(point.ToVector2(), invertedScaleMatrix).ToPoint();
        }
    }
}