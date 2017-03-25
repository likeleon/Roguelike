using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using System;
using System.IO;
using System.Linq;

namespace Roguelike
{
    public sealed class Level
    {
        private static int GridWidth = 19;
        private static int GridHeight = 19;
  
        private readonly Texture2D[] _tileTextures = new Texture2D[Enum.GetValues(typeof(TileType)).Length];
        private readonly Tile[,] _grid = new Tile[GridWidth, GridHeight];
        private Point? _doorTileIndices;

        public Point Origin { get; }
        public Point Size => new Point(GridWidth, GridHeight);
        public int TileSize { get; } = 50;

        public Level(ContentManager content, Point screenSize)
        {
            AddTile(content, "Tiles/spr_tile_floor", TileType.Floor);

            AddTile(content, "Tiles/spr_tile_wall_top", TileType.WallTop);
            AddTile(content, "Tiles/spr_tile_wall_top_left", TileType.WallTopLeft);
            AddTile(content, "Tiles/spr_tile_wall_top_right", TileType.WallTopRight);
            AddTile(content, "Tiles/spr_tile_wall_top_t", TileType.WallTopT);
            AddTile(content, "Tiles/spr_tile_wall_top_end", TileType.WallTopEnd);

            AddTile(content, "Tiles/spr_tile_wall_bottom_left", TileType.WallBottomLeft);
            AddTile(content, "Tiles/spr_tile_wall_bottom_right", TileType.WallBottomRight);
            AddTile(content, "Tiles/spr_tile_wall_bottom_t", TileType.WallBottomT);
            AddTile(content, "Tiles/spr_tile_wall_bottom_end", TileType.WallBottomEnd);

            AddTile(content, "Tiles/spr_tile_wall_side", TileType.WallSide);
            AddTile(content, "Tiles/spr_tile_wall_side_left_t", TileType.WallSideLeftT);
            AddTile(content, "Tiles/spr_tile_wall_side_left_end", TileType.WallSideLeftEnd);
            AddTile(content, "Tiles/spr_tile_wall_side_right_t", TileType.WallSideRightT);
            AddTile(content, "Tiles/spr_tile_wall_side_right_end", TileType.WallSideRightEnd);

            AddTile(content, "Tiles/spr_tile_wall_intersection", TileType.WallIntersection);
            AddTile(content, "Tiles/spr_tile_wall_single", TileType.WallSingle);

            AddTile(content, "Tiles/spr_tile_wall_entrance", TileType.WallEntrance);
            AddTile(content, "Tiles/spr_tile_door_locked", TileType.WallDoorLocked);
            AddTile(content, "Tiles/spr_tile_door_unlocked", TileType.WallDoorUnlocked);

            var originX = (screenSize.X - GridWidth * TileSize) / 2;
            var originY = (screenSize.Y - GridHeight * TileSize) / 2;
            Origin = new Point(originX, originY);

            for (int x = 0; x < _grid.GetLength(0); ++x)
                for (int y = 0; y < _grid.GetLength(1); ++y)
                    _grid[x, y] = new Tile(x, y);
        }

        private void AddTile(ContentManager content, string assetName, TileType tileType)
        {
            _tileTextures[(int)tileType] = content.Load<Texture2D>(assetName);
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in _grid)
                tile.Sprite.Draw(spriteBatch);
        }
    }
}
