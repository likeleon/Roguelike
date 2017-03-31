using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using Roguelike.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Roguelike
{
    public sealed class Level
    {
        private static int GridWidth = 19;
        private static int GridHeight = 19;

        private readonly ContentManager _content;
        private readonly Texture2D[] _tileTextures = new Texture2D[EnumExtensions.GetEnumLength<TileType>()];
        private readonly Tile[,] _grid = new Tile[GridWidth, GridHeight];
        private readonly List<Torch> _torches = new List<Torch>();
        private Point? _doorTileIndices;

        public int FloorNumber => 1;
        public int RoomNumber => 0;

        public Point Origin { get; }
        public Point Size => new Point(GridWidth, GridHeight);
        public int TileSize { get; } = 50;
        public IEnumerable<Torch> Torches => _torches;

        public Level(ContentManager content, Point screenSize)
        {
            _content = content;

            SetTileTexture("Tiles/spr_tile_floor", TileType.Floor);

            SetTileTexture("Tiles/spr_tile_wall_top", TileType.WallTop);
            SetTileTexture("Tiles/spr_tile_wall_top_left", TileType.WallTopLeft);
            SetTileTexture("Tiles/spr_tile_wall_top_right", TileType.WallTopRight);
            SetTileTexture("Tiles/spr_tile_wall_top_t", TileType.WallTopT);
            SetTileTexture("Tiles/spr_tile_wall_top_end", TileType.WallTopEnd);

            SetTileTexture("Tiles/spr_tile_wall_bottom_left", TileType.WallBottomLeft);
            SetTileTexture("Tiles/spr_tile_wall_bottom_right", TileType.WallBottomRight);
            SetTileTexture("Tiles/spr_tile_wall_bottom_t", TileType.WallBottomT);
            SetTileTexture("Tiles/spr_tile_wall_bottom_end", TileType.WallBottomEnd);

            SetTileTexture("Tiles/spr_tile_wall_side", TileType.WallSide);
            SetTileTexture("Tiles/spr_tile_wall_side_left_t", TileType.WallSideLeftT);
            SetTileTexture("Tiles/spr_tile_wall_side_left_end", TileType.WallSideLeftEnd);
            SetTileTexture("Tiles/spr_tile_wall_side_right_t", TileType.WallSideRightT);
            SetTileTexture("Tiles/spr_tile_wall_side_right_end", TileType.WallSideRightEnd);

            SetTileTexture("Tiles/spr_tile_wall_intersection", TileType.WallIntersection);
            SetTileTexture("Tiles/spr_tile_wall_single", TileType.WallSingle);

            SetTileTexture("Tiles/spr_tile_wall_entrance", TileType.WallEntrance);
            SetTileTexture("Tiles/spr_tile_door_locked", TileType.WallDoorLocked);
            SetTileTexture("Tiles/spr_tile_door_unlocked", TileType.WallDoorUnlocked);

            var originX = (screenSize.X - GridWidth * TileSize) / 2;
            var originY = (screenSize.Y - GridHeight * TileSize) / 2;
            Origin = new Point(originX, originY);

            for (int x = 0; x < _grid.GetLength(0); ++x)
                for (int y = 0; y < _grid.GetLength(1); ++y)
                    _grid[x, y] = new Tile(new Point(x, y));
        }

        private void SetTileTexture(string assetName, TileType tileType)
        {
            _tileTextures[(int)tileType] = _content.Load<Texture2D>(assetName);
        }

        public void LoadFromFile(string fileName)
        {
            int[] tileIds;
            using (var stream = TitleContainer.OpenStream(fileName))
            using (var streamReader = new StreamReader(stream))
            {
                var str = streamReader.ReadToEnd();
                var splitSeparators = new[] { '[', ']', '\r', '\n' };
                var tileIdStrs = str.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);
                tileIds = tileIdStrs.Select(int.Parse).ToArray();
            }

            if (tileIds.Length != GridWidth * GridHeight)
                throw new InvalidDataException("Invalid level file, tile count mismatch");

            for (int i = 0; i < tileIds.Length; ++i)
            {
                int x = i % GridWidth;
                int y = i / GridWidth;
                var tile = _grid[x, y];

                tile.Type = (TileType)tileIds[i];
                tile.Sprite = new Sprite(_tileTextures[tileIds[i]]);
                tile.Sprite.Position = (Origin + new Point(TileSize * x, TileSize * y)).ToVector2();

                if (tile.Type == TileType.WallDoorLocked)
                    _doorTileIndices = new Point(x, 0);
            }

            var torchTilePositions = new[]
            {
                new Point(3, 9),
                new Point(7, 7),
                new Point(11, 11),
                new Point(13, 15),
                new Point(15, 3)
            };
            foreach (var tilePos in torchTilePositions)
            {
                var torch = new Torch(_content);
                var x = tilePos.X * TileSize + TileSize / 2;
                var y = tilePos.Y * TileSize + TileSize / 2;
                torch.Position = Origin.ToVector2() + new Vector2(x, y);
                _torches.Add(torch);
            };
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var tile in _grid)
                tile.Sprite.Draw(spriteBatch);

            foreach (var torch in _torches)
                torch.Draw(spriteBatch, gameTime);
        }

        public Tile GetTile(Vector2 index)
        {
            index -= Origin.ToVector2();
            var x = (int)index.X / TileSize;
            var y = (int)index.Y / TileSize;
            return _grid[x, y];
        }

        public void SetTile(Point index, TileType tileType)
        {
            if (!TileIsValid(index))
                return;

            var tile = _grid[index.X, index.Y];
            tile.Type = tileType;
            tile.Sprite.SetTexture(_tileTextures[(int)tileType]);
        }

        public bool IsSolid(Point tileIndex)
        {
            if (!TileIsValid(tileIndex))
                return false;

            var tileType = _grid[tileIndex.X, tileIndex.Y].Type;
            return tileType != TileType.Floor &&
                tileType != TileType.FloorAlt &&
                tileType != TileType.WallDoorUnlocked;
        }

        public bool TileIsValid(Point tileIndex)
        {
            if (tileIndex.X < 0 || tileIndex.X >= GridWidth)
                return false;

            if (tileIndex.Y < 0 || tileIndex.Y >= GridHeight)
                return false;

            return true;
        }

        public void UnlockDoor()
        {
            if (!_doorTileIndices.HasValue)
                return;

            SetTile(_doorTileIndices.Value, TileType.WallDoorUnlocked);
        }

        public Vector2 GetRandomSpawnLocation()
        {
            var tileIndex = Point.Zero;
            while (!IsFloor(tileIndex))
            {
                tileIndex = new Point(RandomGenerator.Next(GridWidth), RandomGenerator.Next(GridHeight));
            }

            var tileLocation = GetActualTileLocation(tileIndex);
            tileLocation.X += RandomGenerator.Next(-10, 11);
            tileLocation.Y += RandomGenerator.Next(-10, 11);

            return tileLocation;
        }

        public bool IsFloor(Point tileIndex)
        {
            var tileType = _grid[tileIndex.X, tileIndex.Y].Type;
            return tileType == TileType.Floor || tileType == TileType.FloorAlt;
        }

        private Vector2 GetActualTileLocation(Point tileIndex)
        {
            return new Vector2(
                x: Origin.X + (tileIndex.X * TileSize) + (TileSize / 2),
                y: Origin.Y + (tileIndex.Y * TileSize) + (TileSize / 2));
        }
    }
}
