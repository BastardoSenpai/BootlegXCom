using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameMode { Normal, Move, Attack }

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public TurnManager turnManager;
    public CombatManager combatManager;
    public SceneSetup sceneSetup;
    public GameUI gameUI;
    public MissionManager missionManager;
    public EnemyAI enemyAI;

    public GameObject[] unitPrefabs;
    public int numPlayerUnits = 4;
    public int numEnemyUnits = 4;

    private GameMode currentGameMode = GameMode.Normal;
    private Unit selectedUnit;

    public AudioClip moveSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip missSound;

    private AudioSource audioSource;

    void Start()
    {
        if (gridSystem == null) gridSystem = GetComponent<GridSystem>();
        if (turnManager == null) turnManager = GetComponent<TurnManager>();
        if (combatManager == null) combatManager = GetComponent<CombatManager>();
        if (sceneSetup == null) sceneSetup = GetComponent<SceneSetup>();
        if (gameUI == null) gameUI = FindObjectOfType<GameUI>();
        if (missionManager == null) missionManager = GetComponent<MissionManager>();
        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();

        audioSource = gameObject.AddComponent<AudioSource>();

        GenerateMap();
        SpawnUnits();
        turnManager.OnTurnChange += HandleTurnChange;
    }

    void GenerateMap()
    {
        gridSystem.CreateGrid();
        sceneSetup.CreateGridVisual();
    }

    void SpawnUnits()
    {
        SpawnTeamUnits(numPlayerUnits, true);
        SpawnTeamUnits(numEnemyUnits, false);
    }

    void SpawnTeamUnits(int numUnits, bool isPlayerTeam)
    {
        List<Cell> emptyCells = gridSystem.GetAllCells().Where(c => !c.IsOccupied && c.TerrainType != TerrainType.Water).ToList();

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
                    gameUI.UpdateSelectedUnitInfo(selectedUnit);
                }
                break;
            case GameMode.Move:
                if (selectedUnit != null && selectedUnit.CanMoveTo(cell))
                {
                    selectedUnit.Move(cell);
                    PlaySound(moveSound);
                    SetGameMode(GameMode.Normal);
                }
                break;
            case GameMode.Attack:
                if (selectedUnit != null && unitOnCell != null && turnManager.enemyUnits.Contains(unitOnCell))
                {
                    bool hit = combatManager.TryAttack(selectedUnit, unitOnCell);
                    PlaySound(hit ? hitSound : missSound);
                    SetGameMode(GameMode.Normal);
                }
                break;
        }

        missionManager.CheckMissionObjectives();
        gameUI.UpdateMissionInfo(missionManager.GetMissionStatus());
    }

    Unit FindUnitOnCell(Cell cell)
    {
        return turnManager.playerUnits.Concat(turnManager.enemyUnits)
            .FirstOrDefault(u => gridSystem.GetCellAtPosition(u.transform.position) == cell);
    }

    public void SetGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        gameUI.UpdateGameMode(newMode);
    }

    void HandleTurnChange(Unit unit)
    {
        if (!turnManager.IsPlayerTurn())
        {
            StartEnemyTurn();
        }
    }

    void StartEnemyTurn()
    {
        foreach (Unit enemyUnit in turnManager.enemyUnits)
        {
            enemyAI.PerformTurn(enemyUnit);
        }
        turnManager.EndCurrentTurn();
    }

    public Cell GetRandomEmptyCell()
    {
        return gridSystem.GetAllCells()
            .Where(c => !c.IsOccupied && c.TerrainType != TerrainType.Water)
            .OrderBy(c => Random.value)
            .FirstOrDefault();
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}