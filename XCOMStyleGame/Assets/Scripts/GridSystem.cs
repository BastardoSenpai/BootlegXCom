using UnityEngine;
using System.Collections.Generic;

public enum CoverType { None, Half, Full }
public enum TerrainType { Normal, Rough, Water }

public class GridSystem : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public int depth = 3;
    public float cellSize = 1f;

    private Cell[,,] grid;

    void Awake()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Cell[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 worldPos = new Vector3(x * cellSize, z * cellSize, y * cellSize);
                    grid[x, y, z] = new Cell(worldPos, new Vector3Int(x, y, z));
                }
            }
        }

        GenerateTerrain();
        GenerateCover();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                if (perlinValue < 0.3f)
                {
                    grid[x, y, 0].TerrainType = TerrainType.Water;
                }
                else if (perlinValue < 0.7f)
                {
                    grid[x, y, 0].TerrainType = TerrainType.Normal;
                }
                else
                {
                    grid[x, y, 0].TerrainType = TerrainType.Rough;
                }
            }
        }
    }

    void GenerateCover()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y, 0].TerrainType != TerrainType.Water)
                {
                    float randomValue = Random.value;
                    if (randomValue < 0.1f)
                    {
                        grid[x, y, 0].CoverType = CoverType.Full;
                        SpawnDestructibleObject(new Vector3(x, 0, y), CoverType.Full);
                    }
                    else if (randomValue < 0.25f)
                    {
                        grid[x, y, 0].CoverType = CoverType.Half;
                        SpawnDestructibleObject(new Vector3(x, 0, y), CoverType.Half);
                    }
                }
            }
        }
    }

    void SpawnDestructibleObject(Vector3 position, CoverType coverType)
    {
        GameObject destructiblePrefab = coverType == CoverType.Full ? 
            Resources.Load<GameObject>("Prefabs/FullCover") : 
            Resources.Load<GameObject>("Prefabs/HalfCover");

        if (destructiblePrefab != null)
        {
            GameObject destructibleObject = Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleObject.AddComponent<DestructibleObject>().Initialize(this, GetCellAtPosition(position));
        }
        else
        {
            Debug.LogError($"Destructible prefab not found for {coverType} cover");
        }
    }

    public Cell GetCellAtPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.z / cellSize);
        int z = Mathf.FloorToInt(worldPosition.y / cellSize);

        if (x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth)
        {
            return grid[x, y, z];
        }

        return null;
    }

    public List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighborPos = cell.GridPosition + dir;
            if (neighborPos.x >= 0 && neighborPos.x < width &&
                neighborPos.y >= 0 && neighborPos.y < height &&
                neighborPos.z >= 0 && neighborPos.z < depth)
            {
                neighbors.Add(grid[neighborPos.x, neighborPos.y, neighborPos.z]);
            }
        }

        return neighbors;
    }

    public CoverType GetCoverTypeInDirection(Cell cell, Vector3 direction)
    {
        Vector3Int gridDirection = new Vector3Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.y),
            Mathf.RoundToInt(direction.z)
        );

        Vector3Int neighborPos = cell.GridPosition + gridDirection;
        if (neighborPos.x >= 0 && neighborPos.x < width &&
            neighborPos.y >= 0 && neighborPos.y < height &&
            neighborPos.z >= 0 && neighborPos.z < depth)
        {
            return grid[neighborPos.x, neighborPos.y, neighborPos.z].CoverType;
        }

        return CoverType.None;
    }

    public List<Cell> GetAllCells()
    {
        List<Cell> allCells = new List<Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    allCells.Add(grid[x, y, z]);
                }
            }
        }
        return allCells;
    }

    public List<Cell> GetCellsInRange(Vector3 position, int range)
    {
        List<Cell> cellsInRange = new List<Cell>();
        Cell centerCell = GetCellAtPosition(position);

        if (centerCell != null)
        {
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        Vector3Int gridPos = centerCell.GridPosition + new Vector3Int(x, y, z);
                        if (gridPos.x >= 0 && gridPos.x < width &&
                            gridPos.y >= 0 && gridPos.y < height &&
                            gridPos.z >= 0 && gridPos.z < depth)
                        {
                            cellsInRange.Add(grid[gridPos.x, gridPos.y, gridPos.z]);
                        }
                    }
                }
            }
        }

        return cellsInRange;
    }

    public void DestroyObjectAtCell(Cell cell)
    {
        cell.CoverType = CoverType.None;
        // You might want to update visuals or trigger other effects here
    }
}

public class Cell
{
    public Vector3 WorldPosition { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    public bool IsOccupied { get; set; }
    public CoverType CoverType { get; set; }
    public TerrainType TerrainType { get; set; }

    public Cell(Vector3 worldPos, Vector3Int gridPos)
    {
        WorldPosition = worldPos;
        GridPosition = gridPos;
        IsOccupied = false;
        CoverType = CoverType.None;
        TerrainType = TerrainType.Normal;
    }
}