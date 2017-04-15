using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using Roguelike.Objects;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Roguelike
{
    public sealed class Level
    {
        public static int GridWidth = 19;
        public static int GridHeight = 19;

        private static readonly Point[] AdjacentTileIndexOffsets = new Point[]
        {
            new Point(0, -1),
            new Point(1, 0),
            new Point(0, 1),
            new Point(-1, 0)
        };

        private static readonly Point[] GeneratePathDirections = new Point[]
        {
            new Point(0, -2),
            new Point(2, 0),
            new Point(0, 2),
            new Point(-2, 0)
        };

        private readonly ContentManager _content;
        private readonly Texture2D[] _tileTextures = new Texture2D[EnumExtensions.GetCount<TileType>()];
        private readonly Tile[,] _tiles = new Tile[GridWidth, GridHeight];
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

            for (int x = 0; x < _tiles.GetLength(0); ++x)
                for (int y = 0; y < _tiles.GetLength(1); ++y)
                    _tiles[x, y] = new Tile(new Point(x, y));
        }

        public void SetTileTexture(string assetName, TileType tileType)
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
                var tile = _tiles[x, y];

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
            foreach (var tile in _tiles)
                tile.Sprite.Draw(spriteBatch);

            foreach (var torch in _torches)
                torch.Draw(spriteBatch, gameTime);
        }

        public Tile GetTile(Vector2 position)
        {
            position -= Origin.ToVector2();
            var x = (int)position.X / TileSize;
            var y = (int)position.Y / TileSize;
            return _tiles[x, y];
        }

        public void GenerateLevel()
        {
            for (var x = 0; x < GridWidth; ++x)
            {
                for (var y = 0; y < GridHeight; ++y)
                {
                    var tile = _tiles[x, y];
                    tile.Sprite = new Sprite();

                    if ((x % 2 != 0) && (y % 2 != 0))
                        SetTileType(tile, TileType.Empty);
                    else
                        SetTileType(tile, TileType.WallTop);

                    tile.Sprite.Position = (Origin + new Point(TileSize * x, TileSize * y)).ToVector2();
                }
            }

            CreatePath(new Point(1, 1));

            CreateRooms(roomCount: 10);

            UpdateTileTextures();
        }

        public Tile GetTileByIndex(Point tileIndex)
        {
            if (!TileIsValid(tileIndex))
                return null;

            return _tiles[tileIndex.X, tileIndex.Y];
        }

        public void SetTileType(Point index, TileType tileType)
        {
            if (!TileIsValid(index))
                return;

            var tile = _tiles[index.X, index.Y];
            SetTileType(tile, tileType);
        }

        private void SetTileType(Tile tile, TileType tileType)
        {
            tile.Type = tileType;

            var tileTexture = _tileTextures[(int)tileType];
            if (tileTexture != null)
                tile.Sprite.SetTexture(tileTexture);
        }

        private void CreatePath(Point startTileIndex)
        {
            var startTile = GetTileByIndex(startTileIndex);

            foreach (var direction in GeneratePathDirections.Shuffle())
            {
                var tile = GetTileByIndex(startTileIndex + direction);
                if (tile?.Type != TileType.Empty)
                    continue;

                SetTileType(tile, TileType.Floor);

                var wallIndex = startTileIndex + new Point(direction.X / 2, direction.Y / 2);
                var wall = GetTileByIndex(wallIndex);
                SetTileType(wall, TileType.Floor);

                CreatePath(tile.Index);
            }
        }

        private void CreateRooms(int roomCount)
        {
            roomCount.Times(() =>
            {
                var roomWidth = Rand.Next(1, 3);
                var roomHeight = Rand.Next(1, 3);

                var startIndex = new Point(
                    x: Rand.Next(1, GridWidth - 1),
                    y: Rand.Next(1, GridHeight - 1));

                for (var x = -1; x < roomWidth; ++x)
                {
                    for (var y = -1; y < roomHeight; ++y)
                    {
                        var index = startIndex + new Point(x, y);
                        var tile = GetTileByIndex(index);
                        if (tile == null)
                            continue;

                        if (tile.Index.X == 0 || tile.Index.X == GridWidth - 1 ||
                            tile.Index.Y == 0 || tile.Index.Y == GridHeight - 1)
                            continue;

                        SetTileType(tile, TileType.Floor);
                    }
                }
            });
        }

        private void UpdateTileTextures()
        {
            for (var x = 0; x < GridWidth; ++x)
            {
                for (var y = 0; y < GridHeight; ++y)
                {
                    if (!IsWall(x, y))
                        continue;

                    var tileType = 0;
                    if (IsWall(x, y - 1))
                        tileType += 1;
                    if (IsWall(x + 1, y))
                        tileType += 2;
                    if (IsWall(x, y + 1))
                        tileType += 4;
                    if (IsWall(x - 1, y))
                        tileType += 8;

                    SetTileType(_tiles[x, y], (TileType)tileType);
                }
            }
        }

        private bool IsWall(int indexX, int indexY)
        {
            var tile = GetTileByIndex(new Point(indexX, indexY));
            return tile?.IsWall == true;
        }

        public bool IsSolid(Point tileIndex)
        {
            if (!TileIsValid(tileIndex))
                return false;

            var tileType = _tiles[tileIndex.X, tileIndex.Y].Type;
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

            SetTileType(_doorTileIndices.Value, TileType.WallDoorUnlocked);
        }

        public Vector2 GetRandomSpawnLocation()
        {
            var tileIndex = Point.Zero;
            while (!IsFloor(tileIndex))
            {
                tileIndex = new Point(Rand.Next(GridWidth), Rand.Next(GridHeight));
            }

            var tileLocation = GetActualTileLocation(tileIndex);
            tileLocation.X += Rand.Next(-10, 11);
            tileLocation.Y += Rand.Next(-10, 11);

            return tileLocation;
        }

        public bool IsFloor(Point tileIndex) => _tiles[tileIndex.X, tileIndex.Y].IsFloor;

        public Vector2 GetActualTileLocation(Point tileIndex)
        {
            return new Vector2(
                x: Origin.X + (tileIndex.X * TileSize) + (TileSize / 2),
                y: Origin.Y + (tileIndex.Y * TileSize) + (TileSize / 2));
        }

        public Tile[] GetShortestPath(Tile start, Tile goal)
        {
            var closedSet = new HashSet<Tile>();

            var openSet = new HashSet<Tile>();
            openSet.Add(start);

            var cameFrom = new Dictionary<Tile, Tile>();

            var exactCosts = new Dictionary<Tile, int>();
            exactCosts.Add(start, 0);

            var estimatedCosts = new Dictionary<Tile, int>();
            estimatedCosts.Add(start, EstimateHeuristicCost(start, goal));

            while (openSet.Count > 0)
            {
                var current = openSet.WithMinimum(node => estimatedCosts.GetValueOrDefault(node, int.MaxValue));
                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in GetNeighborFloorTiles(current))
                {
                    if (closedSet.Contains(neighbor))
                        continue;

                    var tentativeCost = exactCosts[current] + 10;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else if (tentativeCost >= exactCosts.GetValueOrDefault(neighbor, int.MaxValue))
                        continue;

                    cameFrom[neighbor] = current;
                    exactCosts[neighbor] = tentativeCost;
                    estimatedCosts[neighbor] = exactCosts[neighbor] + EstimateHeuristicCost(neighbor, goal);
                }
            }

            return new Tile[0];
        }

        private static Tile[] ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
        {
            var path = new List<Tile>();

            path.Add(current);
            do
            {
                if (!cameFrom.TryGetValue(current, out current))
                    break;
                path.Add(current);
            } while (true);

            path.Reverse();
            return path.ToArray();
        }

        private IEnumerable<Tile> GetNeighborFloorTiles(Tile tile)
        {
            foreach (var indexOffset in AdjacentTileIndexOffsets)
            {
                var neighbor = GetTileByIndex(tile.Index + indexOffset);
                if (neighbor != null && neighbor.IsFloor)
                    yield return neighbor;
            }
        }

        private static int EstimateHeuristicCost(Tile from, Tile to)
        {
            var offset = from.Index - to.Index;
            return Math.Abs(offset.X) + Math.Abs(offset.Y);
        }
    }
}
