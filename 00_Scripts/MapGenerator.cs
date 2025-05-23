using UnityEditor.Tilemaps;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine.Rendering;

public class MapGenerator : MonoBehaviour
{
    int width;
    int height;
    public int targetCost;
    public float hexRadius;

    public GameObject[] TilePrefab;

    public S_Tile[,] tileMap;
    private S_Tile startTile;
    private S_Tile endTile;

    private void Awake()
    {
        AutoCalculateGridSize(targetCost);
        GenerateHexGrid();
        TryFindValidPath(targetCost);
    }

    void AutoCalculateGridSize(int targetCost)
    {
        width = (int)(targetCost * 1.5f);
        height = (int)targetCost;
    }

    void GenerateHexGrid()
    {
        tileMap = new S_Tile[width, height];

        float widthDistance = 2f * hexRadius * 0.75f;
        float heightDistance = Mathf.Sqrt(3f) * hexRadius;
        float gridWidth = (width - 1) * widthDistance;
        float gridHeight = (height - 1) * heightDistance;

        Vector3 centerOffset = new Vector3(gridWidth / 2, 0, gridHeight / 2);

        for(int x = 0; x < width; x++)
        {
            for(int y= 0; y < height; y++)
            {
                float posX = x * widthDistance;
                float posY = y * heightDistance;
                if (x % 2 == 1) posY += heightDistance / 2f;

                Vector3 position = new Vector3(posX, 0, posY) - centerOffset;

                var SelectedObject = TilePrefab[Random.Range(0, TilePrefab.Length)];
                GameObject tileObj = Instantiate(SelectedObject, position, Quaternion.identity, transform);
                S_Tile tile = tileObj.GetComponent<S_Tile>();

                tile.x = x;
                tile.y = y;
                tile.worldPosition = position;
                tileMap[x, y] = tile;
            }
        }
    }

    void TryFindValidPath(int targetCost)
    {
        int maxTries = 100;

        for(int attempt = 0; attempt < maxTries; attempt++)
        {
            startTile = GetRandomWalableTile();
            if(startTile == null)
            {
                Debug.LogError("No walkable start tile found");
                return;
            }

            bool success = FindPathWithEaxctCost(startTile, targetCost);
            if (success)
                return;
        }
    }

    bool FindPathWithEaxctCost(S_Tile start, int targetCost)
    {
        Queue<(S_Tile tile, int totalCost, List<S_Tile> path)> queue = new Queue<(S_Tile, int, List<S_Tile>)>();
        HashSet<S_Tile> visited = new HashSet<S_Tile>();

        queue.Enqueue((start, 0, new List<S_Tile> { start }));

        while(queue.Count > 0)
        {
            var (current, cost, path) = queue.Dequeue();

            if (visited.Contains(current))
                continue;
            visited.Add(current);

            if(cost == targetCost)
            {
                endTile = current;
                Debug.Log("타일 연결 성공 : " + endTile);
                return true;
            }

            foreach(var neighbor in GetNeighbors(current))
            {
                if (!neighbor.isWalkable || visited.Contains(neighbor))
                    continue;

                int newCost = cost + neighbor.moveCost;
                if (newCost > targetCost)
                    continue;

                var newPath = new List<S_Tile>(path) { neighbor };
                queue.Enqueue((neighbor, newCost, newPath));
            }
        }
        Debug.Log("타일 연결 실패");
        return false;
    }

    List<S_Tile> GetNeighbors(S_Tile tile)
    {
        List<S_Tile> neighbors = new List<S_Tile>();
        int x = tile.x;
        int y = tile.y;

        // 짝수 열 기준 오프셋
        int[,] evenColumnOffsets = new int[,]
        {
            {+1, 0 }, // 우
            {-1, 0 }, // 좌
            {0, -1 }, // 하
            {0, +1 }, // 상
            {-1, -1 }, // 좌하단
            {+1, -1 } // 우하단
        };
        // 홀수 열 기준 오프셋
        int[,] oddColumnOffsets = new int[,]
        {
            {+1, 0 }, // 우
            {-1, 0 }, // 좌
            {0, -1 }, // 하
            {0, +1 }, // 상
            {-1, +1 }, // 좌상단
            {+1, +1 } // 우상단
        };

        int[,] offsets = x % 2 == 0 ? evenColumnOffsets : oddColumnOffsets;

        for(int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = x + offsets[i, 0];
            int ny = y + offsets[i, 1];

            if(nx >= 0 && ny >= 0 && nx < width && ny < height)
            {
                neighbors.Add(tileMap[nx, ny]);
            }
        }

        return neighbors;
    }

    // 랜덤한 시작지점을 가지고 오기 위함
    S_Tile GetRandomWalableTile()
    {
        List<S_Tile> walableTiles = new List<S_Tile>();

        foreach(var tile in tileMap)
        {
            if (tile.isWalkable)
                walableTiles.Add(tile);
        }

        if (walableTiles.Count == 0)
            return null;

        return walableTiles[Random.Range(0, walableTiles.Count)];
    }

    public S_Tile GetStartTile()
    {
        return startTile;
    }

    public List<S_Tile> FindPath(S_Tile start, S_Tile goal)
    {
        Queue<(S_Tile tile, int cost, List<S_Tile> path)> queue = new();
        HashSet<S_Tile> visited = new();

        queue.Enqueue((start, 0, new List<S_Tile> { start }));

        while(queue.Count > 0)
        {
            var (current, cost, path) = queue.Dequeue();

            if (current == goal)
                return path;

            if (visited.Contains(current))
                continue;
            visited.Add(current);

            foreach(var neighbor in GetNeighbors(current))
            {
                if (!neighbor.isWalkable || visited.Contains(neighbor))
                    continue;

                var newPath = new List<S_Tile>(path) { neighbor };
                queue.Enqueue((neighbor, cost + neighbor.moveCost, newPath));
            }
        }
        return null;
    }

    public int GetPathCost(List<S_Tile> path)
    {
        if (path == null) return int.MaxValue;
        int total = 0;
        foreach (var tile in path)
            total += tile.moveCost;
        return total;
    }

    // Offset 좌표계 = (x,y)
    // X & 1 = x의 가장 마지막 비트(1의 자리수)
    // 1 -> 00000001 -> 1 -> 홀수
    // 4 -> 100 -> X & 1 -> 짝수
    private Vector2Int CubeToOffset(Vector3Int cube)
    {
        int x = cube.x;
        int z = cube.z + (x - (x & 1)) / 2;
        return new Vector2Int(x, z);
    }

    // Cube 좌표계 = (x,y,z)
    private Vector3Int OffsetToCube(int x, int y)
    {
        int cx = x;
        int cz = y - (x - (x & 1)) / 2;
        int cy = -cx - cz;
        return new Vector3Int(cx, cy, cz);
    }

    public List<S_Tile> GetTilesInRange(S_Tile center, int range)
    {
        List<S_Tile> result = new();
        Vector3Int centerCube = OffsetToCube(center.x, center.y);

        for(int dx = -range; dx <= range; dx++)
        {
            for(int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                int dz = -dx - dy;
                Vector3Int cube = new(centerCube.x + dx, centerCube.y + dy, centerCube.z + dz);
                Vector2Int offset = CubeToOffset(cube);

                int ox = offset.x;
                int oy = offset.y;

                if (ox >= 0 && oy >= 0 && ox < width && oy < height)
                {
                    result.Add(tileMap[ox, oy]);
                }
                else
                {
                    Debug.LogWarning($"{ox}, {oy} offset error");
                }
            }
        }
        return result;
    }
}
