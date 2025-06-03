using System.Collections.Generic;
using UnityEngine;
using static DungeonGenerator;

public class DungeonSplitter
{
    private int partsX, partsY, border;

    public DungeonSplitter(int partsX, int partsY, int border)
    {
        this.partsX = partsX;
        this.partsY = partsY;
        this.border = border;
    }

    // Splits the dungeon grid into a list of rectangular regions (RoomBounds) based on configured rows and columns.
    public List<RoomBounds> Split(TileType[,] grid)
    {
        // Get the dimensions of the grid.
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Calculate the width and height of each section, accounting for the outer border.
        int partWidth = (width - 2 * border) / partsX;
        int partHeight = (height - 2 * border) / partsY;

        var result = new List<RoomBounds>();

        // Loop through each section (grid partition).
        for (int y = 0; y < partsY; y++)
        {
            for (int x = 0; x < partsX; x++)
            {
                // Calculate the top-left corner of the current partition.
                int startX = x * partWidth + border;
                int startY = y * partHeight + border;

                // Calculate the bottom-right corner.
                int endX = startX + partWidth - 1;
                int endY = startY + partHeight - 1;

                // Create a RoomBounds object that defines this partition's corners and center.
                result.Add(new RoomBounds(
                    new(startX, startY),                     // topLeft
                    new(endX, startY),                       // topRight
                    new(startX, endY),                       // bottomLeft
                    new(endX, endY),                         // bottomRight
                    new((startX + endX) / 2, (startY + endY) / 2) // center
                ));
            }
        }

        // Return the list of all defined partitions.
        return result;
    }
}
