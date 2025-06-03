
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveAction : GameAction
{
    private PlayerController player;
    private Vector2Int destination;

    public MoveAction(PlayerController player , Vector2Int dest)
    {
        this.player = player;
        destination = dest;
    }



    public override IEnumerator Execute()
    {
        // Check if the player reference is still valid
        if (player == null)
        {
            yield break;
        }

        var path = PathfindingUtility.GetPath(player.GridPosition, destination);

        foreach (var step in path)
        {
            if (!IsWalkable(step))
            {
                Debug.LogWarning($"Blocked by wall at: {step}");
                break;
            }

            if (player == null) yield break; // Check again before accessing components

            Fog fog = player.GetComponent<Fog>();
            if (fog != null)
            {
                fog.ExposeTiles();
            }

            yield return player.StepTo(step); // StepTo must also handle null internally

            // Check if TurnManager and player are still valid
            if (TurnManager.Instance != null && player != null)
            {
                yield return TurnManager.Instance.EnemyTurn();
            }
            else
            {
                yield break;
            }
        }
    }

    private bool IsWalkable(Vector2Int pos)
    {
        TileType[,] grid = player.dungeonGridContainer.dungeon;
        return grid[pos.x, pos.y] != TileType.Wall;
    }
    public void Halt()
    {
        
    }

}
