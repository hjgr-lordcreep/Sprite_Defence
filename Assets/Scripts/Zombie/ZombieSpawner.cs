using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [System.Serializable]
    struct SpawnArea
    {
        public float _min, _max;
    }

    [Header("Boids")]
    [SerializeField]
    Transform _boids;
    [SerializeField]
    float _speed = 5f;
    [SerializeField]
    LayerMask _boidsLayer;

    //[Header("Range")]
    //[SerializeField, Range(0, 100f)]
    //float _detectRange = 10f;
    //[SerializeField, Range(0, 100f)]
    //float _separationRange = 5f;

    [Header("Spawn")]
    [SerializeField, Range(1, 1000)]
    int _spawnCount;
    [SerializeField]
    SpawnArea _spawnAreaPosX;
    [SerializeField]
    SpawnArea _spawnAreaPosY;
    [SerializeField]
    SpawnArea _spawnAreaPosZ;

    List<Transform> zombieList = new();

    AlignmentRule _alignmentRule = new();
    CohesionRule _cohesionRule = new();
    SeparationRule _separtionRule = new();

    [SerializeField]
    private Transform playerTr = null;

    private void Awake()
    {
        SpawnBoids();
    }

    void SpawnBoids()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(_spawnAreaPosX._min, _spawnAreaPosX._max),
                Random.Range(_spawnAreaPosY._min, _spawnAreaPosY._max),
                Random.Range(_spawnAreaPosZ._min, _spawnAreaPosZ._max));

            zombieList.Add(Instantiate(_boids, spawnPos, Quaternion.identity));
        }
    }

    // 좀비들 움직임
    private void Update()
    {
        foreach (var agent in zombieList)
        {
            //Vector3 dir = _cohesionRule.GetDirection(
            //        agent,
            //        GetNeighbor(agent, _detectRange)
            //        );

            //dir += _alignmentRule.GetDirection(
            //    agent,
            //    GetNeighbor(agent, _detectRange)
            //    );

            //dir += _separtionRule.GetDirection(
            //    agent,
            //    GetNeighbor(agent, _separationRange)
            //    );

            Vector3 dir = Vector3.zero;

            Vector3 dis = playerTr.position - agent.position;
            dir += dis;

            dir = Vector3.Lerp(agent.transform.forward, dir, Time.deltaTime);
            dir.Normalize();

            //if (dis.magnitude < 3f) return;
            agent.transform.position += dir * _speed * Time.deltaTime;

            agent.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}