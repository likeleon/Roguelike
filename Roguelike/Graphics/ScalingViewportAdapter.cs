using Microsoft.Xna.Framework;

namespace Roguelike.Graphics
{
    public class ScalingViewportAdapter : ViewportAdapter
    {
        public ScalingViewportAdapter(Point screenSize, Point virtualSize)
            : base(screenSize, virtualSize)
        {
        }

        public override Matrix GetScaleMatrix()
        {
            var scaleX = (float)ScreenSize.X / VirtualSize.X;
            var scaleY = (float)ScreenSize.Y / VirtualSize.Y;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}
