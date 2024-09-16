using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum MissionType
{
    Elimination,
    Extraction,
    VIPRescue,
    HackTerminal,
    DefendPosition,
    Sabotage,
    IntelGathering,
    BossEncounter,
    SupplyRaid,
    Infiltration,
    AssetRecovery,
    Escort
}

public class MissionManager : MonoBehaviour
{
    public MissionType currentMissionType;
    public int baseTurnsRemaining;
    public int turnsRemaining;
    public bool missionCompleted = false;
    public bool missionFailed = false;

    private GameManager gameManager;
    private TurnManager turnManager;
    private GridSystem gridSystem;
    private MapGenerator mapGenerator;
    private DifficultyManager difficultyManager;

    [System.Serializable]
    public class MissionObjective
    {
        public string description;
        public bool completed;
        public int progress;
        public int requiredProgress;
    }

    public List<MissionObjective> objectives = new List<MissionObjective>();

    // Mission-specific variables
    private Unit vipUnit;
    private Cell extractionPoint;
    private Cell terminalLocation;
    private Cell defensePosition;
    private List<Cell> intelLocations = new List<Cell>();
    private Unit bossUnit;
    private List<Cell> supplyLocations = new List<Cell>();
    private Unit escortUnit;
    private List<Cell> assetLocations = new List<Cell>();

    void Start()
    {
        gameManager = GameManager.Instance;
        turnManager = FindObjectOfType<TurnManager>();
        gridSystem = FindObjectOfType<GridSystem>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        difficultyManager = DifficultyManager.Instance;
    }

    public void GenerateRandomMission()
    {
        currentMissionType = (MissionType)Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length);
        baseTurnsRemaining = Random.Range(10, 21);
        turnsRemaining = Mathf.RoundToInt(baseTurnsRemaining * difficultyManager.GetMissionTimeMultiplier());

        objectives.Clear();
        mapGenerator.GenerateMap(currentMissionType);

