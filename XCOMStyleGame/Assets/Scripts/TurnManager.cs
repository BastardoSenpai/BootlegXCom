using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Team { Player, Enemy }

public class TurnManager : MonoBehaviour
{
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    private Queue<Unit> turnOrder;
    private Unit currentUnit;
    public Team currentTeam = Team.Player;

    public delegate void TurnChangeHandler(Unit unit);
    public event TurnChangeHandler OnTurnChange;

    void Start()
    {
        InitializeTurnOrder();
    }

    void InitializeTurnOrder()
    {
        turnOrder = new Queue<Unit>(playerUnits.Concat(enemyUnits).OrderBy(x => Random.value));
        StartNextTurn();
    }

    public void StartNextTurn()
    {
        if (turnOrder.Count == 0)
        {
            InitializeTurnOrder();
            return;
        }

        currentUnit = turnOrder.Dequeue();
        currentUnit.StartTurn();
        currentTeam = playerUnits.Contains(currentUnit) ? Team.Player : Team.Enemy;

        OnTurnChange?.Invoke(currentUnit);

        if (currentTeam == Team.Enemy)
        {
            PerformAITurn();
        }
    }

    public void EndCurrentTurn()
    {
        currentUnit.EndTurn();
        StartNextTurn();
    }

    private void PerformAITurn()
    {
        // Simple AI logic - move towards nearest player unit and attack if possible
        Unit nearestPlayerUnit = FindNearestUnit(currentUnit, playerUnits);
        if (nearestPlayerUnit != null)
        {
            if (currentUnit.CanAttack(nearestPlayerUnit))
            {
                currentUnit.Attack(nearestPlayerUnit);
            }
            else if (currentUnit.CanMoveTo(GetComponent<GridSystem>().GetCellAtPosition(nearestPlayerUnit.transform.position)))
            {
                currentUnit.Move(GetComponent<GridSystem>().GetCellAtPosition(nearestPlayerUnit.transform.position));
            }
        }

        EndCurrentTurn();
    }

    private Unit FindNearestUnit(Unit fromUnit, List<Unit> unitList)
    {
        return unitList.OrderBy(u => Vector3.Distance(fromUnit.transform.position, u.transform.position)).FirstOrDefault();
    }

    public bool IsPlayerTurn()
    {
        return currentTeam == Team.Player;
    }

    public Unit GetCurrentUnit()
    {
        return currentUnit;
    }
}