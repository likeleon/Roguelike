using Microsoft.Xna.Framework.Graphics;

namespace Roguelike
{
    public sealed class TileTextures
    {
        private readonly Texture2D[] _tileTextures = new Texture2D[EnumExtensions.GetCount<TileType>()];

        public TileTextures()
        {
            SetTileTexture("Tiles/spr_tile_floor", TileType.Floor);
            SetTileTexture("Tiles/spr_tile_floor_alt", TileType.FloorAlt);

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
        }

        private void SetTileTexture(string assetName, TileType tileType)
        {
            _tileTextures[(int)tileType] = Global.Content.Load<Texture2D>(assetName);
        }

        public Texture2D GetTexture(TileType tileType)
        {
            return _tileTextures[(int)tileType];
        }
    }
}
