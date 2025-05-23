using UnityEngine;
using System.Collections.Generic;

// Boids ( Craig W. Reynolds ) ( 1986 )
// 새 떼, 물고기 떼와 같은 집단 행동을 모델링하기 위해 개발
// 단순한 규칙만으로 개체들이 복잡한 무리 행동을 하게 만드는 알고리즘

// 충돌 없이 자연스럽게 회피
// 목표 지점 없이 이동해도 정돈된 군집 유지
// 위험에 반응하거나 특정 방향으로 빠르게 이동 가능

// 핵심 규칙 
// Separation (분리) - 너무 가까운 이웃을 피함
// Alignment (정렬) - 주변 Boid의 평균 방향과 유사하게 이동
// Cohesion (응집) - 무리의 중심 방향으로 이동

// 적 몬스터 떼, 새 떼, 벌레, 물고기 떼 자연스러운 '집단' 행동
// 해양 생물 시뮬레이션
// RTS 미니언 AI
// 입자 연출 (이펙트)

public class Boids : MonoBehaviour
{
    BoidManager manager;
    Vector3 velocity;

    public void Initalize(BoidManager manager)
    {
        this.manager = manager;
        velocity = Random.onUnitSphere * manager.maxSpeed;
        velocity.y = 0f;
    }

    private void Update()
    {
        Vector3 separation = ComputeSeparation() * manager.separationWeight;
        Vector3 alignment = ComputeAlignment() * manager.alignmentWeight;
        Vector3 cohesion = ComputeCohesion() *  manager.cohesionWeight;

        Vector3 acceleration = separation + alignment + cohesion;
        velocity += acceleration * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, manager.maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity);
    }

    // 분리 (Separation) - 너무 가까운 개체와 떨어지려는 힘
    Vector3 ComputeSeparation()
    {
        Vector3 force = Vector3.zero;
        foreach(var boid in manager.boids)
        {
            if (boid == this) continue;
            float dist = Vector3.Distance(transform.position, boid.transform.position);
            force += (transform.position - boid.transform.position).normalized / Mathf.Max(dist, 0.01f);
        }
        return force;
    }

    // 정렬 (Alignmenet) - 이웃들과 같은 방향으로 움직이려는 힘
    Vector3 ComputeAlignment()
    {
        Vector3 avgVel = Vector3.zero;
        int count = 0;
        foreach(var boid in manager.boids)
        {
            if (boid == this) continue;
            avgVel += boid.velocity;
            count++;
        }
        avgVel /= count;
        return (avgVel - velocity).normalized;
    }


    // 응집 (Cohesion) - 무리의 중심으로 모이려는 힘
    Vector3 ComputeCohesion()
    {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach(var boid in manager.boids)
        {
            if (boid == this) continue;
            center += boid.transform.position;
            count++;
        }
        center /= count;
        return (center - transform.position).normalized;
    }
}
