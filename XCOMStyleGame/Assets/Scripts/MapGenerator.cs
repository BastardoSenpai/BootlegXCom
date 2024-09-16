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
        // Implement logic to select a random hazard prefab
        // This depends on how you've set up your hazard prefabs
        return environmentalObjectPrefabs.FirstOrDefault(p => p.GetComponent<EnvironmentalObject>()?.objectType == EnvironmentalObjectType.Hazard);
    }

    private GameObject GetRandomInteractiveObjectPrefab()
    {
        // Implement logic to select a random interactive object prefab
        // This depends on how you've set up your interactive object prefabs
        return environmentalObjectPrefabs.FirstOrDefault(p => p.GetComponent<EnvironmentalObject>()?.objectType == EnvironmentalObjectType.Interactive);
    }

    // ... (rest of the methods remain the same)
}