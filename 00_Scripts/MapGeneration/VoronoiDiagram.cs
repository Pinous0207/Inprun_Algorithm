using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// Voronoi Diagram 
// 2D 평면 상에 여러 개의 점(Seed Point)이 주어졌을 때,
// 각 점에서 가장 가까운 영역을 나누는 분할 구조를 말한다.

// 점마다 고유한 영향 범위를 가진다.
// 모든 영역은 비중첩, 완전 분할
// 중심점 간 경계선은 같은 거리에 있는 지점들의 집합 (그래서 자연스러움)
public class VoronoiDiagram : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int pointCount = 20;
    public int seed = 0;

    private Texture2D voronoiTexture;
    [SerializeField] private RawImage image;

    private void Start()
    {
        GenerateVoronoi();
    }

    void GenerateVoronoi()
    {
        voronoiTexture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        List<Vector2> points = new List<Vector2>(); // Voronoi Diagram의 중심점
        List<Color> colors = new List<Color>(); // 각 point에 연결된 고유 색상
        Random.InitState(seed);
        for(int i = 0; i < pointCount; i++)
        {
            points.Add(new Vector2(Random.Range(0, width), Random.Range(0, height)));
            colors.Add(Random.ColorHSV());
        }

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float minDist = float.MaxValue;
                int closestPoint = 0;
                for(int i = 0; i < points.Count; i++)
                {
                    float dist = Vector2.SqrMagnitude(new Vector2(x, y) - points[i]);
                    if(dist < minDist)
                    {
                        minDist = dist;
                        closestPoint = i;
                    }
                }
                pixels[y * width + x] = colors[closestPoint];
            }
        }
        voronoiTexture.SetPixels(pixels);
        voronoiTexture.Apply();

        image.texture = voronoiTexture;
    }
}
