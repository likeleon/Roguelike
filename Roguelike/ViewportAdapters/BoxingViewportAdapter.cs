using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Roguelike.ViewportAdapters
{
    public enum BoxingMode
    {
        Letterbox,
        Pillarbox
    }

    public class BoxingViewportAdapter : ScalingViewportAdapter
    {
        private readonly GameWindow _window;

        public BoxingMode BoxingMode { get; private set; }

        public BoxingViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice, Point virtualSize)
            : base(graphicsDevice, virtualSize)
        {
            _window = window;
            _window.ClientSizeChanged += OnClientSizeChanged;
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            var aspectRatio = (float)VirtualSize.X / VirtualSize.Y;
            var width = ViewportSize.X;
            var height = (int)(width / aspectRatio + 0.5f);

            if (height > ViewportSize.Y)
            {
                BoxingMode = BoxingMode.Pillarbox;
                width = (int)(ViewportSize.Y * aspectRatio + 0.5f);
                height = ViewportSize.Y;
            }
            else
            {
                BoxingMode = BoxingMode.Letterbox;
            }

            var x = (ViewportSize.X - width) / 2;
            var y = (ViewportSize.Y - height) / 2;
            GraphicsDevice.Viewport = new Viewport(x, y, width, height);
        }

        public override void Reset()
        {
            base.Reset();
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        public override Point PointToScreen(Point point)
        {
            return base.PointToScreen(point - Viewport.Bounds.Location);
        }
    }
}
