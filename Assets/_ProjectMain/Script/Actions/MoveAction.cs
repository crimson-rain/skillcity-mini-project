
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
        
        var path = PathfindingUtility.GetPath(player.GridPosition, destination);

        foreach (var step in path)
        {
            if (!IsWalkable(step))
            {
                Debug.LogWarning($"Blocked by wall at: {step}");
                break;
            }

            Fog fog = player.GetComponent<Fog>();
            fog.ExposeTiles();
            // Player moves exactly one tile
            yield return player.StepTo(step);

            // Then all enemies take exactly one tile-step
            yield return TurnManager.Instance.EnemyTurn();
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
