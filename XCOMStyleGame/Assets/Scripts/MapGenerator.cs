using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public GridSystem gridSystem;
    public GameObject[] terrainPrefabs;
    public GameObject[] coverPrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject[] doodadPrefabs;
    public GameObject[] environmentalObjectPrefabs;

    private List<Cell> availableCells = new List<Cell>();

    public void GenerateMap(MissionType missionType)
    {
        ClearExistingMap();
        InitializeGrid();
        GenerateTerrain();
        PlaceCoverAndObstacles();
        PlaceDoodads();
        PlaceEnvironmentalObjects();

        switch (missionType)
        {
            case MissionType.Elimination:
                GenerateEliminationMap();
                break;
            case MissionType.Extraction:
                GenerateExtractionMap();
                break;
            case MissionType.VIPRescue:
                GenerateVIPRescueMap();
                break;
            case MissionType.HackTerminal:
                GenerateHackTerminalMap();
                break;
            case MissionType.DefendPosition:
                GenerateDefendPositionMap();
                break;
            case MissionType.Sabotage:
                GenerateSabotageMap();
                break;
            case MissionType.IntelGathering:
                GenerateIntelGatheringMap();
                break;
            case MissionType.BossEncounter:
                GenerateBossEncounterMap();
                break;
        }
    }

    // ... (previous methods remain the same)

    private void PlaceEnvironmentalObjects()
    {
        int hazardCount = Mathf.FloorToInt(availableCells.Count * 0.05f);
        int interactiveCount = Mathf.FloorToInt(availableCells.Count * 0.03f);

        PlaceHazards(hazardCount);
        PlaceInteractiveObjects(interactiveCount);
    }

    private void PlaceHazards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (availableCells.Count == 0) break;

            int randomIndex = Random.Range(0, availableCells.Count);
            Cell cell = availableCells[randomIndex];
            availableCells.RemoveAt(randomIndex);

            GameObject hazardPrefab = GetRandomHazardPrefab();
            GameObject hazard = Instantiate(hazardPrefab, cell.WorldPosition, Quaternion.identity, transform);
            hazard.name = $"Hazard_{i}";

            EnvironmentalObject envObject = hazard.GetComponent<EnvironmentalObject>();
            if (envObject != null)
            {
                envObject.objectType = EnvironmentalObjectType.Hazard;
            }
        }
    }

    private void PlaceInteractiveObjects(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (availableCells.Count == 0) break;

            int randomIndex = Random.Range(0, availableCells.Count);
            Cell cell = availableCells[randomIndex];
            availableCells.RemoveAt(randomIndex);

            GameObject interactivePrefab = GetRandomInteractiveObjectPrefab();
            GameObject interactive = Instantiate(interactivePrefab, cell.WorldPosition, Quaternion.identity, transform);
            interactive.name = $"Interactive_{i}";

            EnvironmentalObject envObject = interactive.GetComponent<EnvironmentalObject>();
            if (envObject != null)
            {
                envObject.objectType = EnvironmentalObjectType.Interactive;
            }
        }
    }

    private GameObject GetRandomHazardPrefab()
    {
        return environmentalObjectPrefabs.FirstOrDefault(p => p.GetComponent<EnvironmentalObject>()?.objectType == EnvironmentalObjectType.Hazard);
    }

    private GameObject GetRandomInteractiveObjectPrefab()
    {
        return environmentalObjectPrefabs.FirstOrDefault(p => p.GetComponent<EnvironmentalObject>()?.objectType == EnvironmentalObjectType.Interactive);
    }

    private void GenerateEliminationMap()
    {
        // Place enemy units
        int enemyCount = Random.Range(5, 8);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void GenerateExtractionMap()
    {
        // Place extraction point
        PlaceExtractionPoint();

        // Place enemy units
        int enemyCount = Random.Range(4, 7);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void GenerateVIPRescueMap()
    {
        // Place VIP
        PlaceVIP();

        // Place enemy units
        int enemyCount = Random.Range(5, 8);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }

        // Place extraction point
        PlaceExtractionPoint();
    }

    private void GenerateHackTerminalMap()
    {
        // Place hack terminal
        PlaceHackTerminal();

        // Place enemy units
        int enemyCount = Random.Range(6, 9);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void GenerateDefendPositionMap()
    {
        // Place defense objective
        PlaceDefenseObjective();

        // Place enemy units
        int enemyCount = Random.Range(7, 10);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void GenerateSabotageMap()
    {
        // Place sabotage targets
        int targetCount = Random.Range(2, 4);
        for (int i = 0; i < targetCount; i++)
        {
            PlaceSabotageTarget();
        }

        // Place enemy units
        int enemyCount = Random.Range(5, 8);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void GenerateIntelGatheringMap()
    {
        // Place intel objects
        int intelCount = Random.Range(3, 5);
        for (int i = 0; i < intelCount; i++)
        {
            PlaceIntelObject();
        }

        // Place enemy units
        int enemyCount = Random.Range(4, 7);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void GenerateBossEncounterMap()
    {
        // Place boss enemy
        PlaceBossEnemy();

        // Place regular enemy units
        int enemyCount = Random.Range(3, 5);
        for (int i = 0; i < enemyCount; i++)
        {
            PlaceEnemyUnit();
        }
    }

    private void PlaceEnemyUnit()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate an enemy unit at the cell's position
        // This depends on how your enemy units are set up
        Debug.Log($"Placed enemy unit at {cell.GridPosition}");
    }

    private void PlaceExtractionPoint()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate an extraction point object at the cell's position
        Debug.Log($"Placed extraction point at {cell.GridPosition}");
    }

    private void PlaceVIP()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate a VIP object at the cell's position
        Debug.Log($"Placed VIP at {cell.GridPosition}");
    }

    private void PlaceHackTerminal()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate a hack terminal object at the cell's position
        Debug.Log($"Placed hack terminal at {cell.GridPosition}");
    }

    private void PlaceDefenseObjective()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate a defense objective object at the cell's position
        Debug.Log($"Placed defense objective at {cell.GridPosition}");
    }

    private void PlaceSabotageTarget()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate a sabotage target object at the cell's position
        Debug.Log($"Placed sabotage target at {cell.GridPosition}");
    }

    private void PlaceIntelObject()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate an intel object at the cell's position
        Debug.Log($"Placed intel object at {cell.GridPosition}");
    }

    private void PlaceBossEnemy()
    {
        if (availableCells.Count == 0) return;

        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);

        // Here you would instantiate a boss enemy at the cell's position
        Debug.Log($"Placed boss enemy at {cell.GridPosition}");
    }
}