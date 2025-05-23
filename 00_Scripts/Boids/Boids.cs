using UnityEngine;
using System.Collections.Generic;

// Boids ( Craig W. Reynolds ) ( 1986 )
// �� ��, ����� ���� ���� ���� �ൿ�� �𵨸��ϱ� ���� ����
// �ܼ��� ��Ģ������ ��ü���� ������ ���� �ൿ�� �ϰ� ����� �˰���

// �浹 ���� �ڿ������� ȸ��
// ��ǥ ���� ���� �̵��ص� ������ ���� ����
// ���迡 �����ϰų� Ư�� �������� ������ �̵� ����

// �ٽ� ��Ģ 
// Separation (�и�) - �ʹ� ����� �̿��� ����
// Alignment (����) - �ֺ� Boid�� ��� ����� �����ϰ� �̵�
// Cohesion (����) - ������ �߽� �������� �̵�

// �� ���� ��, �� ��, ����, ����� �� �ڿ������� '����' �ൿ
// �ؾ� ���� �ùķ��̼�
// RTS �̴Ͼ� AI
// ���� ���� (����Ʈ)

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

    // �и� (Separation) - �ʹ� ����� ��ü�� ���������� ��
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

    // ���� (Alignmenet) - �̿���� ���� �������� �����̷��� ��
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


    // ���� (Cohesion) - ������ �߽����� ���̷��� ��
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
