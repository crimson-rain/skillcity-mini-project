using UnityEngine;
using static DungeonGenerator;

// Utility class for common dungeon generation operations.
public static class DungeonUtility
{
    // Fills the entire grid with Wall tiles.
    public static void FillWithWalls(TileType[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = TileType.Wall; // Initialize every tile as a wall
    }

    // Instantiates GameObjects for each tile in the grid, using the appropriate prefab.
    public static void SpawnTiles(
        TileType[,] grid, GameObject floor, GameObject wall, Transform parent, DungeonContainer dungeon)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                // Select prefab based on tile type
                GameObject prefab = grid[x, y] switch
                {
                    TileType.Floor => floor,
                    TileType.Wall => wall,
                    _ => null
                };

                // Instantiate the prefab if it's valid
                if (prefab != null)
                {
                    // Place tile at (x, 0, y) and parent it
                    dungeon.dungeonObjects[x, y] = Object.Instantiate(
                        prefab, new Vector3(x, 0, y), Quaternion.identity, parent);
                }
            }
    }

    // Calculates the overall dungeon grid size based on base size, border, padding, and number of rooms.
    public static Vector2Int CalculateDungeonSize(
        int baseX, int baseY, int roomsX, int roomsY, int border, int padding)
    {
        // Formula accounts for borders on all sides and padding between/around rooms.
        int sizeX = baseX + border * 2 + padding * 2 * roomsX;
        int sizeY = baseY + border * 2 + padding * 2 * roomsY;
        return new Vector2Int(sizeX, sizeY);
    }
}
