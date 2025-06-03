using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject target;
    public Stats stats;
    public float delay;
    public bool debug;
    private int closeAlly;
    DungeonContainer dungeonContainer;
    Dictionary<string, float> probabilities = new Dictionary<string, float>
        {
            { "Move", 20f },
            { "Attack", 30f },
            { "Retreat", 50f }
        };
    Dictionary<string, bool> locks  = new Dictionary<string, bool>
{
    { "Move",    false },
    { "Attack",  false },
    { "Retreat", false  }  
};

    //private bool attackLock, moveLock, retreatLock;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        stats = GetComponent<Stats>();
        dungeonContainer = FindFirstObjectByType<DungeonContainer>();
    }

    public IEnumerator TakeTurn()
    {
        if (this == null || gameObject == null) yield break;
        if (gameObject == null)
        {
            Debug.Log("Null reference to seld");
            yield return null;  
        }
        if(target == null)
        {
            Debug.Log("Null reference to target");
            yield return null;
        }
        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);
        Vector2Int playerPos = GridUtility.WorldToGridPosition(target.transform.position);

        int distanceToPlayer = PathfindingUtility.GetPathLength(myPos, playerPos);


        if (distanceToPlayer > stats.detectionRange)
        {        
            yield return RandomMoveFarStep();
        }//if not in detection just move about

        DecidePlan();
        int intDebuff = (40 - (4 * stats.intelligence));
        int rand = UnityEngine.Random.Range(0, 100 + intDebuff);  

        float moveP = probabilities["Move"];
        float attackP = probabilities["Attack"];
        float retreatP = probabilities["Retreat"];
        float sum = moveP + attackP + retreatP;

        if (rand < moveP)
        {
            yield return RandomMoveStep();
            stats.currentAction=("Rand Move");
        }
        else if (rand < moveP + attackP)
        {
            yield return AttackPlayer();
            stats.currentAction = ("ATK");
        }
        else if (rand < sum)
        {
            yield return RetreatStep();
            stats.currentAction = ("Retreat");
        }
        else
        {
          
            int choice = UnityEngine.Random.Range(0, 3);
            switch (choice)
            {
                case 0:
                yield return RandomMoveStep();
                stats.currentAction = ("Rand Move - rand action");
                break;
                case 1:
                yield return AttackPlayer();
                stats.currentAction = ("ATK - Rand action");
                break;
                default: // case 2
                yield return RetreatStep();
                stats.currentAction = ("Retreat - Rand action");
                break;
            }
        }

        
        if (debug)
            yield return AttackPlayer();

    
        var keys = new List<string>(locks.Keys);
        foreach (var k in keys)
            locks[k] = false;
    }

    private void DecidePlan()//decides high level gameplan
    {
        closeAlly = 0;
        if (target == null) return;

        Stats playerStats = target.GetComponent<Stats>();
        if (playerStats == null) return;

        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);
        Vector2Int playerPos = GridUtility.WorldToGridPosition(target.transform.position);

        int distanceToPlayer = PathfindingUtility.GetPathLength(myPos, playerPos);
        

        if(distanceToPlayer > stats.detectionRange)
        {
           HandlePercentages(probabilities, "Move", 100f, isAbsolute: true, locks);
            locks["Move"] = true;
            //Debug.Log("Not in range");

            return;
        }//if not in detection just move about

        if(stats.intelligence ==0)
        {
            HandlePercentages(probabilities, "Attack", 100f, isAbsolute: true, locks);
            locks["Attack"] = true;
        }



        //int closestEnemyDist = int.MaxValue;
        foreach (Enemy other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (other == this) continue;

            Vector2Int otherPos = GridUtility.WorldToGridPosition(other.transform.position);
            int dist = PathfindingUtility.GetPathLength(myPos, otherPos);
            if (dist < stats.allyThreshold) closeAlly ++;
        }
        if (closeAlly == 0) closeAlly = 1;
        switch(stats.personality)
        {
            case PersonalityType.Shy:
            HandlePercentages(probabilities, "Attack", -50/closeAlly, isAbsolute: false, locks);
            break;
            case PersonalityType.Aggressive:
            HandlePercentages(probabilities, "Attack", 10 * closeAlly, isAbsolute: false, locks);
            break;
        }//emboldens based on personality type and number of close allies

     
        if (stats.currentHealth < 5)//retreat if low health
        {
            if (stats.personality == PersonalityType.Shy)
            {
                HandlePercentages(probabilities, "Retreat", 80 , isAbsolute: false, locks);
            }
            else
            {
                HandlePercentages(probabilities, "Retreat", -20 , isAbsolute: false, locks);
            }
        }

        if (playerStats.currentHealth < stats.damage)//Embolden if can kill
        {
            if (stats.personality == PersonalityType.Shy)
            {
                HandlePercentages(probabilities, "Attack", 50, isAbsolute: false, locks);
            }
            else
            {
                HandlePercentages(probabilities, "Attack", 80, isAbsolute: false, locks);
            }
        }

        if(distanceToPlayer == 1)//flat buff if in melee
        {
            HandlePercentages(probabilities, "Attack", 50, isAbsolute: false, locks);
        }

        
    }


    private IEnumerator MoveStep()
    {

        if (this == null || gameObject == null) yield break;

        if (target == null) yield break;

        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);
        Vector2Int playerPos = GridUtility.WorldToGridPosition(target.transform.position);

        List<Vector2Int> path = PathfindingUtility.GetPath(myPos, playerPos);

        if (path != null && path.Count > 0)
        {
            Vector2Int nextStep = path[0];

            // 1) Don't step onto the player
            if (nextStep == playerPos)
            {
                yield return new WaitForSeconds(delay);
                yield break;
            }

            // 2) Don't step onto another enemy
            Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            foreach (var other in allEnemies)
            {
                if (other == this) continue;
                Vector2Int otherPos = GridUtility.WorldToGridPosition(other.transform.position);
                if (nextStep == otherPos)
                {
                    // blocked by another enemy
                    yield return new WaitForSeconds(delay);
                    yield break;
                }
            }

            // 3) If all clear, move
            yield return StepTo(nextStep);
        }
        else
        {
            // no path�just wait
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator AttackPlayer()
    {
        if (this == null || gameObject == null) yield break;

        if (CheckInRange(stats.attackRange))
        {

            IEnemyAttackBase enemyAttack = GetComponent<IEnemyAttackBase>();

            Stats playerStats = target.GetComponent<Stats>();

            enemyAttack.Attack(playerStats, stats);
            yield return new WaitForSeconds(delay);
        }
        else
        {
            yield return MoveStep();
        }

      
    }



   

    public static void HandlePercentages(
    Dictionary<string, float> probs,
    string key,
    float amount,
    bool isAbsolute,
    Dictionary<string, bool> locks
)
    {
        if (!probs.ContainsKey(key))
            throw new ArgumentException($"Key '{key}' not found in probabilities.");

       
        if (locks.TryGetValue(key, out bool locked) && locked)
            return;

        float oldValue = probs[key];
        float newValue = isAbsolute ? amount : oldValue + amount;
        newValue = Mathf.Clamp(newValue, 0f, 100f);
        probs[key] = newValue;


        float sumLocked = 0f;
        float sumUnlockedOthers = 0f;
        foreach (var kv in probs)
        {
            if (kv.Key == key) continue;
            if (locks.TryGetValue(kv.Key, out bool isLocked) && isLocked)
                sumLocked += kv.Value;
            else
                sumUnlockedOthers += kv.Value;
        }

      
        float remainder = 100f - newValue - sumLocked;


        if (sumUnlockedOthers <= 0f)
            return;

       
        foreach (var k in new List<string>(probs.Keys))
        {
            if (k == key)
                continue;

            if (locks.TryGetValue(k, out bool isLocked2) && isLocked2)
                continue;

            float oldOther = probs[k];
            float share = oldOther / sumUnlockedOthers;
            probs[k] = Mathf.Clamp(remainder * share, 0f, 100f);
        }
    }


    private IEnumerator StepTo(Vector2Int position)
    {
        if (!this || !this.gameObject) yield break;

        Vector3 targetPos = GridUtility.GridToWorldPosition(position);
        float duration = 0.1f;
        float elapsed = 0f;

        // You must guard access to `transform` after each yield
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            if (this == null || gameObject == null) yield break;

            // Avoid accessing transform directly after a yield
            transform.position = Vector3.Lerp(start, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (this == null || gameObject == null) yield break;
        transform.position = targetPos;

        CheckForMine();
    }

    private void CheckForMine()
    {
        // 1) Get the enemy's current grid tile
        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);

        // 2) Find all Mine instances in the scene
        GameObject[] allMines = GameObject.FindGameObjectsWithTag("Mine");
        foreach (var mine in allMines)
        {
            
            Vector2Int minePos = GridUtility.WorldToGridPosition(mine.transform.position);

            if (minePos == myPos)
            {
                
                Stats myStats = GetComponent<Stats>();
                Stats mineStats = mine.GetComponent<Stats>();
                if (myStats != null)
                    myStats.TakeDamage(mineStats.damage);

               
                Destroy(mine.gameObject);

               
                break;
            }
        }
    }

    public bool CheckInRange(int range)
    {
        if (target == null) return false;

        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);
        Vector2Int playerPos = GridUtility.WorldToGridPosition(target.transform.position);

        int distance = PathfindingUtility.GetPathLength(myPos, playerPos);
        return distance <= range;
    }



    public IEnumerator RandomMoveStep()
    {

       

        if (this == null || gameObject == null) yield break;
        if (gameObject == null)
        {
            Debug.Log("Null reference to self");
            yield return null;
        }
        
        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);

        // 2) Build a list of all 8 neighbors
        var neighbors = new List<Vector2Int>();
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                neighbors.Add(new Vector2Int(myPos.x + dx, myPos.y + dy));
            }

        // 3) Shuffle the list
        for (int i = 0; i < neighbors.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, neighbors.Count);
            var tmp = neighbors[i];
            neighbors[i] = neighbors[j];
            neighbors[j] = tmp;
        }

        // 4) Try each until a valid step is found
        var grid = dungeonContainer.dungeon;
        int w = grid.GetLength(0), h = grid.GetLength(1);
        foreach (var dest in neighbors)
        {
            // Bounds & floor check
            if (dest.x < 0 || dest.x >= w || dest.y < 0 || dest.y >= h) continue;
            if (grid[dest.x, dest.y] == TileType.Wall) continue;

            // Don�t step on player
            var playerPos = GridUtility.WorldToGridPosition(target.transform.position);
            if (dest == playerPos) continue;

            // Don�t step on other enemies
            bool occupied = false;
            foreach (var other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                if (other == this) continue;
                if (GridUtility.WorldToGridPosition(other.transform.position) == dest)
                { occupied = true; break; }
            }
            if (occupied) continue;

            // Valid! step there and return
            yield return StepTo(dest);
            yield break;
        }

        // nowhere to go
        yield return new WaitForSeconds(delay);
    }


    public IEnumerator RandomMoveFarStep()
    {

        if (this == null || gameObject == null) yield break;

        // 1) Get my current grid pos
        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);

        // 2) Build a list of all 8 neighbors
        var neighbors = new List<Vector2Int>();
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                neighbors.Add(new Vector2Int(myPos.x + dx, myPos.y + dy));
            }

        // 3) Shuffle the list
        for (int i = 0; i < neighbors.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, neighbors.Count);
            var tmp = neighbors[i];
            neighbors[i] = neighbors[j];
            neighbors[j] = tmp;
        }

        // 4) Try each until a valid step is found
        var grid = dungeonContainer.dungeon;
        int w = grid.GetLength(0), h = grid.GetLength(1);
        foreach (var dest in neighbors)
        {
            // Bounds & floor check
            if (dest.x < 0 || dest.x >= w || dest.y < 0 || dest.y >= h) continue;
            if (grid[dest.x, dest.y] == TileType.Wall) continue;

            // Don�t step on player
            var playerPos = GridUtility.WorldToGridPosition(target.transform.position);
            if (dest == playerPos) continue;

            // Don�t step on other enemies
            bool occupied = false;
            foreach (var other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                if (other == this) continue;
                if (GridUtility.WorldToGridPosition(other.transform.position) == dest)
                { occupied = true; break; }
            }
            if (occupied) continue;

            // Valid! step there and return
            yield return InstantStepTo(dest);
            yield break;
        }

        // nowhere to go
        yield return new WaitForSeconds(delay);
    }


    /// <summary>
    /// Steps exactly one tile directly away from the player (8?way).
    /// </summary>
    public IEnumerator RetreatStep()
        
    {
        if (this == null || gameObject == null) yield break;
        if (target == null) { yield return new WaitForSeconds(delay); yield break; }

        Vector2Int myPos = GridUtility.WorldToGridPosition(transform.position);
        Vector2Int playerPos = GridUtility.WorldToGridPosition(target.transform.position);

        // Compute direction away from player in each axis
        int dx = myPos.x - playerPos.x;
        int dy = myPos.y - playerPos.y;
        Vector2Int dir = new Vector2Int(Mathf.Clamp(dx, -1, 1),
                                        Mathf.Clamp(dy, -1, 1));

        if (dir == Vector2Int.zero)
        {
            // On top of player? just wait
            yield return new WaitForSeconds(delay);
            yield break;
        }

        Vector2Int dest = myPos + dir;

        // Bounds & walkability
        var grid = dungeonContainer.dungeon;
        int w = grid.GetLength(0), h = grid.GetLength(1);
        if (dest.x < 0 || dest.x >= w || dest.y < 0 || dest.y >= h
            || grid[dest.x, dest.y] == TileType.Wall)
        {
            yield return new WaitForSeconds(delay);
            yield break;
        }

        // Occupied by other enemy?
        foreach (var other in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (other == this) continue;
            if (GridUtility.WorldToGridPosition(other.transform.position) == dest)
            {
                yield return new WaitForSeconds(delay);
                yield break;
            }
        }

        // Finally, step away
        yield return StepTo(dest);
    }


    private IEnumerator InstantStepTo(Vector2Int gridPos)
    {
        if (this == null || gameObject == null) yield break;
        Vector3 worldPos = GridUtility.GridToWorldPosition(gridPos);
        transform.position = worldPos;
        CheckForMine();

        yield return new WaitForSeconds(delay);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

}
