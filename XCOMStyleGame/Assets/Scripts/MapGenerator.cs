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

    private List<Cell> availableCells = new List<Cell>();

    public void GenerateMap(MissionType missionType)
    {
        ClearExistingMap();
        InitializeGrid();
        GenerateTerrain();
        PlaceCoverAndObstacles();
        PlaceDoodads();

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

    private void ClearExistingMap()
    {
        // Destroy all existing map objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitializeGrid()
    {
        gridSystem.CreateGrid();
        availableCells = gridSystem.GetAllCells().ToList();
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                Cell cell = gridSystem.GetCellAtPosition(new Vector3(x, 0, y));
                if (cell != null)
                {
                    GameObject terrainPrefab = terrainPrefabs[Random.Range(0, terrainPrefabs.Length)];
                    GameObject terrain = Instantiate(terrainPrefab, cell.WorldPosition, Quaternion.identity, transform);
                    terrain.name = $"Terrain_{x}_{y}";
                }
            }
        }
    }

    private void PlaceCoverAndObstacles()
    {
        int coverCount = Mathf.FloorToInt(availableCells.Count * 0.2f);
        int obstacleCount = Mathf.FloorToInt(availableCells.Count * 0.1f);

        PlaceObjects(coverPrefabs, coverCount, "Cover");
        PlaceObjects(obstaclePrefabs, obstacleCount, "Obstacle");
    }

    private void PlaceDoodads()
    {
        int doodadCount = Mathf.FloorToInt(availableCells.Count * 0.05f);
        PlaceObjects(doodadPrefabs, doodadCount, "Doodad");
    }

    private void PlaceObjects(GameObject[] prefabs, int count, string namePrefix)
    {
        for (int i = 0; i < count; i++)
        {
            if (availableCells.Count == 0) break;

            int randomIndex = Random.Range(0, availableCells.Count);
            Cell cell = availableCells[randomIndex];
            availableCells.RemoveAt(randomIndex);

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject obj = Instantiate(prefab, cell.WorldPosition, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0), transform);
            obj.name = $"{namePrefix}_{i}";

            if (namePrefix == "Cover" || namePrefix == "Obstacle")
            {
                cell.CoverType = (namePrefix == "Cover") ? CoverType.Half : CoverType.Full;
            }
        }
    }

    private void GenerateEliminationMap()
    {
        // Add specific elements for elimination missions
        // e.g., enemy spawn points, strategic positions
    }

    private void GenerateExtractionMap()
    {
        // Add specific elements for extraction missions
        // e.g., extraction zone, hazards
    }

    private void GenerateVIPRescueMap()
    {
        // Add specific elements for VIP rescue missions
        // e.g., VIP location, enemy patrols
    }

    private void GenerateHackTerminalMap()
    {
        // Add specific elements for hack terminal missions
        // e.g., terminal location, defensive positions
    }

    private void GenerateDefendPositionMap()
    {
        // Add specific elements for defend position missions
        // e.g., defense zone, enemy approach paths
    }

    private void GenerateSabotageMap()
    {
        // Add specific elements for sabotage missions
        // e.g., sabotage targets, security measures
    }

    private void GenerateIntelGatheringMap()
    {
        // Add specific elements for intel gathering missions
        // e.g., intel locations, patrol routes
    }

    private void GenerateBossEncounterMap()
    {
        // Add specific elements for boss encounter missions
        // e.g., boss arena, minion spawn points
    }

    public Cell GetExtractionPoint()
    {
        return GetRandomAvailableCell();
    }

    public Cell GetTerminalLocation()
    {
        return GetRandomAvailableCell();
    }

    public Cell GetDefensePosition()
    {
        return GetRandomAvailableCell();
    }

    public List<Cell> GetSabotageTargets()
    {
        int targetCount = Random.Range(2, 5);
        return GetRandomAvailableCells(targetCount);
    }

    public List<Cell> GetIntelLocations()
    {
        int locationCount = Random.Range(3, 6);
        return GetRandomAvailableCells(locationCount);
    }

    public Cell GetVIPSpawnLocation()
    {
        return GetRandomAvailableCell();
    }

    public Cell GetBossSpawnLocation()
    {
        return GetRandomAvailableCell();
    }

    private Cell GetRandomAvailableCell()
    {
        if (availableCells.Count == 0) return null;
        int randomIndex = Random.Range(0, availableCells.Count);
        Cell cell = availableCells[randomIndex];
        availableCells.RemoveAt(randomIndex);
        return cell;
    }

    private List<Cell> GetRandomAvailableCells(int count)
    {
        List<Cell> cells = new List<Cell>();
        for (int i = 0; i < count; i++)
        {
            Cell cell = GetRandomAvailableCell();
            if (cell != null)
            {
                cells.Add(cell);
            }
        }
        return cells;
    }
}