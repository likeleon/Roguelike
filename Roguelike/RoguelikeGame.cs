using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roguelike.Graphics;
using System.Collections.Generic;

namespace Roguelike
{
    public class RoguelikeGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private ScalingViewportAdapter _scalingViewportAdapter;
        private Vector2 _screenCenter;
        private Point _screenSize;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private readonly List<Sprite> _uiSprites = new List<Sprite>();

        public RoguelikeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = false;
            Window.Title = "Roguelike";
        }

        protected override void Initialize()
        {
            _scalingViewportAdapter = new ScalingViewportAdapter(GraphicsDevice, 1920, 1080);
            _screenCenter = _scalingViewportAdapter.BoundingRectangle.Center.ToVector2();
            _screenSize = _scalingViewportAdapter.BoundingRectangle.Size;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Fonts/ADDSBP__");

            LoadUI();
        }

        private void LoadUI()
        {
            // Player
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_warrior_ui"))
            {
                Position = new Vector2(45.0f, 45.0f),
                Origin = new Vector2(30.0f, 30.0f)
            });

            // Bars
            var barOutlineTexture = Content.Load<Texture2D>("UI/spr_bar_outline");
            var barOutlineTextureOrigin = barOutlineTexture.Bounds.Size.ToVector2() / 2.0f;
            _uiSprites.Add(new Sprite(barOutlineTexture)
            {
                Position = new Vector2(205.0f, 35.0f),
                Origin = barOutlineTextureOrigin
            });
            _uiSprites.Add(new Sprite(barOutlineTexture)
            {
                Position = new Vector2(205.0f, 55.0f),
                Origin = barOutlineTextureOrigin
            });

            // Coin and Gem
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_gem_ui"))
            {
                Position = new Vector2(_screenCenter.X - 260.0f, 50.0f),
                Origin = new Vector2(42.0f, 36.0f)
            });
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_coin_ui"))
            {
                Position = new Vector2(_screenCenter.X + 60.0f, 50.0f),
                Origin = new Vector2(48.0f, 24.0f)
            });

            // Key pickup
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_key_ui"))
            {
                Position = _screenSize.ToVector2() - new Vector2(120.0f, 70.0f),
                Origin = new Vector2(90.0f, 45.0f),
                Color = new Color(255, 255, 255, 60)
            });

            // Stats
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_attack_ui"))
            {
                Origin = new Vector2(16.0f, 16.0f),
                Position = _screenCenter - new Vector2(270.0f, 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_defense_ui"))
            {
                Origin = new Vector2(16.0f, 12.0f),
                Position = _screenCenter - new Vector2(150.0f, 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_strength_ui"))
            {
                Origin = new Vector2(22.0f, 12.0f),
                Position = _screenCenter - new Vector2(30.0f, 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_dexterity_ui"))
            {
                Origin = new Vector2(16.0f, 12.0f),
                Position = _screenCenter + new Vector2(90.0f, -30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_stamina_ui"))
            {
                Origin = new Vector2(16.0f, 16.0f),
                Position = _screenCenter + new Vector2(210.0f, -30.0f)
            });
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _scalingViewportAdapter.GetScaleMatrix());

            var text = "MonoGame Font Test";
            var textMiddlePoint = _font.MeasureString(text) / 2;
            _spriteBatch.DrawString(_font, text, _screenCenter, Color.White, 0, textMiddlePoint, 1.0f, SpriteEffects.None, 0.5f);

            foreach (var sprite in _uiSprites)
                sprite.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
