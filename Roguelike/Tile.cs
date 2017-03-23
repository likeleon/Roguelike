namespace Roguelike
{
    public sealed class Tile
    {
        public int ColumnIndex { get; }
        public int RowIndex { get; }

        public Tile(int columnIndex, int rowIndex)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }
    }
}
