using UnityEngine;

public class Fog : MonoBehaviour
{
   [SerializeField] private int detectionrange;
    private DungeonContainer dungeonContainer;
    void Start()
    {
        dungeonContainer = FindAnyObjectByType<DungeonContainer>();

    }
    public void ExposeTiles()
    {
        var playerPos = GridUtility.WorldToGridPosition(transform.position);
        if(dungeonContainer == null) dungeonContainer = FindAnyObjectByType<DungeonContainer>();
        GameObject[,] Tiles = dungeonContainer.dungeonObjects;

        foreach(GameObject game in Tiles)
        {
            if (game.activeSelf) continue;
            var tilePos = GridUtility.WorldToGridPosition(game.transform.position);
            var range = PathfindingUtility.GetPathLength(playerPos, tilePos);
            if (range <= detectionrange) game.SetActive(true);
            if (playerPos == tilePos) game.SetActive(true);

        }

         
        

    }



}
