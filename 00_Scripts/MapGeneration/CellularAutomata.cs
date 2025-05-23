using UnityEngine;

// Cellular Automata
// �� ���� �̿� ������ ���¿� ���� �ڽ��� ���¸� �����ϴ� �ý���
// ���� 2D ���ڿ��� �� ���� 1(�������) / 0(�׾�����) ���� ���� ���¸� ������
// �ݺ����� ��Ģ�� ���� ��ü ���� ��ȭ��Ŵ
// ���� 2D
public class CellularAutomata : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public int fillPercent = 45;
    public int smoothInterations = 5;
    private int[,] map;

    private void Start()
    {
        GenerateMap();
        for(int i = 0; i < smoothInterations; i++)
        {
            SmoothMap();
        }
        DrawTextureFromMap();
    }

    // �������� 0�� 1�� ä�� �־� �ʱ� ���� ���� ����
    // �����ڸ�(�׵θ�)�� ������ ��(1), ���δ� ���� Ȯ���� ��/����� ����
    void GenerateMap()
    {
        map = new int[width, height];

        // UnityEngine.Random.Range = float, int
        // System.Random = int, double
        System.Random rand = new System.Random();

        for(int y = 0;  y < height; y++)
        {
            for (int x= 0; x< width; x++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    map[x, y] = 1;
                else
                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        int[,] newMap = (int[,])map.Clone();

        for(int y = 1; y < height - 1; y++)
        {
            for(int x= 1; x < width - 1; x++)
            {
                int wallCount = GetSurroundingWallCount(x, y);

                if (wallCount > 4)
                    newMap[x, y] = 1;
                else if(wallCount < 4)
                    newMap[x, y] = 0;
            }
        }

        map = newMap;
    }

    int GetSurroundingWallCount(int x, int y)
    {
        int count = 0;
        for(int ny = y - 1; ny <= y + 1; ny++)
        {
            for(int nx = x - 1; nx <= x +1; nx++)
            {
                if (nx == x && ny == y) continue;
                if (map[nx, ny] == 1) count++;
            }
        }
        return count;
    }

    void DrawTextureFromMap()
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for(int y= 0; y < height; y++)
        {
            for (int x= 0;  x< width; x++)
            {
                Color color = (map[x, y] == 1) ? Color.black : Color.white;
                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
