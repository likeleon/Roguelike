using Roguelike.Graphics;

namespace Roguelike
{
    public sealed class Tile
    {
        public int ColumnIndex { get; }
        public int RowIndex { get; }

        public TileType Type { get; set; }
        public Sprite Sprite { get; set; }

        public Tile(int columnIndex, int rowIndex)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }
    }
}
