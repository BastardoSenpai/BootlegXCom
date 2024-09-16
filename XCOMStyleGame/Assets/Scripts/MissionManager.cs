using UnityEngine;
using System.Collections.Generic;

public enum MissionType { Elimination, Extraction, VIPRescue, HackTerminal, DefendPosition }

public class MissionManager : MonoBehaviour
{
    public MissionType currentMissionType;
    public int turnsRemaining;
    public bool missionCompleted = false;
    public bool missionFailed = false;

    private GameManager gameManager;
    private TurnManager turnManager;
    private GridSystem gridSystem;

    [System.Serializable]
    public class MissionObjective
    {
        public string description;
        public bool completed;
    }

    public List<MissionObjective> objectives = new List<MissionObjective>();

    // Mission-specific variables
    private Unit vipUnit;
    private Cell extractionPoint;
    private Cell terminalLocation;
    private Cell defensePosition;
    private int hackProgress = 0;
    private int requiredHackTurns = 3;

    void Start()
    {
        gameManager = GameManager.Instance;
        turnManager = FindObjectOfType<TurnManager>();
        gridSystem = FindObjectOfType<GridSystem>();
    }

    public void GenerateRandomMission()
    {
        currentMissionType = (MissionType)Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length);
        turnsRemaining = Random.Range(10, 21);

        objectives.Clear();
        switch (currentMissionType)
        {
            case MissionType.Elimination:
                int enemyCount = Random.Range(3, 6);
                objectives.Add(new MissionObjective { description = $"Eliminate {enemyCount} enemy units", completed = false });
                break;
            case MissionType.Extraction:
                extractionPoint = gameManager.GetRandomEmptyCell();
                objectives.Add(new MissionObjective { description = $"Reach extraction point", completed = false });
                objectives.Add(new MissionObjective { description = "Ensure all player units reach the extraction point", completed = false });
                break;
            case MissionType.VIPRescue:
                SpawnVIP();
                objectives.Add(new MissionObjective { description = "Rescue the VIP", completed = false });
                objectives.Add(new MissionObjective { description = "Escort the VIP to the extraction point", completed = false });
                break;
            case MissionType.HackTerminal:
                terminalLocation = gameManager.GetRandomEmptyCell();
                objectives.Add(new MissionObjective { description = "Reach the terminal", completed = false });
                objectives.Add(new MissionObjective { description = "Hack the terminal", completed = false });
                objectives.Add(new MissionObjective { description = "Defend the position until hack is complete", completed = false });
                break;
            case MissionType.DefendPosition:
                defensePosition = gameManager.GetRandomEmptyCell();
                objectives.Add(new MissionObjective { description = "Reach the defense position", completed = false });
                objectives.Add(new MissionObjective { description = $"Defend the position for {turnsRemaining} turns", completed = false });
                break;
        }
    }

    void SpawnVIP()
    {
        Cell vipCell = gameManager.GetRandomEmptyCell();
        GameObject vipObject = new GameObject("VIP");
        vipUnit = vipObject.AddComponent<Unit>();
        vipUnit.unitName = "VIP";
        vipUnit.maxHealth = 50;
        vipUnit.currentHealth = 50;
        vipUnit.movementRange = 4;
        vipUnit.SetPosition(vipCell);
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
        }

        if (objectives.TrueForAll(o => o.completed))
        {
            missionCompleted = true;
            Debug.Log("Mission Completed!");
            gameManager.EndMission(true);
        }
    }

    // ... (keep the existing CheckXXXObjectives methods)

    public void EndTurn()
    {
        turnsRemaining--;
        if (turnsRemaining <= 0)
        {
            if (currentMissionType != MissionType.DefendPosition)
            {
                missionFailed = true;
                Debug.Log("Mission Failed: Ran out of turns!");
                gameManager.EndMission(false);
            }
            else
            {
                CheckMissionObjectives();
            }
        }
    }

    public string GetMissionStatus()
    {
        string status = $"Mission: {currentMissionType}\nTurns Remaining: {turnsRemaining}\n\nObjectives:\n";
        foreach (var objective in objectives)
        {
            status += $"- {objective.description}: {(objective.completed ? "Completed" : "Incomplete")}\n";
        }
        return status;
    }

    public void LoadFromSaveData(MissionSaveData saveData)
    {
        currentMissionType = saveData.missionType;
        turnsRemaining = saveData.turnsRemaining;
        objectives = saveData.objectives.Select(o => new MissionObjective { description = o.description, completed = o.completed }).ToList();

        // Restore mission-specific variables based on the mission type
        switch (currentMissionType)
        {
            case MissionType.Extraction:
            case MissionType.VIPRescue:
                extractionPoint = gridSystem.GetCellAtPosition(saveData.extractionPoint);
                break;
            case MissionType.HackTerminal:
                terminalLocation = gridSystem.GetCellAtPosition(saveData.terminalLocation);
                hackProgress = saveData.hackProgress;
                break;
            case MissionType.DefendPosition:
                defensePosition = gridSystem.GetCellAtPosition(saveData.defensePosition);
                break;
        }

        if (currentMissionType == MissionType.VIPRescue)
        {
            SpawnVIP();
            vipUnit.SetPosition(gridSystem.GetCellAtPosition(saveData.vipPosition));
        }
    }
}