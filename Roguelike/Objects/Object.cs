using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using System;

namespace Roguelike.Objects
{
    public abstract class Object
    {
        private TimeSpan _frameDuration;
        private Point _frameSize;
        private bool _isAnimated;
        private int _currentFrame;
        private TimeSpan _timeDelta;

        public int FrameCount { get; private set; }
        protected Sprite Sprite { get; } = new Sprite();

        public void SetSprite(Texture2D texture, int frames = 1, int frameSpeed = 0)
        {
            Sprite.SetTexture(texture);
            FrameCount = frames;

            var textureSize = texture.Bounds.Size;
            _frameSize = new Point(textureSize.X / FrameCount, textureSize.Y);

            IsAnimated = FrameCount > 1;
            if (IsAnimated)
                _frameDuration = TimeSpan.FromMilliseconds(1000.0f / frameSpeed);

            Sprite.Origin = _frameSize.ToVector2() / 2.0f;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsAnimated)
            {
                _timeDelta += gameTime.ElapsedGameTime;

                if (_timeDelta >= _frameDuration)
                {
                    NextFrame();
                    _timeDelta = TimeSpan.Zero;
                }
            }
            Sprite.Draw(spriteBatch);
        }

        private void NextFrame()
        {
            _currentFrame = (_currentFrame + 1) % FrameCount;
            Sprite.TextureRect = new Rectangle(new Point(_frameSize.X * _currentFrame, 0), _frameSize);
        }

        public Vector2 Position
        {
            get { return Sprite.Position; }
            set { Sprite.Position = value; }
        }

        public bool IsAnimated
        {
            get { return _isAnimated; }
            set
            {
                _isAnimated = value;
                if (_isAnimated)
                    _currentFrame = 0;
                else
                    Sprite.TextureRect = new Rectangle(Point.Zero, _frameSize);
            }
        }
    }
}
