using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
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
        private readonly Texture2D[] _tileTextures = new Texture2D[Enum.GetValues(typeof(TileType)).Length];
        private readonly Tile[,] _grid = new Tile[GridWidth, GridHeight];
        private readonly List<Torch> _torches = new List<Torch>();
        private Point? _doorTileIndices;

        public Point Origin { get; }
        public Point Size => new Point(GridWidth, GridHeight);
        public int TileSize { get; } = 50;

        public Level(ContentManager content, Point screenSize)
        {
            _content = content;

            AddTile("Tiles/spr_tile_floor", TileType.Floor);

            AddTile("Tiles/spr_tile_wall_top", TileType.WallTop);
            AddTile("Tiles/spr_tile_wall_top_left", TileType.WallTopLeft);
            AddTile("Tiles/spr_tile_wall_top_right", TileType.WallTopRight);
            AddTile("Tiles/spr_tile_wall_top_t", TileType.WallTopT);
            AddTile("Tiles/spr_tile_wall_top_end", TileType.WallTopEnd);

            AddTile("Tiles/spr_tile_wall_bottom_left", TileType.WallBottomLeft);
            AddTile("Tiles/spr_tile_wall_bottom_right", TileType.WallBottomRight);
            AddTile("Tiles/spr_tile_wall_bottom_t", TileType.WallBottomT);
            AddTile("Tiles/spr_tile_wall_bottom_end", TileType.WallBottomEnd);

            AddTile("Tiles/spr_tile_wall_side", TileType.WallSide);
            AddTile("Tiles/spr_tile_wall_side_left_t", TileType.WallSideLeftT);
            AddTile("Tiles/spr_tile_wall_side_left_end", TileType.WallSideLeftEnd);
            AddTile("Tiles/spr_tile_wall_side_right_t", TileType.WallSideRightT);
            AddTile("Tiles/spr_tile_wall_side_right_end", TileType.WallSideRightEnd);

            AddTile("Tiles/spr_tile_wall_intersection", TileType.WallIntersection);
            AddTile("Tiles/spr_tile_wall_single", TileType.WallSingle);

            AddTile("Tiles/spr_tile_wall_entrance", TileType.WallEntrance);
            AddTile("Tiles/spr_tile_door_locked", TileType.WallDoorLocked);
            AddTile("Tiles/spr_tile_door_unlocked", TileType.WallDoorUnlocked);

            var originX = (screenSize.X - GridWidth * TileSize) / 2;
            var originY = (screenSize.Y - GridHeight * TileSize) / 2;
            Origin = new Point(originX, originY);

            for (int x = 0; x < _grid.GetLength(0); ++x)
                for (int y = 0; y < _grid.GetLength(1); ++y)
                    _grid[x, y] = new Tile(x, y);
        }

        private void AddTile(string assetName, TileType tileType)
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
    }
}
