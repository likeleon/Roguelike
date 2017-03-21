using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roguelike
{
    public class RoguelikeGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private Vector2 _viewportCenter;
        private Point _viewportSize;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

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

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
