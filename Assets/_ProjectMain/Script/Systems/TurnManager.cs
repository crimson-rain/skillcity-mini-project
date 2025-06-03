/* TurnManger.cs - 
 * Responsible for handling all `GameAction` in a turn which are added to a queue and then processed.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private Queue<GameAction> actionQueue = new();
    private bool processing = false;
    public GameObject dungeonGeneration;
    public int FLoorNumber = 0;
    GameObject player;
    // Singleton Creation
    private void Awake()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        if (Instance != this) Destroy(gameObject);

    }

    // Add `GameAction` to the front of the queue.
    public void EnqueueActionFront(GameAction action)
    {
        actionQueue.Enqueue(action);

        if (!processing)
            StartCoroutine(ProcessGameActionQueue());
    }

    // Add `GameAction` to the back of the queue.
    public void EnqueueActionBack(GameAction action)
    {
        throw new System.NotImplementedException();
    }

    // Add `GameAction` to a specific index in the queue.
    public void InsertActionByIndex(GameAction action, uint index)
    {
        throw new System.NotImplementedException();
    }

    // Process `GameAction` in the queue by dequeuing and then executing them
    private IEnumerator ProcessGameActionQueue()
    {
        if (this == null || gameObject == null) yield break;
        processing = true;
        
        while (actionQueue.Count > 0)
        {
            yield return actionQueue.Dequeue().Execute();
        }

        processing = false;
    }

    // Process enemy turn
    public IEnumerator EnemyTurn()
    {
        if (this == null || gameObject == null) yield break;

        if(player == null) player = GameObject.FindGameObjectWithTag("Player"); 


        Stats playerStats = player.GetComponent<Stats>();

        if (playerStats.actionsTaken < playerStats.actionsPerTurn)
            playerStats.actionsTaken++;

        playerStats.actionsTaken = 0;

        if (dungeonGeneration == null) dungeonGeneration = GameObject.Find("Dungeon Generator");

        EnemySpawner[] spawners = dungeonGeneration.GetComponents<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            spawner.IncreaseTurnCounter();
        }

        // Get all enemies
        Enemy[] enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        int completedCount = 0;

        // Start each enemy's turn in parallel
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            StartCoroutine(EnemyTurnWrapper(enemy, () => completedCount++));
        }

        // Wait for all to complete
        while (completedCount < enemies.Length)
        {
            yield return null;
        }
    }

    public void HaltTurnManager()
    {
        StopAllCoroutines();
    }
    private IEnumerator EnemyTurnWrapper(Enemy enemy, System.Action onComplete)
    {
        yield return enemy.TakeTurn();
        onComplete?.Invoke();
    }
}
