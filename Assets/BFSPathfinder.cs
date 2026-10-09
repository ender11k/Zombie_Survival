using System.Collections.Generic;
using UnityEngine;

public class BFSPathfinder : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public bool[,] grid;

    public Vector2Int originOffset = new Vector2Int(25, 25);

    private readonly Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1, 1),   // Top-Right
        new Vector2Int(1, -1),  // Bottom-Right
        new Vector2Int(-1, 1),  // Top-Left
        new Vector2Int(-1, -1)  // Bottom-Left
    };

    void Awake()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        grid = new bool[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = true;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x) + originOffset.x;
        int y = Mathf.RoundToInt(worldPos.y) + originOffset.y;
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        int x = gridPos.x - originOffset.x;
        int y = gridPos.y - originOffset.y;
        return new Vector3(x, y, 0);
    }

    public void SetTileWalkable(Vector3 worldPos, bool isWalkable)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        if (IsInBounds(gridPos))
        {
            grid[gridPos.x, gridPos.y] = isWalkable;
        }
    }

    public bool IsInBounds(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridWidth &&
               gridPos.y >= 0 && gridPos.y < gridHeight;
    }

    // Finds nearest walkable tile if position overlaps a wall
    public Vector2Int GetNearestWalkableTile(Vector2Int gridPos)
    {
        if (IsInBounds(gridPos) && grid[gridPos.x, gridPos.y])
        {
            return gridPos; // Position is already walkable
        }

        // Search adjacent 8 neighbor tiles for closest open tile
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = gridPos + dir;
            if (IsInBounds(neighbor) && grid[neighbor.x, neighbor.y])
            {
                return neighbor;
            }
        }

        return gridPos;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // Ensure both start and target resolve to open walkable tiles
        start = GetNearestWalkableTile(start);
        target = GetNearestWalkableTile(target);

        if (!IsInBounds(start) || !IsInBounds(target)) return path;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == target) break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsInBounds(next) && grid[next.x, next.y])
                {
                    // Prevent corner-cutting through walls
                    if (dir.x != 0 && dir.y != 0)
                    {
                        bool side1 = grid[current.x + dir.x, current.y];
                        bool side2 = grid[current.x, current.y + dir.y];
                        if (!side1 || !side2) continue;
                    }

                    if (!cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
        }

        if (!cameFrom.ContainsKey(target)) return path;

        Vector2Int curr = target;
        while (curr != start)
        {
            path.Add(curr);
            curr = cameFrom[curr];
        }
        path.Reverse();
        return path;
    }
}