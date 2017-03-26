using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.ViewportAdapters
{
    public class DefaultViewportAdapter : ViewportAdapter
    {
        public DefaultViewportAdapter(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public override Point VirtualSize => GraphicsDevice.Viewport.Bounds.Size;

        public override Matrix GetScaleMatrix() => Matrix.Identity;
    }
}
