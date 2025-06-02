using UnityEngine;
using System.Collections.Generic;
using static DungeonGenerator;

public class DungeonRoomGenerator
{
    private int minWidth, minHeight, padding;

    public DungeonRoomGenerator(int minW, int minH, int padding)
    {
        minWidth = minW;
        minHeight = minH;
        this.padding = padding;
    }

    // Generates a single room within the given bounds, places it on the grid, and stores its center point.
    public TileType[,] GenerateRoom(TileType[,] grid, RoomBounds bounds, List<Vector2Int> centers, DungeonContainer dungeon)
    {
        // Define usable area inside the room bounds by accounting for padding.
        int minX = bounds.topLeft.x + padding;
        int maxX = bounds.topRight.x - padding;
        int minY = bounds.topLeft.y + padding;
        int maxY = bounds.bottomLeft.y - padding;

        // Calculate how much space is available to place a room.
        int usableWidth = maxX - minX + 1;
        int usableHeight = maxY - minY + 1;

        // If the usable area is too small, skip room generation.
        if (usableWidth < minWidth || usableHeight < minHeight)
            return grid;

        // Randomly determine room size within the usable area.
        int width = Random.Range(minWidth, usableWidth + 1);
        int height = Random.Range(minHeight, usableHeight + 1);

        // Randomly offset the room within the usable area.
        int offsetX = Random.Range(0, usableWidth - width + 1);
        int offsetY = Random.Range(0, usableHeight - height + 1);

        // Final position of the room in the grid.
        int startX = minX + offsetX;
        int startY = minY + offsetY;

        // Fill in the grid with floor tiles to form the room.
        for (int x = startX; x < startX + width; x++)
            for (int y = startY; y < startY + height; y++)
            {
                grid[x, y] = TileType.Floor;
                dungeon.floorTiles.Add((x, y)); // Track tile in the floor tile set.
            }

        // Record the center of the room for later corridor connections.
        centers.Add(new(startX + width / 2, startY + height / 2));

        // Return the updated grid.
        return grid;
    }
}
