using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class GridMap : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    private Vector2Int gridSize;
    public float cellSize = 1f;

    public Node[,] grid;
    public List<Node> path; // Path for visualization

    void Start()
    {
        // Delay the grid initialization to allow the dungeon to generate first
        StartCoroutine(InitializeGridDelayed());
    }

    IEnumerator InitializeGridDelayed()
    {
        // Wait a frame to ensure the dungeon is generated
        yield return null;
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        FindGridSize();
        CreateGrid(gridSize.x, gridSize.y);
    }

    void FindGridSize()
    {
        BoundsInt floorBounds = floorTilemap.cellBounds;
        BoundsInt wallBounds = wallTilemap.cellBounds;

        // Combine bounds of both tilemaps to cover the entire area
        BoundsInt combinedBounds = floorBounds;
        combinedBounds.xMin = Mathf.Min(floorBounds.xMin, wallBounds.xMin);
        combinedBounds.yMin = Mathf.Min(floorBounds.yMin, wallBounds.yMin);
        combinedBounds.xMax = Mathf.Max(floorBounds.xMax, wallBounds.xMax);
        combinedBounds.yMax = Mathf.Max(floorBounds.yMax, wallBounds.yMax);

        gridSize = new Vector2Int(combinedBounds.size.x, combinedBounds.size.y);
    }

    void CreateGrid(int width, int height)
    {
        grid = new Node[width, height];
        BoundsInt combinedBounds = wallTilemap.cellBounds;
        combinedBounds.xMin = Mathf.Min(floorTilemap.cellBounds.xMin, wallTilemap.cellBounds.xMin);
        combinedBounds.yMin = Mathf.Min(floorTilemap.cellBounds.yMin, wallTilemap.cellBounds.yMin);
        combinedBounds.xMax = Mathf.Max(floorTilemap.cellBounds.xMax, wallTilemap.cellBounds.xMax);
        combinedBounds.yMax = Mathf.Max(floorTilemap.cellBounds.yMax, wallTilemap.cellBounds.yMax);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x + combinedBounds.xMin, y + combinedBounds.yMin, 0);

                if (!floorTilemap.HasTile(cellPosition) && !wallTilemap.HasTile(cellPosition))
                {
                    continue; // Skip this cell if it's not part of floor or wall
                }

                Vector3 worldPoint = wallTilemap.CellToWorld(cellPosition) + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
                bool walkable = !wallTilemap.HasTile(cellPosition) && floorTilemap.HasTile(cellPosition);
                grid[x, y] = new Node(walkable, new Vector2Int(x, y), worldPoint);
            }
        }
    }

    public void ReinitializeGrid()
    {
        InitializeGrid();
    }

    public void MarkPropsAsObstacles(List<Vector2Int> propPositions)
    {
        foreach (var pos in propPositions)
        {
            Vector3Int cellPosition = new Vector3Int(pos.x, pos.y, 0);
            if (IsWithinGrid(cellPosition))
            {
                int x = cellPosition.x - wallTilemap.cellBounds.xMin;
                int y = cellPosition.y - wallTilemap.cellBounds.yMin;
                if (grid != null && x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
                {
                    grid[x, y].walkable = false;
                }
            }
        }
    }

    private bool IsWithinGrid(Vector3Int cellPosition)
    {
        return cellPosition.x >= wallTilemap.cellBounds.xMin && cellPosition.x < wallTilemap.cellBounds.xMax &&
               cellPosition.y >= wallTilemap.cellBounds.yMin && cellPosition.y < wallTilemap.cellBounds.yMax;
    }

    public Node GetNode(Vector3 worldPosition)
    {
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPosition);

        // Check if the cell position is within the bounds of the grid
        if (cellPosition.x < wallTilemap.cellBounds.xMin ||
            cellPosition.x >= wallTilemap.cellBounds.xMax ||
            cellPosition.y < wallTilemap.cellBounds.yMin ||
            cellPosition.y >= wallTilemap.cellBounds.yMax)
        {
            Debug.LogWarning("World position is outside the bounds of the grid.");
            return null;
        }

        int x = cellPosition.x - wallTilemap.cellBounds.xMin;
        int y = cellPosition.y - wallTilemap.cellBounds.yMin;

        // Ensure that the grid array is not null and the indices are within its bounds
        if (grid != null && x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        else
        {
            Debug.LogWarning("Grid array is null or index is out of bounds.");
            return null;
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridPosition.x + x;
                int checkY = node.gridPosition.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n != null) // Only draw nodes that are not null
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (cellSize - 0.1f));
                }
            }
        }

        if (path != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
            }
        }
    }
}
