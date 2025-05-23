using System.Collections;
using UnityEngine;

// A* (A-Star) : '�ְ� ����� ��� Ž��'�̶�� �ǹ̿��� �̸� �ٿ���
/// <summary>
/// ������ ��θ� ���� ȿ�������� ã�� Ž�� �˰���
/// ��ǥ ���������� ���� �Ÿ��� ����ؼ� �ִ� ��� + ���� Ž���� ���ÿ� �޼�
/// </summary>
// f(n) = g(n) + h(n)
// g = ���������� ������� ���� �̵� �Ÿ�
// h = ���� �������� ��ǥ������ ���� �Ÿ� (�޸���ƽ)
// f = ���� ������ �� ���� ���
// f�� ���� ���� ������ Ž��

//  DFS = Stack(LIFO)(���Լ���)
//  BFS = Queue(FIFO)(���Լ���) - ���� �������
//  A* = Priority Queue (�켱���� ť) - �켱������ ���� ���� �� 

// Queue , Enqueue(���� �ִ� ��), Dequeue(���� ���� ��)
// Priority Queue , Enqueue(A, 5), Enqueue(B, 2), Dequeue() -> B

// Heuristic(�޸���ƽ) : �Ϻ��ϰ� ������� ������, ������ ���� ������ �����ϴ� ���
// '���� ��ǥ�� �󸶳� ������?' ������ �������ִ� ����� �ǹ�

// A* : h(n) <- ���� �Ÿ�
// Mathf.Abs = Absolute (��������)
// ����ư �Ÿ� : abs(a.x - b.x) + abs(a.y - b.y)
// ����ư �Ÿ��� �� ���� ���� �Ÿ����� ������ �ƴ� ���� ������θ� ������ �� ����
// ���� �ִ� �Ÿ�
// �̱� ���� ����ư ������ ���ΰ� ���� ����(Grid)�� �Ǿ� �ֽ��ϴ�.
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
