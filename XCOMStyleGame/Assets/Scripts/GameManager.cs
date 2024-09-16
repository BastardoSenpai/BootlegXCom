using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameMode { Normal, Move, Attack, UseAbility, Inventory }

public class GameManager : MonoBehaviour
{
    public GridSystem gridSystem;
    public TurnManager turnManager;
    public CombatManager combatManager;
    public SceneSetup sceneSetup;
    public GameUI gameUI;
    public MissionManager missionManager;
    public EnemyAI enemyAI;
    public CampaignManager campaignManager;

    public GameObject[] unitPrefabs;
    public SoldierClass[] soldierClasses;
    public Weapon[] availableWeapons;
    public int numPlayerUnits = 4;
    public int numEnemyUnits = 4;

    private GameMode currentGameMode = GameMode.Normal;
    private Unit selectedUnit;
    private Ability selectedAbility;

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
        if (campaignManager == null) campaignManager = CampaignManager.Instance;

        audioSource = gameObject.AddComponent<AudioSource>();

        InitializeGame();
    }

    void InitializeGame()
    {
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
        SpawnPlayerUnits();
        SpawnEnemyUnits();
    }

    void SpawnPlayerUnits()
    {
        List<Cell> emptyCells = gridSystem.GetAllCells().Where(c => !c.IsOccupied && c.TerrainType != TerrainType.Water).ToList();

        for (int i = 0; i < numPlayerUnits; i++)
        {
            if (emptyCells.Count == 0 || i >= campaignManager.currentCampaign.soldiers.Count) break;

            int randomIndex = Random.Range(0, emptyCells.Count);
            Cell spawnCell = emptyCells[randomIndex];
            emptyCells.RemoveAt(randomIndex);

            PersistentSoldier persistentSoldier = campaignManager.currentCampaign.soldiers[i];
            Unit unit = campaignManager.CreateUnitFromPersistentSoldier(persistentSoldier, unitPrefabs[0]);
            unit.SetPosition(spawnCell);

            turnManager.playerUnits.Add(unit);
        }
    }

    void SpawnEnemyUnits()
    {
        List<Cell> emptyCells = gridSystem.GetAllCells().Where(c => !c.IsOccupied && c.TerrainType != TerrainType.Water).ToList();

        for (int i = 0; i < numEnemyUnits; i++)
        {
            if (emptyCells.Count == 0) break;

            int randomIndex = Random.Range(0, emptyCells.Count);
            Cell spawnCell = emptyCells[randomIndex];
            emptyCells.RemoveAt(randomIndex);

            int unitTypeIndex = Random.Range(0, unitPrefabs.Length);
            GameObject unitObj = Instantiate(unitPrefabs[unitTypeIndex], spawnCell.WorldPosition, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();
            
            // Assign random soldier class and weapon
            unit.soldierClass = soldierClasses[Random.Range(0, soldierClasses.Length)];
            Weapon randomWeapon = availableWeapons[Random.Range(0, availableWeapons.Length)];
            unit.AddWeaponToInventory(randomWeapon);
            unit.EquipWeapon(randomWeapon);

            unit.SetPosition(spawnCell);

            turnManager.enemyUnits.Add(unit);
            unit.GetComponent<Renderer>().material.color = Color.red;
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
            case GameMode.UseAbility:
                if (selectedUnit != null && selectedAbility != null)
                {
                    if (unitOnCell != null)
                    {
                        selectedUnit.UseAbility(selectedAbility, unitOnCell);
                        SetGameMode(GameMode.Normal);
                    }
                    else
                    {
                        Debug.Log("No valid target for ability.");
                    }
                }
                break;
            case GameMode.Inventory:
                // Handle inventory actions
                break;
        }

        missionManager.CheckMissionObjectives();
        gameUI.UpdateMissionInfo(missionManager.GetMissionStatus());
        gameUI.UpdateSelectedUnitInfo(selectedUnit);
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

    public void UseAbility(Ability ability)
    {
        if (selectedUnit != null)
        {
            selectedAbility = ability;
            SetGameMode(GameMode.UseAbility);
        }
    }

    public void ImproveAbility(Ability ability)
    {
        if (selectedUnit != null)
        {
            CharacterProgression progression = selectedUnit.GetComponent<CharacterProgression>();
            progression.ImproveAbility(ability);
            gameUI.UpdateSelectedUnitInfo(selectedUnit);
        }
    }

    public void EndMission(bool isVictory)
    {
        foreach (Unit unit in turnManager.playerUnits)
        {
            campaignManager.UpdateSoldierAfterMission(unit);
        }

        if (isVictory)
        {
            campaignManager.currentCampaign.completedMissions++;
            campaignManager.currentCampaign.resources += 100; // Add resource reward
        }

        campaignManager.SaveCampaign();
        // Load the campaign menu or next mission scene
    }
}