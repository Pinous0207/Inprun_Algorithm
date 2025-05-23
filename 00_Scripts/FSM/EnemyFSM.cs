using UnityEngine;


// FSM(Finite State Machine)
// 한 번에 하나의 상태(State)만 가지며, 특정 조건에 따라 다른 상태로 전환되는 구조
// State - 현재 상태(Idle, Attack, Die)
// Transition - 상태 변경 조건 (거리, 시간, 이벤트 등)
// Entry / Exit - 상태 진입 시 동작 / 상태 탈출 시 동작
// Update - 상태 지속 중 반복 처리 (EX: 추격 중 이동)

// Why FSM?
// 간단하면서도 명확한 구조
// 테스트, 디버깅 용이
// 상태 별 책임 분리가 쉬움
// 모든 AI 로직의 기반 

// FSM은 AI 캐릭터가 어떤 상태에 있는지, 그리고 언제 어떻게 상태를 바꾸는지를 설계하는 틀

// FSM = Algorithm X, 문제를 푸는 계산 절차가 아니라, '객체가 어떤 상태로 행동해야 하는지'를 표현하는 구조
// FSM은 계산 모델이자, 제어 흐름을 정의하는 알고리즘 구조로 간주될 수 있습니다.

// 컴퓨터 공학 관점 : FSM은 형식적 알고리즘 모델 (오토마타 이론, 상태기계)
// 게임 개발 관점 : FSM은 설계 구조 (디자인 구조, 상태 처리 프레임워크)

// 단점
// 1. 상태 수가 많아질수록 복잡도가 급격히 증가
// EX : 상태가 3~4개일 때는 문제 X, 상태가 10개 이상이라면 상태 전이 조건을 일일이 관리하기 어렵다.
// n개의 상태가 있으면, 전이 조건은 최대 n²개가 될 수 있음.
// 상태 폭발(state explosion) 문제
// 2. 행동이 고정적이라 유연성이 부족함
// 상태마다 동작이 고정되어 있으므로, 상황에 따라 AI가 전략적으로 판단하는 행동을 만들기가 어렵다.
// EX: 체력이 50% 이하면 도망 -> 중복 코드, 설계 어려움
// 3. 조건 분기가 많아질수록 관리가 힘듦
// 4. 재사용성과 확장성 낮음

// BT, GOAP

// HFSM (Hierarchical Finite State Machine)
public class EnemyFSM : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Attack
    }

    public State currentState = State.Idle;
    Animator animator;
    public Transform target;
    public float speed = 2.0f;
    public float chaseRange = 5.0f;
    public float attackRange = 1.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch(currentState)
        {
            case State.Idle:
                IDLE();
                break;
            case State.Chase:
                CHASE();
                break;
            case State.Attack:
                ATTACK();
                break;
        }

        TransitionCheck();
    }

    private void IDLE()
    {
        AnimatorChange("IDLE");
    }

    private void CHASE()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            target.position, Time.deltaTime * speed);

        Rotate();

        AnimatorChange("MOVE");
    }

    private void ATTACK()
    {
        Rotate();
        AnimatorChange("ATTACK");
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

    void TransitionCheck()
    {
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < attackRange)
            currentState = State.Attack;
        else if (dist < chaseRange)
            currentState = State.Chase;
        else
            currentState = State.Idle;
    }
}
