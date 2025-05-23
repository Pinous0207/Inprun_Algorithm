using System.Collections;
using UnityEngine;

// A* (A-Star) : '최고 등급의 경로 탐색'이라는 의미에서 이름 붙여짐
/// <summary>
/// 최적의 경로를 가장 효율적으로 찾는 탐색 알고리즘
/// 목표 지점까지의 예상 거리를 고려해서 최단 경로 + 빠른 탐색을 동시에 달성
/// </summary>
// f(n) = g(n) + h(n)
// g = 시작점부터 현재까지 실제 이동 거리
// h = 현재 지점에서 목표까지의 예상 거리 (휴리스틱)
// f = 현재 지점의 총 예상 비용
// f가 가장 낮은 노드부터 탐색

//  DFS = Stack(LIFO)(후입선출)
//  BFS = Queue(FIFO)(선입선출) - 들어온 순서대로
//  A* = Priority Queue (우선순위 큐) - 우선순위가 가장 높은 것 

// Queue , Enqueue(값을 넣는 것), Dequeue(값을 빼는 것)
// Priority Queue , Enqueue(A, 5), Enqueue(B, 2), Dequeue() -> B

// Heuristic(휴리스틱) : 완벽하게 계산하지 않지만, 괜찮은 답을 빠르게 추측하는 방법
// '대충 목표에 얼마나 가까울까?' 빠르게 추측해주는 계산을 의미

// A* : h(n) <- 예상 거리
// Mathf.Abs = Absolute (절대적인)
// 맨해튼 거리 : abs(a.x - b.x) + abs(a.y - b.y)
// 맨해튼 거리는 두 지점 간의 거리에서 직선이 아닌 격자 기반으로만 움직일 수 있을
// 때의 최단 거리
// 미국 뉴욕 맨헤튼 지역은 도로가 격자 형태(Grid)로 되어 있습니다.
public class AStarVisualizer : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(ShowASTARPath());
        }
    }

    IEnumerator ShowASTARPath()
    {
        var path = pathFinder.GetAStarPath();
        var tiles = gridManager.tiles;

        foreach (var pos in path)
        {
            tiles[pos.y, pos.x].SetColor(Color.yellow);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
