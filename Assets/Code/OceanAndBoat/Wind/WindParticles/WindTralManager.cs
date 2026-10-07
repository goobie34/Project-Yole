using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WindTralManager : MonoBehaviour
{

    //private List<GameObject> children = new();
    [SerializeField] private Rigidbody speedTarget;

    [SerializeField] private Transform ParticleHolder;

    [SerializeField] private GameObject prefab;

    //[SerializeField] private int maxTrailParticles = 10;

    [SerializeField] private float spawnPerSecond = 1;
    [SerializeField] private float spawnPerSpeed = 1;

    [SerializeField] private float maxLifeTime = 1.0f;
    [SerializeField] private float minLifeTime = 1.0f;

    [SerializeField] private float maxSpawnRange = 200.0f;
    [SerializeField] private float maxSpawnHeight = 10.0f;

    [SerializeField] private float spawnHeighOffset = 10f;

    private float spawnTimeCounter = 0;
    private float spawnSpeedCounter = 0;

    private Vector3 originPos => transform.position + Vector3.up * spawnHeighOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(ParticleHolder == null)
            ParticleHolder = new GameObject("WindParticleContainer").transform;
    }

   
    // Update is called once per frame
    void FixedUpdate()
    {
        SpawnBySpeed(Time.fixedDeltaTime);
        SpawnLoop(Time.fixedDeltaTime);
    }

    private void SpawnBySpeed(float deltaTime)
    {
        if (speedTarget == null)
            return;

        if (spawnPerSpeed == 0)
            return;

        float speed = speedTarget.linearVelocity.magnitude;

        spawnSpeedCounter += (speed * deltaTime * spawnPerSpeed);

        while (true)
        {
            if(spawnSpeedCounter < 1)
                break;
            --spawnSpeedCounter;
            SpawnParticle();
        }
    }

    private void SpawnLoop(float deltaTime)
    {
        if (spawnPerSecond == 0)
            return;

        spawnTimeCounter += ((deltaTime) * spawnPerSecond);

        while (true)
        {
            if (spawnTimeCounter < 1)
                break;
            --spawnTimeCounter;
            SpawnParticle();
        }

        //while (children.Count < maxTrailParticles)
        //{
        //    children.Add(null);
        //}

        //for (int i = 0; i < maxTrailParticles; i++)
        //{
        //    if (children[i] != null)
        //        continue;

        //    children[i] = SpawnParticle();

        //}


    }

    private GameObject SpawnParticle()
    {
        var particle = Instantiate(prefab,ParticleHolder);

        var lifetime = particle.AddComponent<LifetimeTimer>();

        lifetime.lifetime = Random.Range(minLifeTime, maxLifeTime);

        particle.transform.position = originPos + GetRandomPos(maxSpawnRange, maxSpawnHeight, 1);

        return particle;
    }

    private Vector3 GetRandomPos(float maxRange, float maxHeight, float minLerp)
    {
        var randomDirection = Random.insideUnitSphere;

        randomDirection.y = Mathf.Abs(randomDirection.y);

        randomDirection.x *= maxRange;
        randomDirection.z *= maxRange;
        randomDirection.y *= maxHeight;

        randomDirection *= Random.Range(Mathf.Max(minLerp,0), 1);

        return randomDirection;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(originPos, Vector3.up * maxSpawnHeight);

        Gizmos.DrawRay(originPos, Vector3.left * maxSpawnRange);
        Gizmos.DrawRay(originPos, Vector3.right * maxSpawnRange);
        Gizmos.DrawRay(originPos, Vector3.forward * maxSpawnRange);
        Gizmos.DrawRay(originPos, Vector3.back * maxSpawnRange);
    }
}
