using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public TurnManager turnManager;
    public CombatManager combatManager;
    public SceneSetup sceneSetup;

    public GameObject unitPrefab;
    public int numUnits = 4;

    private Unit selectedUnit;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        InitializeGame();
    }

    void InitializeGame()
    {
        if (gridSystem == null) gridSystem = GetComponent<GridSystem>();
        if (turnManager == null) turnManager = GetComponent<TurnManager>();
        if (combatManager == null) combatManager = GetComponent<CombatManager>();
        if (sceneSetup == null) sceneSetup = GetComponent<SceneSetup>();

        SpawnUnits();
    }

    void SpawnUnits()
    {
        List<Cell> emptyCells = new List<Cell>();
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                Cell cell = gridSystem.GetCellAtPosition(new Vector3(x, 0, y));
                if (cell != null && !cell.IsOccupied)
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

            GameObject unitObj = sceneSetup.CreateUnitVisual(spawnCell.WorldPosition);
            Unit unit = unitObj.AddComponent<Unit>();
            unit.SetPosition(spawnCell);

            turnManager.AddUnit(unit);
        }
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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

        if (unitOnCell != null)
        {
            SelectUnit(unitOnCell);
        }
        else if (selectedUnit != null)
        {
            MoveSelectedUnit(cell);
        }
    }

    Unit FindUnitOnCell(Cell cell)
    {
        foreach (Unit unit in turnManager.units)
        {
            if (gridSystem.GetCellAtPosition(unit.transform.position) == cell)
            {
                return unit;
            }
        }
        return null;
    }

    void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        Debug.Log($"Selected unit at {unit.transform.position}");
    }

    void MoveSelectedUnit(Cell targetCell)
    {
        if (selectedUnit.CanMoveTo(targetCell))
        {
            selectedUnit.SetPosition(targetCell);
            Debug.Log($"Moved unit to {targetCell.WorldPosition}");
        }
        else
        {
            Debug.Log("Cannot move to that cell");
        }
    }
}