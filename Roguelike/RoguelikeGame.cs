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
        private Vector2 _viewportCenter;
        private Point _viewportSize;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private readonly List<Sprite> _uiSprites = new List<Sprite>();

        public RoguelikeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = false;
            Window.Title = "Roguelike";
        }

        protected override void Initialize()
        {
            _viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2();
            _viewportSize = GraphicsDevice.Viewport.Bounds.Size;

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
            var playerUiSprite = new Sprite(Content.Load<Texture2D>("UI/spr_warrior_ui"))
            {
                Position = new Vector2(45.0f, 45.0f),
                Origin = new Vector2(30.0f, 30.0f)
            };
            _uiSprites.Add(playerUiSprite);
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

            _spriteBatch.Begin();

            var text = "MonoGame Font Test";
            var textMiddlePoint = _font.MeasureString(text) / 2;
            _spriteBatch.DrawString(_font, text, _viewportCenter, Color.White, 0, textMiddlePoint, 1.0f, SpriteEffects.None, 0.5f);

            foreach (var sprite in _uiSprites)
                sprite.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
