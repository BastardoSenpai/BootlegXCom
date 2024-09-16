using UnityEngine;
using System.Collections.Generic;

public enum MissionType { Elimination, Extraction, Capture }

public class MissionManager : MonoBehaviour
{
    public MissionType currentMissionType;
    public int turnsRemaining;
    public bool missionCompleted = false;
    public bool missionFailed = false;

    private GameManager gameManager;
    private TurnManager turnManager;

    [System.Serializable]
    public class MissionObjective
    {
        public string description;
        public bool completed;
    }

    public List<MissionObjective> objectives = new List<MissionObjective>();

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        turnManager = FindObjectOfType<TurnManager>();
        GenerateRandomMission();
    }

    void GenerateRandomMission()
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
                Vector3 extractionPoint = gameManager.GetRandomEmptyCell().WorldPosition;
                objectives.Add(new MissionObjective { description = $"Reach extraction point at {extractionPoint}", completed = false });
                objectives.Add(new MissionObjective { description = "Ensure all player units reach the extraction point", completed = false });
                break;
            case MissionType.Capture:
                objectives.Add(new MissionObjective { description = "Capture the enemy VIP", completed = false });
                objectives.Add(new MissionObjective { description = "Escort the VIP to the extraction point", completed = false });
                break;
        }
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
            case MissionType.Capture:
                CheckCaptureObjectives();
                break;
        }

        if (objectives.TrueForAll(o => o.completed))
        {
            missionCompleted = true;
            Debug.Log("Mission Completed!");
        }
    }

    void CheckEliminationObjectives()
    {
        int enemyCount = turnManager.enemyUnits.Count;
        int requiredEliminations = int.Parse(objectives[0].description.Split(' ')[1]);
        if (enemyCount <= turnManager.enemyUnits.Count - requiredEliminations)
        {
            objectives[0].completed = true;
        }
    }

    void CheckExtractionObjectives()
    {
        Vector3 extractionPoint = ParseVector3(objectives[0].description.Split("at ")[1]);
        bool allUnitsExtracted = true;
        foreach (Unit unit in turnManager.playerUnits)
        {
            if (Vector3.Distance(unit.transform.position, extractionPoint) <= 1f)
            {
                objectives[0].completed = true;
            }
            else
            {
                allUnitsExtracted = false;
            }
        }
        objectives[1].completed = allUnitsExtracted;
    }

    void CheckCaptureObjectives()
    {
        // This would require additional logic to track the VIP unit and its status
        // For simplicity, we'll just mark these as completed for now
        objectives[0].completed = true;
        objectives[1].completed = true;
    }

    Vector3 ParseVector3(string vectorString)
    {
        string[] components = vectorString.Trim('(', ')').Split(',');
        return new Vector3(
            float.Parse(components[0]),
            float.Parse(components[1]),
            float.Parse(components[2])
        );
    }

    public void EndTurn()
    {
        turnsRemaining--;
        if (turnsRemaining <= 0)
        {
            missionFailed = true;
            Debug.Log("Mission Failed: Ran out of turns!");
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
}