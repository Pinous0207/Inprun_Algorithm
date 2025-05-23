using UnityEngine;
using System.Collections.Generic;

// BT(Behavior Tree) (행동 트리)
// 게임 AI가 어떤 행동을 어떤 순서로 수행할지를 트리 구조로 표현한 시스템

// Why BT?
// 1. FSM보다 구조적, 재사용성 높음
// 2. 행동을 트리처럼 설계해서 복잡한 AI도 논리적으로 구성 가능
// 3. 조건과 행동을 모듈로 쪼개 유지보수와 확장에 매우 유리

// Root - 트리의 시작점
// Composite - 자식 노드들을 제어 (Selector, Sequence)
// Decorator - 하나의 자식 노드만 제어 (반전, 반복 등)
// Leaf (Action/Condition) - 실제 행동 또는 조건을 수행하는 노드

// Sequence (순차 실행자) - 자식 노드들을 순서대로 실행하며, 모두 성공해야 최종 성공을 반환
// Selector (선택자) - 자식 노드들을 순서대로 평가하며, 하나라도 성공하면 즉시 성공 반환
// Leaf (리프 노드) - 행동(Action) 또는 조건(Condition) 을 실제로 수행하는 가장 말단 노드


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
