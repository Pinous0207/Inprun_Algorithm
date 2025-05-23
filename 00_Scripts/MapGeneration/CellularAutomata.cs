using UnityEngine;

// Cellular Automata
// 각 셀이 이웃 셀들의 상태에 따라 자신의 상태를 갱신하는 시스템
// 보통 2D 격자에서 각 셀은 1(살아있음) / 0(죽어있음) 같은 이진 상태를 가지며
// 반복적인 규칙에 따라 전체 맵을 진화시킴
// 동굴 2D
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

    // 무작위로 0과 1을 채워 넣어 초기 지형 맵을 생성
    // 가장자리(테두리)는 무조건 벽(1), 내부는 일정 확률로 벽/빈공간 결정
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
