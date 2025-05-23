using UnityEngine;


// FSM(Finite State Machine)
// �� ���� �ϳ��� ����(State)�� ������, Ư�� ���ǿ� ���� �ٸ� ���·� ��ȯ�Ǵ� ����
// State - ���� ����(Idle, Attack, Die)
// Transition - ���� ���� ���� (�Ÿ�, �ð�, �̺�Ʈ ��)
// Entry / Exit - ���� ���� �� ���� / ���� Ż�� �� ����
// Update - ���� ���� �� �ݺ� ó�� (EX: �߰� �� �̵�)

// Why FSM?
// �����ϸ鼭�� ��Ȯ�� ����
// �׽�Ʈ, ����� ����
// ���� �� å�� �и��� ����
// ��� AI ������ ��� 

// FSM�� AI ĳ���Ͱ� � ���¿� �ִ���, �׸��� ���� ��� ���¸� �ٲٴ����� �����ϴ� Ʋ

// FSM = Algorithm X, ������ Ǫ�� ��� ������ �ƴ϶�, '��ü�� � ���·� �ൿ�ؾ� �ϴ���'�� ǥ���ϴ� ����
// FSM�� ��� ������, ���� �帧�� �����ϴ� �˰��� ������ ���ֵ� �� �ֽ��ϴ�.

// ��ǻ�� ���� ���� : FSM�� ������ �˰��� �� (���丶Ÿ �̷�, ���±��)
// ���� ���� ���� : FSM�� ���� ���� (������ ����, ���� ó�� �����ӿ�ũ)

// ����
// 1. ���� ���� ���������� ���⵵�� �ް��� ����
// EX : ���°� 3~4���� ���� ���� X, ���°� 10�� �̻��̶�� ���� ���� ������ ������ �����ϱ� ��ƴ�.
// n���� ���°� ������, ���� ������ �ִ� n������ �� �� ����.
// ���� ����(state explosion) ����
// 2. �ൿ�� �������̶� �������� ������
// ���¸��� ������ �����Ǿ� �����Ƿ�, ��Ȳ�� ���� AI�� ���������� �Ǵ��ϴ� �ൿ�� ����Ⱑ ��ƴ�.
// EX: ü���� 50% ���ϸ� ���� -> �ߺ� �ڵ�, ���� �����
// 3. ���� �бⰡ ���������� ������ ����
// 4. ���뼺�� Ȯ�强 ����

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
