using UnityEngine;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
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
}

public class Cell
{
    public Vector3 WorldPosition { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    public bool IsOccupied { get; set; }

    public Cell(Vector3 worldPos, Vector3Int gridPos)
    {
        WorldPosition = worldPos;
        GridPosition = gridPos;
        IsOccupied = false;
    }
}