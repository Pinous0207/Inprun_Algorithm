using System.Collections;
using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;

// DFS (Depth-First Search) 기반의 Backtracking 미로
// 한 방향으로 가능한 깊이까지 계속 전직하다가,
// 더 이상 갈 수 없을 때 그제서야 한 칸씩 되돌아오면서 다른 경로를 탐색하는 방식

// 시작 - 격자에서 시작 셀을 선택하고 방문 처리
// 전진 - 현재 셀에서 랜덤ㅈ한 방향으로 이동 가능한 인접 셀을 찾음
// 연결 - 이동하려는 셀과 현재 셀 사이의 벽을 제거해 길을 연결
// 재귀 or 스택 - 이동한 셀에서 같은 과정을 반복
// 백트래킹 - 더 이상 이동할 셀이 없으면 스택(Stack)에서 꺼내서 되돌아감
public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public RawImage image;

    public GameObject WallPrefab;
    public GameObject CharacterPrefab;
    public Transform mazeParent;
    public Transform planeTransform;

    private int[,] map;

    private void Start()
    {
        GenerateMaze();
        BuildMaze();
        DrawMazeTexture(); // ctrl + k + / 
    }

    void GenerateMaze()
    {
        map = new int[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                map[x, y] = 1;

        Vector2Int start = new Vector2Int(1, 1);
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        // Queue - Enqueue, Dequeue
        // Stack - Push, Pop
        stack.Push(start);
        map[start.x, start.y] = 0;

        Vector2Int[] directions =
        {
            new Vector2Int(0, 2), // Vector2Int.up * 2
            new Vector2Int(0, -2), // Vector2Int.down
            new Vector2Int(2,0), // Vector2Int.right
            new Vector2Int(-2, 0) // Vector2Int.left
        };

        System.Random rand = new System.Random();

        while(stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach(var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if(neighbor.x > 0 && neighbor.x < width - 1 && 
                    neighbor.y > 0 && neighbor.y < height - 1 &&
                    map[neighbor.x, neighbor.y] == 1)
                {
                    neighbors.Add(neighbor);
                }
            }

            if(neighbors.Count > 0)
            {
                stack.Push(current);
                Vector2Int chosen = neighbors[rand.Next(neighbors.Count)];
                Vector2Int between = (current + chosen) / 2;

                map[chosen.x, chosen.y] = 0;
                map[between.x, between.y] = 0;

                stack.Push(chosen);
            }
        }
    }

    void BuildMaze()
    {
        float scaleX = map.GetLength(0) / 2;
        float scaleZ = map.GetLength(1) / 2;

        planeTransform.localScale = new Vector3(scaleX * 2 / 10, 1, scaleZ * 2 / 10);
        planeTransform.position = new Vector3(scaleX, -0.5f, scaleZ);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x, 0, y);

                if (x == 1 && y == 1)
                {
                    GameObject character = Instantiate(CharacterPrefab, pos, Quaternion.identity, mazeParent);
                    character.name = "PLAYER";
                }
                else if (map[x,y] == 1)
                {
                    GameObject wall = Instantiate(WallPrefab, pos, Quaternion.identity, mazeParent);
                    wall.name = $"Wall_{x}_{y}";
                }
            }
        }
    }

    void DrawMazeTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                pixels[y * width + x] = (map[x, y] == 1) ? Color.black : Color.white;

        texture.SetPixels(pixels);
        texture.Apply();
        image.texture = texture;
    }
}
