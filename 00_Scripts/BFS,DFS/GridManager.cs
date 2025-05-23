using UnityEngine;

// DFS (Depth-First Search)��? == ���� �켱 Ž��
/// <summary>
/// � ���(����)���� �����ؼ� �ִ��� ���� ������ ����, �� �̻� �� ���� ���� ��
/// �� �ܰ� ���� ���ư��� �ٸ� �������� �������� ����� Ž��
/// LIFO (Last in First Out) - Stack (���Լ���)
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
