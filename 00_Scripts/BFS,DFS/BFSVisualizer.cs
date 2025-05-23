using System.Collections;
using UnityEngine;

// BFS (Breath-First Search)란? == 너비 우선 탐색
/// <summary>
/// 현재 노드에서 인접한 노드들부터 먼저 모두 탐색, 그 인접 노드들의
/// 인접 노드들을 탐색하는 방식
/// FIFO(First in First out) - Queue (선입선출)
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
