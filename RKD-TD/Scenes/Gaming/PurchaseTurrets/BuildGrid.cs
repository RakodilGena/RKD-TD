using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Cameras;

namespace RKD_TD.Scenes.Gaming.PurchaseTurrets;

internal sealed class BuildGrid
{
    private readonly BuildCell[,] _cells;
    private readonly Vector2 _cellSize;

    public BuildGrid(int[,] buildableLayer, Vector2 cellSize)
    {
        _cellSize = cellSize;

        int rows = buildableLayer.GetLength(0);
        int cols = buildableLayer.GetLength(1);
        _cells = new BuildCell[rows, cols];

        for (int y = 0; y < rows; y++)
        for (int x = 0; x < cols; x++)
        {
            _cells[y, x] = new BuildCell
            {
                WorldPosition = new Vector2(x * cellSize.X, y * cellSize.Y),
                IsBuildable = buildableLayer[y, x] == 1,
                CellSize = _cellSize
            };
        }
    }

    public BuildCell? GetCellAtWorld(Vector2 worldPos, ICamera camera)
    {
        var position = camera.ScreenToWorld(worldPos);

        Vector2 tileIndex = position / _cellSize;
        int x = (int)tileIndex.X;
        int y = (int)tileIndex.Y;

        if (x < 0 || y < 0 || x >= _cells.GetLength(1) || y >= _cells.GetLength(0))
            return null;

        return _cells[y, x];
    }

    public static BuildGrid FromMap(
        XDocument mapDocument)
    {
        XElement root = mapDocument.Root!;
        XElement tilesetElement = root.Element("Tileset")!;

        Vector2 tileSize = new Vector2(
            int.Parse(tilesetElement.Attribute("tileWidth")!.Value),
            int.Parse(tilesetElement.Attribute("tileHeight")!.Value));

        var buildGridElement = root.Element("BuildGrid")!;

        // Split the value of the tiles data into rows by splitting on
        // the new line character
        string[] rows = buildGridElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Split the value of the first row to determine the total number of columns
        int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

        // Create the tilemap
        int[,] buildableLayer = new int[rows.Length, columnCount];

        // Process each row
        for (int row = 0; row < rows.Length; row++)
        {
            // Split the row into individual columns
            string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            // Process each column of the current row
            for (int column = 0; column < columnCount; column++)
            {
                int cell = int.Parse(columns[column]);

                buildableLayer[row, column] = cell;
            }
        }

        return new BuildGrid(buildableLayer, tileSize);
    }
}