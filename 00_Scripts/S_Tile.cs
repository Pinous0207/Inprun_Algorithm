using UnityEngine;

public class S_Tile : MonoBehaviour
{
    public int moveCost = 1;
    public bool isWalkable = true;

    public int x;
    public int y;
    public Vector3 worldPosition;

    public bool isVisited = false;
    public Color mainColor;

    public void Visited(bool Visit)
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor", Visit ? mainColor : Color.gray);
    }
}
