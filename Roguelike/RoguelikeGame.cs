using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roguelike.Graphics;
using Roguelike.Objects;
using Roguelike.ViewportAdapters;
using System.Collections.Generic;

namespace Roguelike
{
    public class RoguelikeGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly Point _virtualSize = new Point(1920, 1080);
        private readonly Point _virtualCenter;

        private Camera _camera;
        private ViewportAdapter _viewportAdapter;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _projectileTexture;

        private Level _level;
        private Player _player;
        private Sprite _animSprite;

        private readonly List<Sprite> _uiSprites = new List<Sprite>();
        private readonly List<Sprite> _lightGrid = new List<Sprite>();
        private readonly List<Projectile> _playerProjectiles = new List<Projectile>();

        public RoguelikeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = false;

            Window.AllowUserResizing = true;
            Window.Title = "Roguelike";

            _virtualCenter = new Point(_virtualSize.X / 2, _virtualSize.Y / 2);
        }

        protected override void Initialize()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _virtualSize);

            _camera = new Camera(_viewportAdapter);
            _camera.Zoom = 2.0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Fonts/ADDSBP__");
            _projectileTexture = Content.Load<Texture2D>("Projectiles/spr_sword");

            LoadUI();

            _level = new Level(Content, _virtualSize);
            _level.LoadFromFile("Content/Data/level_data.txt");

            ConstructLightGrid();

            _player = new Player(Content);
            _player.Position = _virtualCenter.ToVector2() + new Vector2(197.0f, 410.0f);

            _animSprite = new Sprite(Content.Load<Texture2D>("UI/spr_aim"));
            _animSprite.Origin = new Vector2(16.5f, 16.5f);
            _animSprite.Scale = new Vector2(2.0f);
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
                Position = new Vector2(_virtualCenter.X - 260.0f, 50.0f),
                Origin = new Vector2(42.0f, 36.0f)
            });
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_coin_ui"))
            {
                Position = new Vector2(_virtualCenter.X + 60.0f, 50.0f),
                Origin = new Vector2(48.0f, 24.0f)
            });

            // Key pickup
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_key_ui"))
            {
                Position = _virtualSize.ToVector2() - new Vector2(120.0f, 70.0f),
                Origin = new Vector2(90.0f, 45.0f),
                Color = new Color(255, 255, 255, 60)
            });

            // Stats
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_attack_ui"))
            {
                Origin = new Vector2(16.0f, 16.0f),
                Position = new Vector2(_virtualCenter.X - 270.0f, _virtualSize.Y - 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_defense_ui"))
            {
                Origin = new Vector2(16.0f, 12.0f),
                Position = new Vector2(_virtualCenter.X - 150.0f, _virtualSize.Y - 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_strength_ui"))
            {
                Origin = new Vector2(22.0f, 12.0f),
                Position = new Vector2(_virtualCenter.X - 30.0f, _virtualSize.Y - 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_dexterity_ui"))
            {
                Origin = new Vector2(16.0f, 12.0f),
                Position = new Vector2(_virtualCenter.X + 90.0f, _virtualSize.Y - 30.0f)
            });

            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_stamina_ui"))
            {
                Origin = new Vector2(16.0f, 16.0f),
                Position = new Vector2(_virtualCenter.X + 210.0f, _virtualSize.Y - 30.0f)
            });
        }

        private void ConstructLightGrid()
        {
            var lightTexture = Content.Load<Texture2D>("spr_light_grid");

            var levelAreaSize = new Point(_level.Size.X * _level.TileSize, _level.Size.Y * _level.TileSize);
            var levelArea = new Rectangle(_level.Origin, levelAreaSize);

            var width = levelArea.Width / 25;
            var height = levelArea.Height / 25;
            var lightTotal = width * height;

            for (int i = 0; i < lightTotal; ++i)
            {
                var lightSprite = new Sprite(lightTexture);
                var lightPositionX = levelArea.Left + ((i % width) * 25);
                var lightPositionY = levelArea.Top + ((i / width) * 25);
                lightSprite.Position = new Vector2(lightPositionX, lightPositionY);
                _lightGrid.Add(lightSprite);
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Order.IsOrderIssued(OrderType.Cancel))
                Exit();

            _player.Update(gameTime, _level, _camera);

            _animSprite.Position = _viewportAdapter.PointToScreen(Mouse.GetState().Position).ToVector2();

            var playerPosition = _player.Position;

            if (_player.IsAttacking)
            {
                _player.IsAttacking = false;
                if (_player.Mana >= 2)
                {
                    var target = _camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
                    var projectile = new Projectile(_projectileTexture, playerPosition, target);
                    _playerProjectiles.Add(projectile);

                    _player.Mana -= 2;
                }
            }

            UpdateLight(playerPosition);

            UpdateProjectiles(gameTime);

            _camera.Position = playerPosition;

            base.Update(gameTime);
        }

        private void UpdateLight(Vector2 playerPosition)
        {
            foreach (var sprite in _lightGrid)
            {
                var tileAlpha = 255.0f;

                var distance = Vector2.Distance(sprite.Position, playerPosition);
                if (distance < 200.0f)
                    tileAlpha = 0.0f;
                else if (distance < 250.0f)
                    tileAlpha = (51.0f * (distance - 200.0f)) / 10.0f;

                foreach (var torch in _level.Torches)
                {
                    distance = Vector2.Distance(sprite.Position, torch.Position);
                    if (distance < 100.0f)
                    {
                        var brightness = (tileAlpha - ((tileAlpha / 100.0f) * distance)) * torch.Brightness;
                        tileAlpha -= brightness;
                    }

                    if (tileAlpha < 0.0f)
                        tileAlpha = 0.0f;
                }

                sprite.Color = new Color(255, 255, 255, (int)tileAlpha);
            }
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            for (int i = _playerProjectiles.Count - 1; i >= 0; --i)
            {
                var projectileTileType = _level.GetTile(_playerProjectiles[i].Position).Type;
                if (projectileTileType != TileType.Floor && projectileTileType != TileType.FloorAlt)
                    _playerProjectiles.RemoveAt(i);
                else
                    _playerProjectiles[i].Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(3, 3, 3, 225));

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            _level.Draw(_spriteBatch, gameTime);

            foreach (var projectile in _playerProjectiles)
                projectile.Draw(_spriteBatch, gameTime);

            _player.Draw(_spriteBatch, gameTime);

            foreach (var sprite in _lightGrid)
                sprite.Draw(_spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());

            _animSprite.Draw(_spriteBatch);

            foreach (var sprite in _uiSprites)
                sprite.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
