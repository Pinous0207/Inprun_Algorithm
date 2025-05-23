using System.Collections;
using UnityEngine;

// BFS (Breath-First Search)��? == �ʺ� �켱 Ž��
/// <summary>
/// ���� ��忡�� ������ ������� ���� ��� Ž��, �� ���� ������
/// ���� ������ Ž���ϴ� ���
/// FIFO(First in First out) - Queue (���Լ���)
/// </summary>
public class BFSVisualizer : MonoBehaviour
{
    PathFinder pathFinder;
    GridManager gridManager;
    private void Start()
    {
        pathFinder = GetComponent<PathFinder>();
        gridManager = GetComponent<GridManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(ShowBFSPath());
        }
    }

    IEnumerator ShowBFSPath()
    {
        var path = pathFinder.GetBFSPath();
        var tiles = gridManager.tiles;

        foreach(var pos in path)
        {
            tiles[pos.y, pos.x].SetColor(Color.blue);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
