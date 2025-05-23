using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int boidCount = 30;
    public float spawnRadius = 10.0f;

    public float separationDistance = 1.0f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;

    public float maxSpeed = 5.0f;
    public float maxForce = 0.5f;

    public List<Boids> boids = new List<Boids>();

    private void Start()
    {
        for(int i = 0; i < boidCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject obj = Instantiate(boidPrefab, pos, Quaternion.identity);
            Boids boid = obj.AddComponent<Boids>();
            boid.Initalize(this);
            boids.Add(boid);
        }
    }
}
