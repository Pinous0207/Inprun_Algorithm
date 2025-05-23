using UnityEngine;
using System.Collections.Generic;

// BT(Behavior Tree) (�ൿ Ʈ��)
// ���� AI�� � �ൿ�� � ������ ���������� Ʈ�� ������ ǥ���� �ý���

// Why BT?
// 1. FSM���� ������, ���뼺 ����
// 2. �ൿ�� Ʈ��ó�� �����ؼ� ������ AI�� �������� ���� ����
// 3. ���ǰ� �ൿ�� ���� �ɰ� ���������� Ȯ�忡 �ſ� ����

// Root - Ʈ���� ������
// Composite - �ڽ� ������ ���� (Selector, Sequence)
// Decorator - �ϳ��� �ڽ� ��常 ���� (����, �ݺ� ��)
// Leaf (Action/Condition) - ���� �ൿ �Ǵ� ������ �����ϴ� ���

// Sequence (���� ������) - �ڽ� ������ ������� �����ϸ�, ��� �����ؾ� ���� ������ ��ȯ
// Selector (������) - �ڽ� ������ ������� ���ϸ�, �ϳ��� �����ϸ� ��� ���� ��ȯ
// Leaf (���� ���) - �ൿ(Action) �Ǵ� ����(Condition) �� ������ �����ϴ� ���� ���� ���


//          HFSM vs BT


public class EnemyBT : MonoBehaviour
{
    private BT_Node root;
    Animator animator;

    public Transform target;
    public float speed = 2.0f;
    public float chaseRange = 5.0f;
    public float attackRange = 1.5f;
    private void Start()
    {
        animator = GetComponent<Animator>();
        root = new BT_Selector(new List<BT_Node>
        {
            new BT_Sequence(new List<BT_Node>
            {
                new BT_Leaf(CheckPlayerInRange),
                new BT_Leaf(AttackPlayer)
            }),
            new BT_Sequence(new List<BT_Node>
            {
                new BT_Leaf(CheckChaseRange),
                new BT_Leaf(ChasePlayer)
            }),
            new BT_Leaf(IDLE)
        });
    }

    private void Update()
    {
        root.Evaluate();
    }

    private BT_NodeStatus RangeCheck(float range)
    {
        float dist = Vector3.Distance(transform.position, target.position);
        return dist < range ? BT_NodeStatus.Success : BT_NodeStatus.Failure;
    }
    BT_NodeStatus CheckChaseRange()
    {
        return RangeCheck(chaseRange);
    }
   
    BT_NodeStatus CheckPlayerInRange()
    {
        return RangeCheck(attackRange);
    }

    BT_NodeStatus IDLE()
    {
        AnimatorChange("IDLE");
        return BT_NodeStatus.Success;
    }

    BT_NodeStatus AttackPlayer()
    {
        Rotate();
        AnimatorChange("ATTACK");
        return BT_NodeStatus.Success;
    }

    BT_NodeStatus ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position,
          target.position, Time.deltaTime * speed);

        Rotate();
        AnimatorChange("MOVE");

        return BT_NodeStatus.Running;
    }

    void Rotate()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0.0f;
        transform.forward = direction;
    }
    private void AnimatorChange(string temp)
    {
        animator.SetBool("IDLE", false);
        animator.SetBool("MOVE", false);
        animator.SetBool("ATTACK", false);

        animator.SetBool(temp, true);
    }
}
