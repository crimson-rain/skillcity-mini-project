using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Prefabs & Settings")]
    public GameObject floor;
    public GameObject wall;
    public GameObject player;
    public int dungeonSizeX = 52;
    public int dungeonSizeY = 28;
    public int roomNumX = 3;
    public int roomNumY = 2;
    public int minRoomX = 5;
    public int minRoomY = 4;

    [SerializeField] private int roomPadding = 1;
    [SerializeField] private int dungeonBorder = 1;
    [Range(0, 1)] public float straightCorridorChance = 0.5f;
    public bool regenerateOnFlag;
    DungeonContainer dungeonGridContainer;
    private GameObject dungeonContainer;
    private float roomChance = 0.7f;
    private List<RoomBounds> roomBoundsList;
    private List<Vector2Int> roomCenters = new();
    [SerializeField] private int growOnFloor;
    private DungeonSettings defaultSettings;
    public TileType[,] dungeonGrid;

    public EnemySpawner enemySpawner;
    public ItemSpawner itemSpawner;
    public TrapSpawner trapSpawner;
    public StairsSpawner stairsSpawner;
    private void Start()
    {
        dungeonGridContainer = GetComponent<DungeonContainer>();
        defaultSettings = new DungeonSettings(minRoomX, minRoomY, roomNumX, roomNumY, dungeonSizeX, dungeonSizeY);
        GenerateDungeon();
    }

    private void Update()
    {
        //Debug optin to make a new dungeon in the 
        if (regenerateOnFlag)
        {
            GenerateDungeon();
            regenerateOnFlag = false;
        }
    }

    public void GenerateDungeon()
    {

        //Reset the items and enemies, avoiding all 
        ResetEntities();

        //Clears the lists containing the centre point of each rooom 
        roomCenters.Clear();
        //CLears the list containing the 4 corner points of each rooom 
        roomBoundsList = new();

        // Clean old dungeon
        if (dungeonContainer != null)
            Destroy(dungeonContainer);


        //Create new dungeon container what all the tiles will be stored in
        dungeonContainer = new GameObject("DungeonContainer");
        dungeonContainer.transform.parent = this.transform;

        // Adjust dimensions
        Vector2Int dungeonSize = DungeonUtility.CalculateDungeonSize(
            defaultSettings.width, defaultSettings.height, roomNumX, roomNumY, dungeonBorder, roomPadding);
        dungeonSizeX = dungeonSize.x + (TurnManager.Instance.FLoorNumber *growOnFloor);
        dungeonSizeY = dungeonSize.y + (TurnManager.Instance.FLoorNumber * growOnFloor);

        //Fills the area of the dungeon with blank walls 
        dungeonGrid = dungeonGridContainer.CreateDungeonGrid(dungeonSizeX, dungeonSizeY);
        DungeonUtility.FillWithWalls(dungeonGrid);

        //Decides how many of the rooms should be spawned
        DecideDungeonType();

        // Room splitting - splits the room up into smaller sections
        var splitter = new DungeonSplitter(roomNumX, roomNumY, dungeonBorder);
        roomBoundsList = splitter.Split(dungeonGrid);

        // Room placement - places rooms in those sections
        var roomGen = new DungeonRoomGenerator(minRoomX, minRoomY, roomPadding);
        foreach (var room in roomBoundsList)
        {
            if (Random.value <= roomChance)
            {
                dungeonGrid = roomGen.GenerateRoom(dungeonGrid, room, roomCenters, dungeonGridContainer);
            }
        }

        // Connect rooms - uses the centre points and draws a line 
        var connector = new DungeonConnector(straightCorridorChance, dungeonSizeX, dungeonSizeY);
        dungeonGrid = connector.ConnectRooms(dungeonGrid, roomCenters, dungeonGridContainer);


        dungeonGridContainer.dungeon = dungeonGrid;

        //Uses the array generated to spawn the tile objects   
        DungeonUtility.SpawnTiles(dungeonGrid, floor, wall, dungeonContainer.transform, dungeonGridContainer);



        //Places player in random spots
        int rand = Random.Range(0, dungeonGridContainer.floorTiles.Count);
        int spawnPosX = dungeonGridContainer.floorTiles[rand].x;
        int spawnPosY = dungeonGridContainer.floorTiles[rand].y;
        GameObject playerSpawn = dungeonGridContainer.dungeonObjects[spawnPosX, spawnPosY];
        player.transform.position = playerSpawn.transform.position;

        //passes grid position to player to allow movement
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.GridPosition = new Vector2Int(spawnPosX, spawnPosY);
            playerController.dungeonGridContainer = dungeonGridContainer; // Optional, if needed
        }

        //passes generated gris to the spawners
        itemSpawner.dungeonContainer = dungeonGridContainer;
        enemySpawner.dungeonContainer = dungeonGridContainer;
        trapSpawner.dungeonContainer = dungeonGridContainer;

        //spawns an initial batch of enemies bas
        stairsSpawner.BatchSpawn(1);
        itemSpawner.BatchSpawn(5);
        enemySpawner.BatchSpawn(5);
        trapSpawner.BatchSpawn(5);

        foreach(GameObject game in DungeonContainer.FindAnyObjectByType<DungeonContainer>().dungeonObjects)
        {
            game.SetActive(false);
        }//set each tile to false to act as "fog"

        //Generate Seen tiles
        Fog fog = FindAnyObjectByType<Fog>();
        fog.ExposeTiles();
    }

    private void DecideDungeonType()
    {
        //Gets random tule type
        var type = DungeonTypeUtility.GetRandom();
        switch (type)
        {
            case DungeonType.SingleRoom:

            //sets the room to be 90% of the whole dungeon
            minRoomX = Mathf.CeilToInt(defaultSettings.width * 0.9f);
            minRoomY = Mathf.CeilToInt(defaultSettings.height * 0.9f);
            //makes it so only 1 room can spawn
            roomNumX = roomNumY = 1;

            // makes it guarenteed for a room to spawn
            roomChance = 1.1f;
            break;
            case DungeonType.fullRoom:
            //sets to default settings
            ResetToDefaultSettings();
            // makes it guarenteed for a room to spawn
            roomChance = 1f;
            break;
            case DungeonType.halfRoom:
            //sets to default settings
            ResetToDefaultSettings();
            // makes it a 50% chance for a room to spawn so only half the rooms will spawn
            roomChance = 0.5f;
            break;
            case DungeonType.threeQuartRoom:
            //sets to default settings
            ResetToDefaultSettings();
            //makes it a 75% chance for a room to spawn
            roomChance = 0.75f;
            break;
        }


    }//Decides on which dungeoh type the dungeon should be

    private void ResetToDefaultSettings() // sets the room numbers and room sizes to whatever was set in inspector to undo single rooms
    {
        minRoomX = defaultSettings.minRoomX;
        minRoomY = defaultSettings.minRoomY;
        roomNumX = defaultSettings.roomNumX;
        roomNumY = defaultSettings.roomNumY;
    }

    private void ResetEntities()
    {
        //Gets the player inventory
        Inventory inv = GetComponent<Inventory>();
        ItemBase[] ib = FindObjectsByType<ItemBase>(FindObjectsSortMode.None);
        foreach (ItemBase item in ib)
        {
            //Destroys item is it isn't in the players inventory (will be disabled if it is)
            if (item.isActiveAndEnabled) Destroy(item.gameObject);

        }

        //Destroys every enemy on the field
        Enemy[] enemy = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy item in enemy)
        {

            Destroy(item.gameObject);
        }



    }//Destroys all enemies and items in preperation for respawning the dungeon. Leaves items in inventory 
}
