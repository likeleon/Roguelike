using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using System;

namespace Roguelike
{
    public abstract class Object
    {
        private readonly Sprite _sprite;
        private readonly TimeSpan _frameDuration;
        private readonly Point _frameSize;

        private bool _isAnimated;
        private int _currentFrame;
        private TimeSpan _timeDelta;

        public int FrameCount { get; }

        public Object(Texture2D texture, int frames = 1, int frameSpeed = 0)
        {
            _sprite = new Sprite(texture);
            FrameCount = frames;

            var textureSize = texture.Bounds.Size;
            _frameSize = new Point(textureSize.X / FrameCount, textureSize.Y);

            IsAnimated = FrameCount > 1;
            if (IsAnimated)
                _frameDuration = TimeSpan.FromMilliseconds(1000.0f / frameSpeed);

            _sprite.Origin = _frameSize.ToVector2() / 2.0f;
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
            _sprite.Draw(spriteBatch);
        }

        private void NextFrame()
        {
            _currentFrame = (_currentFrame + 1) % FrameCount;
            _sprite.TextureRect = new Rectangle(new Point(_frameSize.X * _currentFrame, 0), _frameSize);
        }

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
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
                    _sprite.TextureRect = new Rectangle(Point.Zero, _frameSize);
            }
        }
    }
}
