using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.ViewportAdapters
{
    public class ScalingViewportAdapter : ViewportAdapter
    {
        public ScalingViewportAdapter(GraphicsDevice graphicsDevice, Point virtualSize)
            : base(graphicsDevice)
        {
            VirtualSize = virtualSize;
        }

        public override Point VirtualSize { get; }

        public override Matrix GetScaleMatrix()
        {
            var scale = ViewportSize.ToVector2() / VirtualSize.ToVector2();
            return Matrix.CreateScale(new Vector3(scale, 1.0f));
        }
    }
}
