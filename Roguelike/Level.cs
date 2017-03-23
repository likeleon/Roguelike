using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Roguelike
{
    public sealed class Level
    {
        private static int GridWidth = 19;
        private static int GridHeight = 19;
        private static int TileSize = 50;

        private readonly Texture2D[] _tileTextures = new Texture2D[Enum.GetValues(typeof(TileType)).Length];
        private readonly Point _origin;
        private readonly Tile[,] _grid = new Tile[GridWidth, GridHeight];

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
            _origin = new Point(originX, originY);

            for (int x = 0; x < _grid.GetLength(0); ++x)
                for (int y = 0; y < _grid.GetLength(1); ++y)
                    _grid[x, y] = new Tile(x, y);
        }

        private void AddTile(ContentManager content, string assetName, TileType tileType)
        {
            _tileTextures[(int)tileType] = content.Load<Texture2D>(assetName);
        }
    }
}
