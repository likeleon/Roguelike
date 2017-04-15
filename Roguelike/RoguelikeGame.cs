using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Roguelike.Graphics;
using Roguelike.Objects;
using Roguelike.Sounds;
using Roguelike.Utils;
using Roguelike.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace Roguelike
{
    public class RoguelikeGame : Game
    {
        private static readonly int MaxItemSpawnCount = 50;
        private static readonly int MaxEnemySpawnCount = 20;
        private static readonly int EnemyDamage = 10;

        private static readonly IReadOnlyDictionary<PlayerClass, string> ProjectileNamesByClass = new Dictionary<PlayerClass, string>()
        {
            [PlayerClass.Archer] = "arrow",
            [PlayerClass.Mage] = "magic_ball",
            [PlayerClass.Thief] = "dagger",
            [PlayerClass.Warrior] = "sword",
        };

        private readonly GraphicsDeviceManager _graphics;
        private readonly Point _virtualSize = new Point(1920, 1080);
        private readonly Point _virtualCenter;
        private readonly Texture2D[] _playerUiTextures = new Texture2D[EnumExtensions.GetCount<PlayerClass>()];
        private readonly Texture2D[] _attackStatTextures = new Texture2D[2];
        private readonly Texture2D[] _defenseStatTextures = new Texture2D[2];
        private readonly Texture2D[] _strengthStatTextures = new Texture2D[2];
        private readonly Texture2D[] _dexterityStatTextures = new Texture2D[2];
        private readonly Texture2D[] _staminaStatTextures = new Texture2D[2];
        private readonly List<Sprite> _uiSprites = new List<Sprite>();
        private readonly List<Sprite> _lightGrid = new List<Sprite>();
        private readonly List<Projectile> _playerProjectiles = new List<Projectile>();
        private readonly List<Item> _items = new List<Item>();
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private readonly AudioListener _audioListener = new AudioListener();
        private readonly AudioEmitter _audioEmitter = new AudioEmitter();

        private Camera _camera;
        private ViewportAdapter _viewportAdapter;

        private Texture2D _projectileTexture;
        private TileTextures _tileTextures;

        private Level _level;
        private Player _player;
        private Sprite _aimSprite;
        private Sprite _healthBarSprite;
        private Sprite _manaBarSprite;
        private Sprite _keyUiSprite;
        private Sprite _attackStatSprite;
        private Sprite _defenseStatSprite;
        private Sprite _strengthStatSprite;
        private Sprite _dexterityStatSprite;
        private Sprite _staminaStatSprite;

        private Quest _quest;
        private int _scoreTotal;
        private int _goldTotal;

        private Song _music;
        private AmbientSound _fireAmbientSound;
        private SoundEffect _enemyDieSound;
        private SoundEffect _gemPickupSound;
        private SoundEffect _coinPickupSound;
        private SoundEffect _keyPickupSound;
        private SoundEffect _playerHitSound;

        private Tile _playerPreviousTile;

        public RoguelikeGame()
        {
            Content.RootDirectory = "Content";

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 800,
                SynchronizeWithVerticalRetrace = true
            };

            IsMouseVisible = false;

            Window.AllowUserResizing = true;
            Window.Title = "Roguelike";

            _virtualCenter = new Point(_virtualSize.X / 2, _virtualSize.Y / 2);
        }

        protected override void Initialize()
        {
            Global.Content = Content;
            Global.GraphicsDevice = GraphicsDevice;
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Global.DebugPathFinding = true;
            Global.DebugLevelGeneration = true;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _virtualSize);

            _camera = new Camera(_viewportAdapter)
            {
                Zoom = 2.0f
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Global.Font = Content.Load<SpriteFont>("Fonts/ADDSBP__");

            _player = new Player(Content);

            _tileTextures = new TileTextures();
            GenerateLevel();

            var projectileName = ProjectileNamesByClass[_player.Class];
            _projectileTexture = Content.Load<Texture2D>($"Projectiles/spr_{projectileName}");

            if (!Global.DebugLevelGeneration)
                ConstructLightGrid();

            LoadUI();

            _aimSprite = new Sprite(Content.Load<Texture2D>("UI/spr_aim"))
            {
                Origin = new Vector2(16.5f, 16.5f),
                Scale = new Vector2(2.0f)
            };

            SoundEffect.DistanceScale = 80.0f;

            _audioListener.Forward = Vector3.UnitY;
            _audioListener.Up = Vector3.UnitZ;

            var trackIndex = Rand.Next(1, EnumExtensions.GetCount<MusicTrack>() + 1);
            _music = Content.Load<Song>($"Music/msc_main_track_{trackIndex}");
            MediaPlayer.Play(_music);

            _fireAmbientSound = new AmbientSound(Content.Load<SoundEffect>("Sounds/snd_fire"), _audioListener);
            _enemyDieSound = Content.Load<SoundEffect>("Sounds/snd_enemy_dead");
            _gemPickupSound = Content.Load<SoundEffect>("Sounds/snd_gem_pickup");
            _coinPickupSound = Content.Load<SoundEffect>("Sounds/snd_coin_pickup");
            _keyPickupSound = Content.Load<SoundEffect>("Sounds/snd_key_pickup");
            _playerHitSound = Content.Load<SoundEffect>("Sounds/snd_player_hit");
        }

        private void GenerateLevel()
        {
            _level = Level.Generate(_tileTextures, _virtualSize);

            if (!Global.DebugLevelGeneration)
            {
                SpawnItem(ItemType.Key);

                PopulateLevel();
            }

            _quest = Quest.CreateRandom();

            _player.Position = _level.PlayerSpawnLocation;
        }

        private void LoadUI()
        {
            _playerUiTextures[(int)PlayerClass.Warrior] = Content.Load<Texture2D>("UI/spr_warrior_ui");
            _playerUiTextures[(int)PlayerClass.Mage] = Content.Load<Texture2D>("UI/spr_mage_ui");
            _playerUiTextures[(int)PlayerClass.Archer] = Content.Load<Texture2D>("UI/spr_archer_ui");
            _playerUiTextures[(int)PlayerClass.Thief] = Content.Load<Texture2D>("UI/spr_thief_ui");

            // Player
            _uiSprites.Add(new Sprite(_playerUiTextures[(int)_player.Class])
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
                Color = new Color(255, 255, 255) * 0.235f
            };
            _uiSprites.Add(_keyUiSprite);

            // Stats
            _attackStatSprite = LoadStatUI("attack", _attackStatTextures, new Vector2(_virtualCenter.X - 270.0f, _virtualSize.Y - 30.0f));
            _defenseStatSprite = LoadStatUI("defense", _defenseStatTextures, new Vector2(_virtualCenter.X - 150.0f, _virtualSize.Y - 30.0f));
            _strengthStatSprite = LoadStatUI("strength", _strengthStatTextures, new Vector2(_virtualCenter.X - 30.0f, _virtualSize.Y - 30.0f));
            _dexterityStatSprite = LoadStatUI("dexterity", _dexterityStatTextures, new Vector2(_virtualCenter.X + 90.0f, _virtualSize.Y - 30.0f));
            _staminaStatSprite = LoadStatUI("stamina", _staminaStatTextures, new Vector2(_virtualCenter.X + 210.0f, _virtualSize.Y - 30.0f));

            foreach (var playerTrait in _player.Traits)
            {
                switch (playerTrait)
                {
                    case PlayerTrait.Attack:
                        _attackStatSprite.SetTexture(_attackStatTextures[1]);
                        _attackStatSprite.Scale = new Vector2(1.2f);
                        break;

                    case PlayerTrait.Defense:
                        _defenseStatSprite.SetTexture(_defenseStatTextures[1]);
                        _defenseStatSprite.Scale = new Vector2(1.2f);
                        break;

                    case PlayerTrait.Strength:
                        _strengthStatSprite.SetTexture(_strengthStatTextures[1]);
                        _strengthStatSprite.Scale = new Vector2(1.2f);
                        break;

                    case PlayerTrait.Dexterity:
                        _dexterityStatSprite.SetTexture(_dexterityStatTextures[1]);
                        _dexterityStatSprite.Scale = new Vector2(1.2f);
                        break;

                    case PlayerTrait.Stamina:
                        _staminaStatSprite.SetTexture(_staminaStatTextures[1]);
                        _staminaStatSprite.Scale = new Vector2(1.2f);
                        break;
                }
            }
        }

        private Sprite LoadStatUI(string name, Texture2D[] statTextures, Vector2 spritePosition)
        {
            statTextures[0] = Content.Load<Texture2D>($"UI/spr_{name}_ui");
            statTextures[1] = Content.Load<Texture2D>($"UI/spr_{name}_ui_alt");
            var statSprite = new Sprite(statTextures[0])
            {
                Origin = new Vector2(16.0f, 16.0f),
                Position = spritePosition
            };
            _uiSprites.Add(statSprite);
            return statSprite;
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
            MaxItemSpawnCount.Times(() =>
            {
                if (Rand.Next(2) == 1)
                {
                    var itemType = (ItemType)Rand.Next(2);
                    SpawnItem(itemType);
                }
            });

            MaxEnemySpawnCount.Times(() =>
            {
                if (Rand.Next(2) == 1)
                {
                    var enemyType = (EnemyType)Rand.Next(EnumExtensions.GetCount<EnemyType>());
                    SpawnEnemy(enemyType);
                }
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

            _audioListener.Position = new Vector3(playerPosition, 0.0f);

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

            var closestTorch = _level.Torches.WithMinimum(torch => Vector2.Distance(_player.Position, torch.Position));
            if (closestTorch != null)
                _fireAmbientSound.SetPosition(closestTorch.Position);

            var playerCurrentTile = _level.GetTile(playerPosition);
            if (_playerPreviousTile != playerCurrentTile)
            {
                _playerPreviousTile = playerCurrentTile;
                foreach (var enemy in _enemies)
                {
                    if (Vector2.Distance(enemy.Position, playerPosition) >= 300.0f)
                        continue;

                    enemy.UpdatePathFinding(_level, playerPosition);
                }
            }

            _camera.Position = playerPosition;

            if (_quest != null && _quest.IsCompleted)
            {
                _scoreTotal += Rand.Next(1000, 2001);
                _quest = null;
            }

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

                        if (_quest?.QuestType == QuestType.CollectGem)
                            _quest.DecreaseGoal(amount: 1);

                        PlaySound(_gemPickupSound);
                        break;

                    case ItemType.Gold:
                        {
                            var goldValue = ((Gold)item).GoldValue;

                            _goldTotal += goldValue;

                            if (_quest?.QuestType == QuestType.CollectGold)
                                _quest.DecreaseGoal(goldValue);

                            PlaySound(_coinPickupSound);
                        }
                        break;

                    case ItemType.Heart:
                        _player.Health += ((Heart)item).Health;
                        break;

                    case ItemType.Key:
                        _level.UnlockDoor();
                        _keyUiSprite.Color = Color.White;
                        PlaySound(_keyPickupSound);
                        break;

                    case ItemType.Potion:
                        {
                            var potion = (Potion)item;
                            switch (potion.PotionType)
                            {
                                case PotionType.Attack:
                                    _player.Attack += potion.Attack;
                                    break;

                                case PotionType.Defense:
                                    _player.Defense += potion.Defense;
                                    break;

                                case PotionType.Strength:
                                    _player.Strength += potion.Strength;
                                    break;

                                case PotionType.Dexterity:
                                    _player.Dexterity += potion.Dexterity;
                                    break;

                                case PotionType.Stamina:
                                    _player.Stamina += potion.Stamina;
                                    break;
                            }
                        }
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
                {
                    _player.TakeDamage(EnemyDamage);
                    PlaySound(_playerHitSound);
                }

                bool enemyWasRemoved = false;
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
                        position.X += Rand.Next(-15, 16);
                        position.Y += Rand.Next(-15, 16);
                        var itemType = Rand.Next(0, 2) == 0 ? ItemType.Gold : ItemType.Gem;
                        SpawnItem(itemType, position);
                    }

                    if (Rand.Next(5) == 0)
                    {
                        position.X += Rand.Next(-15, 16);
                        position.Y += Rand.Next(-15, 16);
                        SpawnItem(ItemType.Heart, position);
                    }
                    else if (Rand.Next(5) == 1)
                    {
                        position.X += Rand.Next(-15, 16);
                        position.Y += Rand.Next(-15, 16);
                        SpawnItem(ItemType.Potion, position);
                    }

                    PlaySound(_enemyDieSound, enemy.Position);

                    if (_quest?.QuestType == QuestType.KillEnemy)
                        _quest.DecreaseGoal(amount: 1);

                    _enemies.RemoveAt(i);
                    enemyWasRemoved = true;
                    break;
                }

                if (!enemyWasRemoved)
                    enemy.Update(gameTime);
            }
        }

        private void SpawnItem(ItemType itemType, Vector2? position = null)
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

                case ItemType.Potion:
                    item = new Potion(Content);
                    break;

                case ItemType.Key:
                    item = new Key(Content);
                    break;

                default:
                    throw new InvalidOperationException($"Unable to create an item with type '{itemType}'");
            }

            item.Position = position ?? _level.GetRandomSpawnLocation();
            _items.Add(item);
        }

        private void SpawnEnemy(EnemyType enemyType, Vector2? position = null)
        {
            Enemy enemy = null;
            switch (enemyType)
            {
                case EnemyType.Slime:
                    enemy = new Slime();
                    break;

                case EnemyType.Humanoid:
                    enemy = new Humanoid();
                    break;

                default:
                    throw new InvalidOperationException($"Unable to create an enemy with type '{enemyType}'");
            }

            enemy.Position = position ?? _level.GetRandomSpawnLocation();
            _enemies.Add(enemy);
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

            var spriteBatch = Global.SpriteBatch;

            var worldTransformMatrix = Global.DebugLevelGeneration ? 
                _viewportAdapter.GetScaleMatrix() : _camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: worldTransformMatrix);

            _level.Draw(spriteBatch, gameTime);

            foreach (var item in _items)
                item.Draw(spriteBatch, gameTime);

            foreach (var enemy in _enemies)
                enemy.Draw(spriteBatch, gameTime);

            foreach (var projectile in _playerProjectiles)
                projectile.Draw(spriteBatch, gameTime);

            _player.Draw(spriteBatch, gameTime);

            foreach (var sprite in _lightGrid)
                sprite.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: _viewportAdapter.GetScaleMatrix());

            _aimSprite.Draw(spriteBatch);

            // Quest
            if (_quest != null)
                DrawString(_quest.Title, new Vector2(_virtualSize.X / 2.0f, _virtualSize.Y - 75.0f), 1.3f);

            // Draw player stats
            DrawString(_player.Attack.ToString(), new Vector2(_virtualCenter.X - 210.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Defense.ToString(), new Vector2(_virtualCenter.X - 90.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Strength.ToString(), new Vector2(_virtualCenter.X + 30.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Dexterity.ToString(), new Vector2(_virtualCenter.X + 150.0f, _virtualSize.Y - 25.0f), 1.5f);
            DrawString(_player.Stamina.ToString(), new Vector2(_virtualCenter.X + 270.0f, _virtualSize.Y - 25.0f), 1.5f);

            DrawString(_scoreTotal.ToString().PadLeft(6, '0'), new Vector2(_virtualCenter.X - 120.0f, 50.0f), 2.0f);
            DrawString(_goldTotal.ToString().PadLeft(6, '0'), new Vector2(_virtualCenter.X + 220.0f, 50.0f), 2.0f);

            foreach (var sprite in _uiSprites)
                sprite.Draw(spriteBatch);

            DrawString($"Floor {_level.FloorNumber}", new Vector2(70.0f, _virtualSize.Y - 60.0f), 1.5f);
            DrawString($"Room {_level.RoomNumber}", new Vector2(70.0f, _virtualSize.Y - 25.0f), 1.5f);

            _healthBarSprite.TextureRect = new Rectangle(0, 0, (int)(213.0f / _player.MaxHealth * _player.Health), 8);
            _healthBarSprite.Draw(spriteBatch);

            _manaBarSprite.TextureRect = new Rectangle(0, 0, (int)(213.0f / _player.MaxMana * _player.Mana), 8);
            _manaBarSprite.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawString(string text, Vector2 position, float scale)
        {
            var textSize = Global.Font.MeasureString(text);
            Global.SpriteBatch.DrawString(Global.Font, text, position, Color.White, 
                rotation: 0.0f, 
                origin: textSize / 2.0f, 
                scale: scale, 
                effects: SpriteEffects.None, 
                layerDepth: 0.0f);
        }

        private void PlaySound(SoundEffect sound, Vector2? position = null)
        {
            var instance = sound.CreateInstance();
            instance.Pitch = Rand.Next(-5, 5) / 100.0f;
            instance.Play();

            if (position.HasValue)
            {
                _audioEmitter.Position = new Vector3(position.Value, 0.0f);
                instance.Apply3D(_audioListener, _audioEmitter);
            }
        }
    }
}
