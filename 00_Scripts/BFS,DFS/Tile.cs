using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f);
    }

}
