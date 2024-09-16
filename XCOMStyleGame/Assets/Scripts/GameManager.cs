using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public enum GameMode { Normal, Move, Attack, UseAbility, Inventory }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GridSystem gridSystem;
    public TurnManager turnManager;
    public CombatManager combatManager;
    public SceneSetup sceneSetup;
    public GameUI gameUI;
    public MissionManager missionManager;
    public EnemyAI enemyAI;
    public CampaignManager campaignManager;
    public WorldMap worldMap;
    public BaseManager baseManager;

    public GameObject[] unitPrefabs;
    public SoldierClass[] soldierClasses;
    public Weapon[] availableWeapons;
    public Equipment[] availableEquipments;

    private GameMode currentGameMode = GameMode.Normal;
    private Unit selectedUnit;
    private Ability selectedAbility;

    public AudioClip moveSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip missSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MissionScene")
        {
            InitializeMissionScene();
        }
        else if (scene.name == "WorldMapScene")
        {
            InitializeWorldMapScene();
        }
    }

    void InitializeMissionScene()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        turnManager = FindObjectOfType<TurnManager>();
        combatManager = FindObjectOfType<CombatManager>();
        sceneSetup = FindObjectOfType<SceneSetup>();
        gameUI = FindObjectOfType<GameUI>();
        missionManager = FindObjectOfType<MissionManager>();
        enemyAI = FindObjectOfType<EnemyAI>();

        if (SaveSystem.MissionSaveExists())
        {
            LoadMission();
        }
        else
        {
            StartNewMission();
        }
    }

    void InitializeWorldMapScene()
    {
        worldMap = FindObjectOfType<WorldMap>();
        baseManager = FindObjectOfType<BaseManager>();
        // Initialize world map and base management UI
    }

    void StartNewMission()
    {
        missionManager.GenerateRandomMission();
        SpawnUnits();
        turnManager.StartFirstTurn();
    }

    void SpawnUnits()
    {
        SpawnPlayerUnits();
        SpawnEnemyUnits();
    }

    void SpawnPlayerUnits()
    {
        List<Cell> emptyCells = gridSystem.GetAllCells().Where(c => !c.IsOccupied && c.TerrainType != TerrainType.Water).ToList();

        foreach (var soldier in campaignManager.currentCampaign.soldiers)
        {
            if (emptyCells.Count == 0) break;

            int randomIndex = Random.Range(0, emptyCells.Count);
            Cell spawnCell = emptyCells[randomIndex];
            emptyCells.RemoveAt(randomIndex);

            GameObject unitObj = Instantiate(unitPrefabs[0], spawnCell.WorldPosition, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();
            unit.InitializeFromPersistentSoldier(soldier);
            unit.SetPosition(spawnCell);

            turnManager.playerUnits.Add(unit);
        }
    }

    void SpawnEnemyUnits()
    {
        // Similar to the previous implementation, but adjust based on mission difficulty
    }

    public void SetGameMode(GameMode newMode)
    {
        currentGameMode = newMode;
        gameUI.UpdateGameMode(newMode);
    }

    public void UseAbility(Ability ability)
    {
        if (selectedUnit != null)
        {
            selectedAbility = ability;
            SetGameMode(GameMode.UseAbility);
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

        SaveSystem.SaveCampaign(campaignManager.currentCampaign);
        SaveSystem.DeleteMissionSave();
        SceneManager.LoadScene("WorldMapScene");
    }

    public void SaveMission()
    {
        MissionSaveData missionData = new MissionSaveData(missionManager, turnManager, gridSystem);
        SaveSystem.SaveMission(missionData);
        Debug.Log("Mission saved successfully.");
    }

    public void LoadMission()
    {
        MissionSaveData missionData = SaveSystem.LoadMission();
        if (missionData != null)
        {
            missionManager.LoadFromSaveData(missionData);
            turnManager.LoadFromSaveData(missionData);
            gridSystem.LoadFromSaveData(missionData.gridData);
            Debug.Log("Mission loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load mission.");
            StartNewMission();
        }
    }

    void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name == "MissionScene")
        {
            SaveMission();
        }
    }
}