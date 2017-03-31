using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roguelike.Graphics;
using Roguelike.Objects;
using Roguelike.ViewportAdapters;
using System;
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
        private Sprite _aimSprite;
        private Sprite _healthBarSprite;
        private Sprite _manaBarSprite;
        private Sprite _keyUiSprite;

        private int _scoreTotal;
        private int _goldTotal;

        private readonly List<Sprite> _uiSprites = new List<Sprite>();
        private readonly List<Sprite> _lightGrid = new List<Sprite>();
        private readonly List<Projectile> _playerProjectiles = new List<Projectile>();
        private readonly List<Item> _items = new List<Item>();
        private readonly List<Enemy> _enemies = new List<Enemy>();

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

            _aimSprite = new Sprite(Content.Load<Texture2D>("UI/spr_aim"));
            _aimSprite.Origin = new Vector2(16.5f, 16.5f);
            _aimSprite.Scale = new Vector2(2.0f);

            PopulateLevel();
        }

        private void LoadUI()
        {
            // Player
            _uiSprites.Add(new Sprite(Content.Load<Texture2D>("UI/spr_warrior_ui"))
            {
                Position = new Vector2(45.0f, 45.0f),
                Origin = new Vector2(30.0f, 30.0f)
            });

            // Bar outlines
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

            // Bars
            var healthBarTexture = Content.Load<Texture2D>("UI/spr_health_bar");
            var barTextureOrigin = healthBarTexture.Bounds.Size.ToVector2() / 2.0f;
            _healthBarSprite = new Sprite(healthBarTexture)
            {
                Position = new Vector2(205.0f, 35.0f),
                Origin = barTextureOrigin
            };
            _manaBarSprite = new Sprite(Content.Load<Texture2D>("UI/spr_mana_bar"))
            {
                Position = new Vector2(205.0f, 55.0f),
                Origin = barTextureOrigin
            };

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
            _keyUiSprite = new Sprite(Content.Load<Texture2D>("UI/spr_key_ui"))
            {
                Position = _virtualSize.ToVector2() - new Vector2(120.0f, 70.0f),
                Origin = new Vector2(90.0f, 45.0f),
                Color = new Color(255, 255, 255, 60)
            };
            _uiSprites.Add(_keyUiSprite);

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
        
        private void PopulateLevel()
        {
            SpawnItem(ItemType.Key, _virtualCenter.ToVector2() + new Vector2(50.0f, 0.0f));
            SpawnItem(ItemType.Gold, _virtualCenter.ToVector2() - new Vector2(50.0f, 0.0f));

            _enemies.Add(new Humanoid(Content)
            {
                Position = _player.Position - new Vector2(50.0f, 0.0f)
            });

            _enemies.Add(new Slime(Content)
            {
                Position = _player.Position + new Vector2(50.0f, 0.0f)
            });
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

            _aimSprite.Position = _viewportAdapter.PointToScreen(Mouse.GetState().Position).ToVector2();

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

            UpdateItems(playerPosition);

            UpdateLight(playerPosition);

            UpdateEnemies(gameTime);

            UpdateProjectiles(gameTime);

            _camera.Position = playerPosition;

            base.Update(gameTime);
        }

        private void UpdateItems(Vector2 playerPosition)
        {
            for (int i = _items.Count - 1; i >= 0; --i)
            {
                var item = _items[i];
                if (Vector2.Distance(item.Position, playerPosition) > 40.0f)
                    continue;

                switch (item.ItemType)
                {
                    case ItemType.Gem:
                        _scoreTotal += ((Gem)item).ScoreValue;
                        break;

                    case ItemType.Gold:
                        _goldTotal += ((Gold)item).GoldValue;
                        break;

                    case ItemType.Heart:
                        _player.Health += ((Heart)item).Health;
                        break;

                    case ItemType.Key:
                        _level.UnlockDoor();
                        _keyUiSprite.Color = Color.White;
                        break;
                }
                _items.RemoveAt(i);
            }
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

        private void UpdateEnemies(GameTime gameTime)
        {
            var playerTile = _level.GetTile(_player.Position);

            for (int i = _enemies.Count - 1; i >= 0; --i)
            {
                var enemy = _enemies[i];

                var enemyTile = _level.GetTile(enemy.Position);
                if (enemyTile == playerTile && _player.CanTakeDamage)
                    _player.TakeDamage(10);

                foreach (var projectile in _playerProjectiles)
                {
                    if (enemyTile != _level.GetTile(projectile.Position))
                        continue;

                    enemy.TakeDamage(25);

                    if (!enemy.IsDead)
                        continue;

                    var position = enemy.Position;
                    for (int j = 0; j < 5; ++j)
                    {
                        position.X += RandomGenerator.Next(-15, 16);
                        position.Y += RandomGenerator.Next(-15, 16);
                        var itemType = RandomGenerator.Next(0, 2) == 0 ? ItemType.Gold : ItemType.Gem;
                        SpawnItem(itemType, position);
                    }

                    if (RandomGenerator.Next(5) == 0)
                    {
                        position.X += RandomGenerator.Next(-15, 16);
                        position.Y += RandomGenerator.Next(-15, 16);
                        SpawnItem(ItemType.Heart, position);
                    }
                    else if (RandomGenerator.Next(5) == 1)
                    {
                        position.X += RandomGenerator.Next(-15, 16);
                        position.Y += RandomGenerator.Next(-15, 16);
                        SpawnItem(ItemType.Potion, position);
                    }

                    _enemies.RemoveAt(i);
                    break;
                }
            }
        }

        private void SpawnItem(ItemType itemType, Vector2 position)
        {
            Item item = null;
            switch (itemType)
            {
                case ItemType.Gem:
                    item = new Gem(Content);
                    break;

                case ItemType.Gold:
                    item = new Gold(Content);
                    break;

                case ItemType.Heart:
                    item = new Heart(Content);
                    break;

                case ItemType.Key:
                    item = new Key(Content, _font);
                    break;

                default:
                    throw new InvalidOperationException($"Unable to create an item with type '{itemType}'");
            }

            item.Position = position;
            _items.Add(item);
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

            foreach (var item in _items)
                item.Draw(_spriteBatch, gameTime);

            foreach (var enemy in _enemies)
                enemy.Draw(_spriteBatch, gameTime);

            foreach (var projectile in _playerProjectiles)
                projectile.Draw(_spriteBatch, gameTime);

            _player.Draw(_spriteBatch, gameTime);

            foreach (var sprite in _lightGrid)
                sprite.Draw(_spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());

            _aimSprite.Draw(_spriteBatch);

            DrawString(_player.Attack.ToString(), new Vector2(_virtualCenter.X - 210.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Defense.ToString(), new Vector2(_virtualCenter.X - 90.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Strength.ToString(), new Vector2(_virtualCenter.X + 30.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Dexterity.ToString(), new Vector2(_virtualCenter.X + 150.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Stamina.ToString(), new Vector2(_virtualCenter.X + 270.0f, _virtualSize.Y - 25.0f), 1.5f);

            DrawString(_scoreTotal.ToString().PadLeft(6, '0'), new Vector2(_virtualCenter.X - 120.0f, 50.0f), 2.0f);
            DrawString(_goldTotal.ToString().PadLeft(6, '0'), new Vector2(_virtualCenter.X + 220.0f, 50.0f), 2.0f);

            foreach (var sprite in _uiSprites)
                sprite.Draw(_spriteBatch);

            DrawString($"Floor {_level.FloorNumber}", new Vector2(70.0f, _virtualSize.Y - 60.0f), 1.5f);
            DrawString($"Room {_level.RoomNumber}", new Vector2(70.0f, _virtualSize.Y - 25.0f), 1.5f);

            _healthBarSprite.TextureRect = new Rectangle(0, 0, (int)(213.0f / _player.MaxHealth * _player.Health), 8);
            _healthBarSprite.Draw(_spriteBatch);

            _manaBarSprite.TextureRect = new Rectangle(0, 0, (int)(213.0f / _player.MaxMana * _player.Mana), 8);
            _manaBarSprite.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawString(string text, Vector2 position, float scale)
        {
            var textSize = _font.MeasureString(text);
            _spriteBatch.DrawString(_font, text, position, Color.White, 
                rotation: 0.0f, 
                origin: textSize / 2.0f, 
                scale: scale, 
                effects: SpriteEffects.None, 
                layerDepth: 0.0f);
        }
    }
}
