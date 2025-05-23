using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil.Cil;


// Steering Behaviors ( Craig W. Reynolds ) ( 1999 )
// Steering behaviors system, behavioral control technique

// 유닛이 환경과 목표에 따라 방향과 속도를 자연스럽게 조절할 수 있도록 벡터 연산을 조합한
// 행동 제어 기법
// 자율 캐릭터의 방향 조절 및 이동을 위한 알고리즘적 프레임워크

// 조향(Steering) = desiredVelocity - currentVelocity
// desiredVelocity = 캐릭터가 "원하는 방향"
// currentVelocity = 캐릭터의 현재 이동 방향

// 핵심 규칙
// Seek - 목표를 향해 직선적으로 따라감
// Flee - 특정 지점에서 멀어지려 함
// Arrive - 목표 근처에서 속도를 줄이며 멈춤
// Wander - 자연스럽게 방향을 틀며 떠돌기
// Pursuit - 이동 중인 타겟을 예측해서 추적
// Evade - 이동 중인 타겟의 미래 위치를 회피
// Obstacle Avoidance - 장애물 근처에서 반대 방향으로 Steering

public enum AgentState
{
    Seek,
    Flee,
    Wander,
    Idle
}

public interface ISteeringBehavior
{
    Vector3 CalculateForce(SteeringBehavior agent);
    float Weight { get; set; }
}

public class SeekBehavior : ISteeringBehavior
{
    public Vector3 Target;
    public float Weight { get; set; } = 1.0f;
    public SeekBehavior(Vector3 target) { Target = target; }

    public Vector3 CalculateForce(SteeringBehavior agent)
    {
        Vector3 desired = (Target - agent.transform.position).normalized * agent.maxSpeed;
        return desired - agent.velocity;
    }
}

public class FleeBehavior : ISteeringBehavior
{
    public Vector3 Target;
    public float Weight { get; set; } = 1.0f;
    public FleeBehavior(Vector3 target) { Target = target; }

    public Vector3 CalculateForce(SteeringBehavior agent)
    {
        Vector3 desired = (agent.transform.position - Target).normalized * agent.maxSpeed;
        return desired - agent.velocity;
    }
}

public class WanderBehavior : ISteeringBehavior
{
    private Vector3 wanderTarget = Vector3.zero;
    public float CircleDistance = 2.0f;
    public float CircleRadius = 1.5f;
    public float Jitter = 0.2f;
    public float Weight { get; set; } = 1.0f;

    public Vector3 CalculateForce(SteeringBehavior agent)
    {
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * Jitter, 0, Random.Range(-1.0f, 1.0f) * Jitter);
        wanderTarget = wanderTarget.normalized * CircleRadius;

        Vector3 targetLocal = wanderTarget + Vector3.forward * CircleDistance;
        Vector3 targetWorld = agent.transform.TransformPoint(targetLocal);

        Vector3 desired = (targetWorld - agent.transform.position).normalized * agent.maxSpeed;
        return desired - agent.velocity;
    }
}

public class ObstacleAvoidanceBehavior : ISteeringBehavior
{
    public float RayLength = 5.0f;
    public float Weight { get; set; } = 2.0f;
    private LayerMask mask;
     private Vector3[] lastDirections; // Gizmos 용
    public ObstacleAvoidanceBehavior(LayerMask obstacleMask)
    {
        mask = obstacleMask;
    }

    public Vector3 CalculateForce(SteeringBehavior agent)
    {
        RaycastHit hit;
        Vector3[] directions =
        {
            agent.transform.forward,
            Quaternion.AngleAxis(30, Vector3.up) * agent.transform.forward,
            Quaternion.AngleAxis(-30, Vector3.up) * agent.transform.forward,
        };

        foreach (var dir in directions)
        {
            if (Physics.Raycast(agent.transform.position, dir, out hit, RayLength, mask))
            {
                Vector3 avoidDir = Vector3.Reflect(dir, hit.normal);
                return avoidDir.normalized * agent.maxSpeed;
            }
        }
        return Vector3.zero;
    }
    public void DrawGizmos(Transform origin)
    {
        if (lastDirections == null) return;

        Gizmos.color = Color.red;
        foreach (var dir in lastDirections)
        {
            Gizmos.DrawLine(origin.position, origin.position + dir.normalized * RayLength);
        }
    }
}

public class SteeringBehavior : MonoBehaviour
{
    public AgentState CurrentState;
    public Transform player;
    public Player playerStats;
    public float maxSpeed = 5.0f;
    public float maxForce = 10.0f;
    public float detectionRadius = 10.0f;
    public Vector3 velocity;

    private ISteeringBehavior seek;
    private ISteeringBehavior flee;
    private ISteeringBehavior wander;
    private ISteeringBehavior obstacleAvoidance;
    private List<ISteeringBehavior> behaviors = new List<ISteeringBehavior>();

    private void Start()
    {
        seek = new SeekBehavior(player.position);
        flee = new FleeBehavior(player.position);
        wander = new WanderBehavior();
        obstacleAvoidance = new ObstacleAvoidanceBehavior(LayerMask.GetMask("Obstacle"));

        behaviors.Add(seek);
        behaviors.Add(flee);
        behaviors.Add(wander);
        behaviors.Add(obstacleAvoidance);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        ((SeekBehavior)seek).Target = player.position;
        ((FleeBehavior)flee).Target = player.position;

        seek.Weight = (playerStats.Health <= 50.0f && distanceToPlayer <= detectionRadius) ? 1.0f : 0.0f;
        flee.Weight = (playerStats.Health > 50f && distanceToPlayer <= detectionRadius) ? 1.0f : 0.0f;
        wander.Weight = (distanceToPlayer > detectionRadius) ? 1.0f : 0.0f;
        obstacleAvoidance.Weight = 2.0f;

        Vector3 steering = Vector3.zero;
        foreach(var behavior in behaviors)
        {
            steering += behavior.CalculateForce(this) * behavior.Weight;
        }

        steering = Vector3.ClampMagnitude(steering, maxForce);
        velocity += steering * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity);

        if (seek.Weight > 0f) CurrentState = AgentState.Seek;
        else if (flee.Weight > 0.0f) CurrentState = AgentState.Flee;
        else if (wander.Weight > 0.0f) CurrentState = AgentState.Wander;
        else CurrentState = AgentState.Idle;
    }
}