        switch (currentMissionType)
        {
            case MissionType.Elimination:
                GenerateEliminationMission();
                break;
            case MissionType.Extraction:
                GenerateExtractionMission();
                break;
            case MissionType.VIPRescue:
                GenerateVIPRescueMission();
                break;
            case MissionType.HackTerminal:
                GenerateHackTerminalMission();
                break;
            case MissionType.DefendPosition:
                GenerateDefendPositionMission();
                break;
            case MissionType.Sabotage:
                GenerateSabotageMission();
                break;
            case MissionType.IntelGathering:
                GenerateIntelGatheringMission();
                break;
            case MissionType.BossEncounter:
                GenerateBossEncounterMission();
                break;
            case MissionType.SupplyRaid:
                GenerateSupplyRaidMission();
                break;
            case MissionType.Infiltration:
                GenerateInfiltrationMission();
                break;
            case MissionType.AssetRecovery:
                GenerateAssetRecoveryMission();
                break;
            case MissionType.Escort:
                GenerateEscortMission();
                break;
        }
    }

    void GenerateEliminationMission()
    {
        int enemyCount = Random.Range(4, 7);
        objectives.Add(new MissionObjective { description = $"Eliminate all {enemyCount} enemy units", completed = false, progress = 0, requiredProgress = enemyCount });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateExtractionMission()
    {
        objectives.Add(new MissionObjective { description = "Reach the extraction point", completed = false });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateVIPRescueMission()
    {
        vipUnit = mapGenerator.SpawnVIP();
        objectives.Add(new MissionObjective { description = "Locate and secure the VIP", completed = false });
        objectives.Add(new MissionObjective { description = "Escort VIP to extraction point", completed = false });
        objectives.Add(new MissionObjective { description = "Extract all units including VIP", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateHackTerminalMission()
    {
        terminalLocation = mapGenerator.GetTerminalLocation();
        objectives.Add(new MissionObjective { description = "Reach the terminal", completed = false });
        objectives.Add(new MissionObjective { description = "Hack the terminal", completed = false, progress = 0, requiredProgress = 3 });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateDefendPositionMission()
    {
        defensePosition = mapGenerator.GetDefensePosition();
        int turnsToDefend = Random.Range(5, 8);
        objectives.Add(new MissionObjective { description = $"Defend the position for {turnsToDefend} turns", completed = false, progress = 0, requiredProgress = turnsToDefend });
        objectives.Add(new MissionObjective { description = "Eliminate all enemies", completed = false });
    }

    void GenerateSabotageMission()
    {
        List<Cell> sabotageTargets = mapGenerator.GetSabotageTargets();
        int targetsToSabotage = Mathf.Min(sabotageTargets.Count, Random.Range(2, 4));
        objectives.Add(new MissionObjective { description = $"Sabotage {targetsToSabotage} enemy targets", completed = false, progress = 0, requiredProgress = targetsToSabotage });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateIntelGatheringMission()
    {
        intelLocations = mapGenerator.GetIntelLocations();
        int intelToGather = Mathf.Min(intelLocations.Count, Random.Range(3, 6));
        objectives.Add(new MissionObjective { description = $"Gather {intelToGather} pieces of intel", completed = false, progress = 0, requiredProgress = intelToGather });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateBossEncounterMission()
    {
        bossUnit = mapGenerator.SpawnBossEnemy();
        objectives.Add(new MissionObjective { description = "Defeat the boss enemy", completed = false });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateSupplyRaidMission()
    {
        supplyLocations = mapGenerator.GetSupplyLocations();
        int requiredSupplies = Mathf.Min(supplyLocations.Count, Random.Range(3, 6));
        objectives.Add(new MissionObjective { description = $"Secure {requiredSupplies} supply caches", completed = false, progress = 0, requiredProgress = requiredSupplies });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateInfiltrationMission()
    {
        objectives.Add(new MissionObjective { description = "Infiltrate enemy base undetected", completed = false });
        objectives.Add(new MissionObjective { description = "Plant tracking device", completed = false });
        objectives.Add(new MissionObjective { description = "Exfiltrate without raising alarms", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateAssetRecoveryMission()
    {
        assetLocations = mapGenerator.GetAssetLocations();
        int requiredAssets = Mathf.Min(assetLocations.Count, Random.Range(2, 4));
        objectives.Add(new MissionObjective { description = $"Recover {requiredAssets} alien artifacts", completed = false, progress = 0, requiredProgress = requiredAssets });
        objectives.Add(new MissionObjective { description = "Extract all units with recovered assets", completed = false });
        extractionPoint = mapGenerator.GetExtractionPoint();
    }

    void GenerateEscortMission()
    {
        SpawnEscortUnit();
        extractionPoint = mapGenerator.GetExtractionPoint();
        objectives.Add(new MissionObjective { description = "Escort VIP to extraction point", completed = false });
        objectives.Add(new MissionObjective { description = "Ensure VIP survives", completed = false });
        objectives.Add(new MissionObjective { description = "Extract all units", completed = false });
    }

    void SpawnEscortUnit()
    {
        Cell escortCell = mapGenerator.GetVIPSpawnLocation();
        GameObject escortObject = new GameObject("EscortVIP");
        escortUnit = escortObject.AddComponent<Unit>();
        escortUnit.unitName = "VIP";
        escortUnit.maxHealth = 50;
        escortUnit.currentHealth = 50;
        escortUnit.movementRange = 4;
        escortUnit.SetPosition(escortCell);
    }

    public void CheckMissionObjectives()
    {
        switch (currentMissionType)
        {
            case MissionType.Elimination:
                CheckEliminationObjectives();
                break;
            case MissionType.Extraction:
                CheckExtractionObjectives();
                break;
            case MissionType.VIPRescue:
                CheckVIPRescueObjectives();
                break;
            case MissionType.HackTerminal:
                CheckHackTerminalObjectives();
                break;
            case MissionType.DefendPosition:
                CheckDefendPositionObjectives();
                break;
            case MissionType.Sabotage:
                CheckSabotageObjectives();
                break;
            case MissionType.IntelGathering:
                CheckIntelGatheringObjectives();
                break;
            case MissionType.BossEncounter:
                CheckBossEncounterObjectives();
                break;
            case MissionType.SupplyRaid:
                CheckSupplyRaidObjectives();
                break;
            case MissionType.Infiltration:
                CheckInfiltrationObjectives();
                break;
            case MissionType.AssetRecovery:
                CheckAssetRecoveryObjectives();
                break;
            case MissionType.Escort:
                CheckEscortObjectives();
                break;
        }

        if (objectives.TrueForAll(o => o.completed))
        {
            missionCompleted = true;
            Debug.Log("Mission Completed!");
            gameManager.EndMission(true);
        }
    }

    void CheckEliminationObjectives()
    {
        MissionObjective eliminateObjective = objectives[0];
        eliminateObjective.progress = turnManager.initialEnemyCount - turnManager.enemyUnits.Count;
        eliminateObjective.completed = eliminateObjective.progress >= eliminateObjective.requiredProgress;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted && eliminateObjective.completed;
    }

    void CheckExtractionObjectives()
    {
        bool anyUnitAtExtractionPoint = turnManager.playerUnits.Any(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[0].completed = anyUnitAtExtractionPoint;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted;
    }

    void CheckVIPRescueObjectives()
    {
        bool vipSecured = turnManager.playerUnits.Any(u => Vector3.Distance(u.transform.position, vipUnit.transform.position) <= 1f);
        objectives[0].completed = vipSecured;

        bool vipAtExtractionPoint = Vector3.Distance(vipUnit.transform.position, extractionPoint.WorldPosition) <= 1f;
        objectives[1].completed = vipAtExtractionPoint;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[2].completed = allUnitsExtracted && vipAtExtractionPoint;
    }

    void CheckHackTerminalObjectives()
    {
        bool unitAtTerminal = turnManager.playerUnits.Any(u => Vector3.Distance(u.transform.position, terminalLocation.WorldPosition) <= 1f);
        objectives[0].completed = unitAtTerminal;

        if (unitAtTerminal && objectives[1].progress < objectives[1].requiredProgress)
        {
            objectives[1].progress++;
        }
        objectives[1].completed = objectives[1].progress >= objectives[1].requiredProgress;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[2].completed = allUnitsExtracted && objectives[1].completed;
    }

    void CheckDefendPositionObjectives()
    {
        bool anyUnitAtDefensePosition = turnManager.playerUnits.Any(u => Vector3.Distance(u.transform.position, defensePosition.WorldPosition) <= 1f);
        if (anyUnitAtDefensePosition)
        {
            objectives[0].progress++;
        }
        objectives[0].completed = objectives[0].progress >= objectives[0].requiredProgress;

        objectives[1].completed = turnManager.enemyUnits.Count == 0;
    }

    void CheckSabotageObjectives()
    {
        MissionObjective sabotageObjective = objectives[0];
        sabotageObjective.progress = mapGenerator.GetSabotageTargets().Count(target => 
            turnManager.playerUnits.Exists(u => Vector3.Distance(u.transform.position, target.WorldPosition) <= 1f));
        sabotageObjective.completed = sabotageObjective.progress >= sabotageObjective.requiredProgress;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted && sabotageObjective.completed;
    }

    void CheckIntelGatheringObjectives()
    {
        MissionObjective gatherObjective = objectives[0];
        gatherObjective.progress = intelLocations.Count(location => 
            turnManager.playerUnits.Exists(u => Vector3.Distance(u.transform.position, location.WorldPosition) <= 1f));
        gatherObjective.completed = gatherObjective.progress >= gatherObjective.requiredProgress;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted && gatherObjective.completed;
    }

    void CheckBossEncounterObjectives()
    {
        objectives[0].completed = bossUnit == null || bossUnit.currentHealth <= 0;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted && objectives[0].completed;
    }

    void CheckSupplyRaidObjectives()
    {
        MissionObjective secureObjective = objectives[0];
        secureObjective.progress = supplyLocations.Count(location => 
            turnManager.playerUnits.Exists(u => Vector3.Distance(u.transform.position, location.WorldPosition) <= 1f));
        secureObjective.completed = secureObjective.progress >= secureObjective.requiredProgress;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted && secureObjective.completed;
    }

    void CheckInfiltrationObjectives()
    {
        // This would require more complex logic to track detection state
        // For simplicity, we'll just check if players reached certain points without engaging in combat
        bool reachedInfiltrationPoint = turnManager.playerUnits.Any(u => Vector3.Distance(u.transform.position, mapGenerator.GetInfiltrationPoint().WorldPosition) <= 1f);
        objectives[0].completed = reachedInfiltrationPoint;

        bool plantedDevice = turnManager.playerUnits.Any(u => Vector3.Distance(u.transform.position, mapGenerator.GetPlantingPoint().WorldPosition) <= 1f);
        objectives[1].completed = plantedDevice;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[2].completed = allUnitsExtracted && !turnManager.CombatOccurred;
    }

    void CheckAssetRecoveryObjectives()
    {
        MissionObjective recoverObjective = objectives[0];
        recoverObjective.progress = assetLocations.Count(location => 
            turnManager.playerUnits.Exists(u => Vector3.Distance(u.transform.position, location.WorldPosition) <= 1f));
        recoverObjective.completed = recoverObjective.progress >= recoverObjective.requiredProgress;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[1].completed = allUnitsExtracted && recoverObjective.completed;
    }

    void CheckEscortObjectives()
    {
        bool vipAtExtractionPoint = Vector3.Distance(escortUnit.transform.position, extractionPoint.WorldPosition) <= 1f;
        objectives[0].completed = vipAtExtractionPoint;

        objectives[1].completed = escortUnit.currentHealth > 0;

        bool allUnitsExtracted = turnManager.playerUnits.All(u => gridSystem.GetCellAtPosition(u.transform.position) == extractionPoint);
        objectives[2].completed = allUnitsExtracted && vipAtExtractionPoint;
    }
}