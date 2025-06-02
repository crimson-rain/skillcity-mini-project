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
    
    // Singleton Creation
    private void Awake()
    {
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
        // Update spawners
        EnemySpawner[] es = dungeonGeneration.GetComponents<EnemySpawner>();

        foreach(var enemy in es)
        {
            enemy.IncreaseTurnCounter();
        }
         
        // Process indvidual enemy action
        Enemy[] enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (var e in enemies)
        {
            if (e == null)
                continue; 
            yield return e.TakeTurn();
        }
    }

}
