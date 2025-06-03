using UnityEngine;
using System.Collections.Generic;
using static DungeonGenerator;

public class DungeonConnector
{
    private float corridorStraightness;
    private int width, height;

    public DungeonConnector(float straightness, int width, int height)
    {
        corridorStraightness = straightness;
        this.width = width;
        this.height = height;
    }

   
    public TileType[,] ConnectRooms(TileType[,] grid, List<Vector2Int> centers, DungeonContainer dungeon)
    {
        // If there's only one room, no corridors are needed.
        if (centers.Count == 1) return grid;

        // Loop through each room center and connect it to the next one in the list.
        for (int i = 0; i < centers.Count; i++)
        {
            // Starting point: current room center.
            Vector2Int start = centers[i];
            // Ending point: next room center (wraps around at the end).
            Vector2Int end = centers[(i + 1) % centers.Count];

            // Current coordinates start from the start room center.
            int x = start.x, y = start.y;

            // Randomly decide whether to prioritize horizontal (x) or vertical (y) movement first.
            bool preferX = Random.value < 0.5f;
            string last = preferX ? "x" : "y"; // Track the last direction moved.

            // Keep moving until we reach the end room center.
            while (x != end.x || y != end.y)
            {
                // Decide whether to step in the x direction:
                //  - If we're not already at the correct x,
                //  - And either we're already at the correct y,
                //  - Or based on random chance (influenced by corridorStraightness).
                bool stepX = x != end.x && (y == end.y ||
                             (Random.value < corridorStraightness ? last == "x" : Random.value < 0.5f));

                if (stepX)
                {
                    // Move in x direction toward the target.
                    x += (int)Mathf.Sign(end.x - x);
                    last = "x";
                }
                else
                {
                    // Move in y direction toward the target.
                    y += (int)Mathf.Sign(end.y - y);
                    last = "y";
                }

                // Ensure we're within bounds of the grid.
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    var pos = (x, y);

                    // If the tile is a wall or not already part of the dungeon floor,
                    // convert it to a corridor tile.
                    if (grid[x, y] == TileType.Wall || !dungeon.floorTiles.Contains(pos))
                    {
                        grid[x, y] = TileType.Floor;
                        dungeon.corridoorTiles.Add(pos);
                    }
                }
            }
        }

        // Return the updated grid with connected corridors.
        return grid;
    } // Connects all rooms by drawing corridors between their center points.
}
