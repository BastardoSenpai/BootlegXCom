using UnityEngine;
using System.Collections.Generic;

public enum GameMode { Normal, Move, Attack }

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public TurnManager turnManager;
    public CombatManager combatManager;
    public SceneSetup sceneSetup;
    public GameUI gameUI;

    public GameObject[] unitPrefabs; // Array of different unit prefabs
    public int numPlayerUnits = 3;
    public int numEnemyUnits = 3;

    private GameMode currentGameMode = GameMode.Normal;
    private Unit selectedUnit;

    void Start()
    {
        if (gridSystem == null) gridSystem = GetComponent<GridSystem>();
        if (turnManager == null) turnManager = GetComponent<TurnManager>();
        if (combatManager == null) combatManager = GetComponent<CombatManager>();
        if (sceneSetup == null) sceneSetup = GetComponent<SceneSetup>();
        if (gameUI == null) gameUI = FindObjectOfType<GameUI>();

        GenerateMap();
        SpawnUnits();
    }

    void GenerateMap()
    {
        // Simple procedural generation
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                if (Random.value < 0.1f) // 10% chance for full cover
                {
                    gridSystem.SetCoverType(new Vector3(x, 0, y), CoverType.Full);
                    sceneSetup.CreateCoverVisual(new Vector3(x, 0, y), CoverType.Full);
                }
                else if (Random.value < 0.2f) // 20% chance for half cover
                {
                    gridSystem.SetCoverType(new Vector3(x, 0, y), CoverType.Half);
                    sceneSetup.CreateCoverVisual(new Vector3(x, 0, y), CoverType.Half);
                }
            }
        }
    }

    void SpawnUnits()
    {
        SpawnTeamUnits(numPlayerUnits, true);
        SpawnTeamUnits(numEnemyUnits, false);
    }

    void SpawnTeamUnits(int numUnits, bool isPlayerTeam)
    {
        List<Cell> emptyCells = new List<Cell>();
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                Cell cell = gridSystem.GetCellAtPosition(new Vector3(x, 0, y));
                if (cell != null && !cell.IsOccupied && cell.CoverType == CoverType.None)
                {
                    emptyCells.Add(cell);
                }
            }
        }

        for (int i = 0; i < numUnits; i++)
        {
            if (emptyCells.Count == 0) break;

            int randomIndex = Random.Range(0, emptyCells.Count);
            Cell spawnCell = emptyCells[randomIndex];
            emptyCells.RemoveAt(randomIndex);

            int unitTypeIndex = Random.Range(0, unitPrefabs.Length);
            GameObject unitObj = Instantiate(unitPrefabs[unitTypeIndex], spawnCell.WorldPosition, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();
            unit.SetPosition(spawnCell);

            if (isPlayerTeam)
            {
                turnManager.playerUnits.Add(unit);
                unit.GetComponent<Renderer>().material.color = Color.blue;
            }
            else
            {
                turnManager.enemyUnits.Add(unit);
                unit.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    void Update()
    {
        if (turnManager.IsPlayerTurn())
        {
            HandlePlayerInput();
        }
    }

    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Cell clickedCell = gridSystem.GetCellAtPosition(hit.point);
                if (clickedCell != null)
                {
                    HandleCellClick(clickedCell);
                }
            }
        }
    }

    void HandleCellClick(Cell cell)
    {
        Unit unitOnCell = FindUnitOnCell(cell);

        switch (currentGameMode)
        {
            case GameMode.Normal:
                if (unitOnCell != null && turnManager.playerUnits.Contains(unitOnCell))
                {
                    selectedUnit = unitOnCell;
                }
                break;
            case GameMode.Move:
                if (selectedUnit != null && selectedUnit.CanMoveTo(cell))
                {
                    selectedUnit.Move(cell);
                    SetGameMode(GameMode.Normal);
                }
                break;
            case GameMode.Attack:
                if (selectedUnit != null && unitOnCell != null && turnManager.enemyUnits.Contains(unitOnCell))
                {
                    combatManager.TryAttack(selectedUnit, unitOnCell);
                    SetGameMode(GameMode.Normal);
                }
                break;
        }
    }

    Unit FindUnitOnCell(Cell cell)
    {
        foreach (Unit unit in turnManager.playerUnits.Concat(turnManager.enemyUnits))
        {
            if (gridSystem.GetCellAtPosition(unit.transform.position) == cell)
            {
                return unit;
            }
        }
        return null;
    }

    public void SetGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        // Update UI or visual indicators based on the new game mode
    }
}