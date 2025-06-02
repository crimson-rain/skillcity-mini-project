using UnityEngine;
using System.Collections.Generic;
public class DungeonContainer : MonoBehaviour
{
    public TileType[,] dungeon;
    public List<(int x, int y)> floorTiles = new List<(int x, int y)>();
    public List<(int x, int y)> corridoorTiles = new List<(int x, int y)>();
    public GameObject[,] dungeonObjects;
    public bool debug;
    public TileType[,] CreateDungeonGrid(int x, int y)//Creates a array of Tile Type enum and then clears the previous list referring to the old dungeon 
    {
        dungeon = new TileType[x, y];
        dungeonObjects = new GameObject[x, y];
        clearLists();
        return dungeon;
    }

    public void clearLists()//Clear the lists of floor tiles and corridoor tiles 
    {
        floorTiles.Clear();
        corridoorTiles.Clear();
    }



}
