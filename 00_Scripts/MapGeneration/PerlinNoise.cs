using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


// Perlin Noise - Ken Perlin
// �ڿ������� ������ ���� ���� ������ִ� �˰���.
// �Ϲ� �������� �ٸ���, ���� ���̿� �ε巯�� ���Ӽ��� �־ ����̳� ����ó�� �ڿ� ����
// ���� ���Ӽ�
// Mathf.PerlinNoise(x(float),y(float))
//   Random(Random.Range) vs Perlin Noise
//���Ӽ� - / ���� / ���� /
//���� - / 0.2, 0.95, 0.1, .07 / 0.44, 0.46, 0.47, 0.45
public class PerlinNoise : MonoBehaviour
{
    public Terrain terrain;

    public bool showPlaneTexture = true;
    public int width = 128;
    public int height = 128;
    public float scale = 10.0f;

    public bool showGraph = true;
    public LineRenderer lineRenderer;
    public float graphHeight = 2.0f;
    public float graphWidth = 10.0f;

    private void Start()
    {
        float[,] heights = new float[width, height];

        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                heights[x, y] = noise;
            }
        }
        terrain.terrainData.heightmapResolution = width + 1;
        
        terrain.transform.position = new Vector3(-(width / 2), -100.0f, -(height / 2));

        terrain.terrainData.size = new Vector3(width, 50, height);
        terrain.terrainData.SetHeights(0, 0, heights);

        DrawTextureForPlane();
        DrawDebugGraph();
    }

    void DrawTextureForPlane()
    {
        if (!showPlaneTexture) return;

        Texture2D noiseTex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                float noise = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[y * width + x] = new Color(noise, noise, noise);
            }
        }

        noiseTex.SetPixels(pixels);
        noiseTex.Apply();

        GetComponent<Renderer>().material.mainTexture = noiseTex;

    }

    void DrawDebugGraph()
    {
        if (!showGraph || lineRenderer == null) return;

        lineRenderer.positionCount = width;
        for(int x = 0; x < width; x++)
        {
            float t = (float)x / (width - 1);
            float xCoord = t * scale;
            float noise = Mathf.PerlinNoise(xCoord, 0f);

            float graphX = t * graphWidth;
            float graphY = noise * graphHeight;
            Vector3 point = new Vector3(graphX, graphY, 0.0f);
            lineRenderer.SetPosition(x, point);
        }
    }
}
