using Microsoft.Xna.Framework;
using Roguelike.Graphics;

namespace Roguelike
{
    public sealed class Tile
    {
        public Point Index { get; }
        public TileType Type { get; set; }
        public Sprite Sprite { get; set; }

        public Tile(Point index)
        {
            Index = index;
        }
    }
}
