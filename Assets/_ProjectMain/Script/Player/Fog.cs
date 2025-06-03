using UnityEngine;
using UnityEngine.UI;
public class Fog : MonoBehaviour
{
   [SerializeField] private int detectionrange;
    private DungeonContainer dungeonContainer;
    void Start()
    {
        dungeonContainer = FindAnyObjectByType<DungeonContainer>();
        if (detectionrange == 0) detectionrange = 5;
    }
    public void ExposeTiles()
    {
        var playerPos = GridUtility.WorldToGridPosition(transform.position);
        if(dungeonContainer == null) dungeonContainer = FindAnyObjectByType<DungeonContainer>();
        GameObject[,] Tiles = dungeonContainer.dungeonObjects;

        foreach(GameObject game in Tiles)
        {

            if (game.transform.Find("Sprite").gameObject.activeSelf) continue;
            var tilePos = GridUtility.WorldToGridPosition(game.transform.position);
            var range = PathfindingUtility.GetPathLength(playerPos, tilePos);
            if (range <= detectionrange || playerPos == tilePos)
            {
                GameObject fog = game.transform.Find("Fog")?.gameObject;
                Debug.Log(fog.name);

                GameObject tile = game.transform.Find("Sprite")?.gameObject;


                fog.GetComponent<FadingHandler>().StartFadeOut();///set the fog to false 
                tile.SetActive(true);//set the sprite to true

            }
         
            

        }

        UpdateSpriteRenderersForVisibleTiles<Enemy>();
        UpdateSpriteRenderersForVisibleTiles<ItemBase>();


         
        

    }

    public void UpdateSpriteRenderersForVisibleTiles<T>() where T : MonoBehaviour
    {
        if (dungeonContainer == null)
            dungeonContainer = FindAnyObjectByType<DungeonContainer>();

        GameObject[,] Tiles = dungeonContainer.dungeonObjects;

        T[] objects = GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);

        foreach (T obj in objects)
        {
            GameObject go = obj.gameObject;
            Vector2Int gridPos = GridUtility.WorldToGridPosition(go.transform.position);

            // Defensive bounds check
            if (gridPos.x < 0 || gridPos.y < 0 ||
                gridPos.x >= Tiles.GetLength(0) || gridPos.y >= Tiles.GetLength(1)) continue;
            bool tileIsVisible = Tiles[gridPos.x, gridPos.y].activeSelf;
            SpriteRenderer[] sr = go.GetComponentsInChildren<SpriteRenderer>();
            if (sr.Length > 0)
            {
                foreach(SpriteRenderer sr2 in sr) sr2.enabled = tileIsVisible;
            }

            Image[] image = go.GetComponentsInChildren<Image>();
            if(image.Length > 0)
            {
                foreach (Image i in image) i.enabled = tileIsVisible;
            }

            GameObject stairs = GameObject.Find("ev_stairs_down");
            if(stairs != null) stairs.SetActive(tileIsVisible);


        }
    }



}
