using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PathFinder : MonoBehaviour
{
    GridManager gridManager;
    private void Start()
    {
        gridManager = GetComponent<GridManager>();
    }

    public int dfsSearchCount;
    public int bfsSearchCount;
    public int aStarSearchCount;

    [SerializeField] private Text DFS_T, BFS_T, AStar_T;

    public Vector2Int start = new Vector2Int(0, 0);
    public Vector2Int end = new Vector2Int(9, 9);

    Vector2Int[] directions =
    {
        Vector2Int.down,
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right
    };

    #region A*
    public List<Vector2Int> GetAStarPath()
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int>();
        var fScore = new Dictionary<Vector2Int, int>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);
        // f = g + h

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        while(openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            aStarSearchCount++;
            if (current == end)
            {
                AStar_T.text = "A* : " + aStarSearchCount.ToString();
                return ReconstructPath(cameFrom, end);
            }
            closedSet.Add(current);

            foreach(var dir in directions)
            {
                var neighbor = current + dir;

                if (!IsValid(neighbor) || closedSet.Contains(neighbor))
                    continue;

                int tentativeG = gScore[current] + 1;

                if(!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, end);
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return null;
    }

    int Heuristic(Vector2Int  a, Vector2Int b)
    {
        // 맨해튼 거리
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    #endregion
    #region BFS
    public List<Vector2Int> GetBFSPath()
    {
        var visited = new HashSet<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            bfsSearchCount++;
            if (current == end)
            {
                BFS_T.text = "BFS : " + bfsSearchCount.ToString();
                return ReconstructPath(cameFrom, end);
            }
            foreach(var dir in directions)
            {
                var next = current + dir;

                if(IsValid(next) && !visited.Contains(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                    cameFrom[next] = current;
                }
            }
        }
        return null;
    }
    #endregion
    #region DFS
    public List<Vector2Int> GetDFSPath()
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        return DFS(start, end, visited);
    }

    List<Vector2Int> DFS(Vector2Int current, Vector2Int end, HashSet<Vector2Int> visited)
    {
        dfsSearchCount++;
        if (!IsValid(current) || visited.Contains(current)) return null;
        visited.Add(current);

        if (current == end)
            return new List<Vector2Int> { current };

        foreach(var dir in directions)
        {
            var next = current + dir;
            var path = DFS(next, end, visited);
            if(path != null)
            {
                path.Insert(0, current);
                return path;
            }
        }
        DFS_T.text = "DFS : " + dfsSearchCount.ToString();
        return null;
    }
    #endregion
    bool IsValid(Vector2Int pos)
    {
        var grid = gridManager.gridData;
        int w = grid.GetLength(1);
        int h = grid.GetLength(0);
        return pos.x >= 0 && pos.x < w && pos.y >= 0 && pos.y < h && grid[pos.y, pos.x] == 0;
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        var current = end;

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(current);
        path.Reverse();
        return path;
    }
}
