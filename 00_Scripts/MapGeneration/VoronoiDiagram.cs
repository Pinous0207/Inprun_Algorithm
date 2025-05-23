using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// Voronoi Diagram 
// 2D ��� �� ���� ���� ��(Seed Point)�� �־����� ��,
// �� ������ ���� ����� ������ ������ ���� ������ ���Ѵ�.

// ������ ������ ���� ������ ������.
// ��� ������ ����ø, ���� ����
// �߽��� �� ��輱�� ���� �Ÿ��� �ִ� �������� ���� (�׷��� �ڿ�������)
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

        List<Vector2> points = new List<Vector2>(); // Voronoi Diagram�� �߽���
        List<Color> colors = new List<Color>(); // �� point�� ����� ���� ����
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
