using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    private GridSystem gridSystem;
    private CombatManager combatManager;
    private TurnManager turnManager;

    void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        combatManager = FindObjectOfType<CombatManager>();
        turnManager = FindObjectOfType<TurnManager>();
    }

    public void PerformTurn(Unit enemyUnit)
    {
        List<Unit> playerUnits = turnManager.playerUnits;
        Unit nearestTarget = FindNearestUnit(enemyUnit, playerUnits);

        if (nearestTarget == null)
        {
            EndTurn(enemyUnit);
            return;
        }

        if (enemyUnit.CanAttack(nearestTarget))
        {
            combatManager.TryAttack(enemyUnit, nearestTarget);
        }
        else
        {
            MoveTowardsTarget(enemyUnit, nearestTarget);
        }

        if (enemyUnit.HasRemainingActions())
        {
            PerformTurn(enemyUnit);
        }
        else
        {
            EndTurn(enemyUnit);
        }
    }

    private Unit FindNearestUnit(Unit fromUnit, List<Unit> unitList)
    {
        return unitList
            .OrderBy(u => Vector3.Distance(fromUnit.transform.position, u.transform.position))
            .FirstOrDefault();
    }

    private void MoveTowardsTarget(Unit enemyUnit, Unit target)
    {
        List<Cell> path = FindPath(enemyUnit, target);
        if (path.Count > 0)
        {
            int movementRange = Mathf.Min(enemyUnit.movementRange, path.Count - 1);
            Cell destinationCell = path[movementRange];
            enemyUnit.Move(destinationCell);
        }
    }

    private List<Cell> FindPath(Unit start, Unit goal)
    {
        // Implement A* pathfinding algorithm here
        // For simplicity, we'll use a basic approach
        Cell startCell = gridSystem.GetCellAtPosition(start.transform.position);
        Cell goalCell = gridSystem.GetCellAtPosition(goal.transform.position);

        List<Cell> path = new List<Cell>();
        Cell current = startCell;

        while (current != goalCell)
        {
            path.Add(current);
            List<Cell> neighbors = gridSystem.GetNeighbors(current);
            current = neighbors.OrderBy(c => Vector3.Distance(c.WorldPosition, goalCell.WorldPosition)).First();
        }

        return path;
    }

    private void EndTurn(Unit enemyUnit)
    {
        enemyUnit.EndTurn();
        turnManager.EndCurrentTurn();
    }
}