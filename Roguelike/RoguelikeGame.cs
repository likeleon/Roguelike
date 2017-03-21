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

            // TODO: use this.Content to load your game content here
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

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
