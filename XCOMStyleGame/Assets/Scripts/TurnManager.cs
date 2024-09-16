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

    private GameManager gameManager;
    private MissionManager missionManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        missionManager = FindObjectOfType<MissionManager>();
    }

    public void StartFirstTurn()
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
        missionManager.EndTurn();
        StartNextTurn();
    }

    private void PerformAITurn()
    {
        gameManager.enemyAI.PerformTurn(currentUnit);
        EndCurrentTurn();
    }

    public bool IsPlayerTurn()
    {
        return currentTeam == Team.Player;
    }

    public Unit GetCurrentUnit()
    {
        return currentUnit;
    }

    public void LoadFromSaveData(MissionSaveData saveData)
    {
        // Clear existing units
        playerUnits.Clear();
        enemyUnits.Clear();

        // Spawn and initialize player units
        foreach (var unitData in saveData.playerUnits)
        {
            GameObject unitObj = Instantiate(gameManager.unitPrefabs[0], unitData.position, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();
            unit.LoadFromSaveData(unitData);
            playerUnits.Add(unit);
        }

        // Spawn and initialize enemy units
        foreach (var unitData in saveData.enemyUnits)
        {
            GameObject unitObj = Instantiate(gameManager.unitPrefabs[1], unitData.position, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();
            unit.LoadFromSaveData(unitData);
            enemyUnits.Add(unit);
        }

        // Recreate the turn order
        turnOrder = new Queue<Unit>(playerUnits.Concat(enemyUnits).OrderBy(x => Random.value));

        // Set the current unit and team
        currentUnit = turnOrder.Peek();
        currentTeam = playerUnits.Contains(currentUnit) ? Team.Player : Team.Enemy;

        OnTurnChange?.Invoke(currentUnit);
    }
}