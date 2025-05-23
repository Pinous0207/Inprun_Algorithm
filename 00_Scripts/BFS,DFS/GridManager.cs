using UnityEngine;

// DFS (Depth-First Search)란? == 깊이 우선 탐색
/// <summary>
/// 어떤 노드(지점)에서 시작해서 최대한 깊이 내려간 다음, 더 이상 갈 곳이 없을 때
/// 한 단계 위로 돌아가서 다른 방향으로 내려가는 방식의 탐색
/// LIFO (Last in First Out) - Stack (후입선출)
/// </summary>
public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int[,] gridData = new int[,]
    {
        {0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
        {0, 1, 0, 1, 1, 0, 1, 0, 1, 0},
        {0, 0, 0, 1, 0, 0, 0, 0, 1, 0},
        {1, 1, 0, 1, 0, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
        {0, 1, 1, 1, 0, 1, 0, 1, 1, 1},
        {0, 1, 0, 0, 0, 0, 0, 1, 0, 0},
        {0, 1, 0, 1, 1, 1, 0, 1, 0, 1},
        {0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
        {1, 1, 0, 0, 0, 1, 1, 1, 1, 0}
    };

    private void Start()
    {
        GenerateGrid();
    }
    public Tile[,] tiles;

    void GenerateGrid()
    {
        int rows = gridData.GetLength(0);
        int cols = gridData.GetLength(1);
        tiles = new Tile[rows, cols];

        for(int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                var tileGo = Instantiate(tilePrefab, new Vector3(x, -y, 0), Quaternion.identity);
                var tile = tileGo.GetComponent<Tile>();
                tile.gridPosition = new Vector2Int(x, y);
                tile.SetColor(gridData[y, x] == 1 ? Color.gray : Color.white);
                tiles[y, x] = tile;
            }
        }
    }
}
