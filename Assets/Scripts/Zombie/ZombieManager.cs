using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> zombiePrefabs = null;
    [SerializeField]
    private int[] weights = { 1, 1, 20 }; // 초기 가중치
    private int increment1 = 1; // 첫 번째 프리팹 가중치 증가량
    private int increment2 = 2; // 두 번째 프리팹 가중치 증가량

    [SerializeField]
    private List<Zombie> zombieList = null;
    [SerializeField]
    private int MaxSpawnCount = 30;

    [SerializeField]
    private int highWeightValue = 200; // 높은 가중치 값
    [SerializeField]
    private int lowWeightValue = 10; // 낮은 가중치 값
    [SerializeField]
    private float spawntime = 0.1f;
    [SerializeField]
    private float spawnRange = 5f;


    private IEnumerator asd = null;
    private bool activeCoroutine = false;

    private Zombie zombie = null;
    private int activeZombie;
    private Transform[] spawnPoints = null;
    private int[] spawnWeights;
    private int totalWeight;

    float timer;

    public int ActiveZombieCount { get; private set; }

    // 좀비 매니저가 활성화 될 때 좀비 카운트 +/- 이벤트 등록
    private void OnEnable()
    {
        Zombie.OnZombieEnabled += IncrementZombieCount;
        Zombie.OnZombieDisabled += DecrementZombieCount;
    }
    // 좀비 매니저가 비활성화 될 때 좀비 카운트 +/- 이벤트 제거
    private void OnDisable()
    {
        Zombie.OnZombieEnabled -= IncrementZombieCount;
        Zombie.OnZombieDisabled -= DecrementZombieCount;
    }

    // 액티브 좀비 카운트 증가 함수
    private void IncrementZombieCount()
    {
        ActiveZombieCount++;
        //Debug.Log("Active Zombies: " + ActiveZombieCount);
    }

    // 액티브 좀비 카운트 감소 함수
    private void DecrementZombieCount()
    {
        // 액티브 좀비 카운트가 0이하로 내려가지 않음
        ActiveZombieCount = Mathf.Max(0, ActiveZombieCount - 1);
        //Debug.Log("Active Zombies: " + ActiveZombieCount);
    }


    private void Awake()
    {
        zombieList = new List<Zombie>();
        spawnPoints = GetComponentsInChildren<Transform>();
        //StartCoroutine(PoolingCoroutine());
    }

    private void Update()
    {
        //timer += Time.deltaTime;
        ZombieClean();

        if(UIManager.instance.isLive)
        {
            if (ActiveZombieCount < MaxSpawnCount / 3.0f && activeCoroutine == false)
            {
                activeCoroutine = true;
                InitWeights(); // 가중치 초기화 함수 호출
                asd = PoolingCoroutine();
                StartCoroutine(asd);
                //StartCoroutine(PoolingCoroutine());
            }

        }

    }

    // 좀비를 스폰할 때 스폰 위치의 가중치를 결정하는 함수
    // 가중치가 높은 곳이 스폰 확률이 높음
    private void InitWeights()
    {
        spawnWeights = new int[spawnPoints.Length];

        // 랜덤으로 하나의 포인트에 높은 가중치 설정
        int highWeightIndex = Random.Range(1, spawnPoints.Length); // 첫 번째(부모) 제외

        for (int i = 1; i < spawnPoints.Length; i++) // 첫 번째 제외
        {
            spawnWeights[i] = (i == highWeightIndex) ? highWeightValue : lowWeightValue;
        }

        // 가중치 합계 계산
        totalWeight = 0;
        foreach (int weight in spawnWeights)
        {
            totalWeight += weight;
        }
    }

    //private void Pooling()
    //{
    //    for (int i = 0; i < MaxSpawnCount; i++)
    //    {
    //        if (timer < 0.2f) return;
    //        zombie = null;
    //        if (GetValidZombie(out zombie))
    //        {
    //            // 위치를 가중치 기반으로 선택
    //            Vector3 spawnPosition = SelectSpawnPoint();
    //            zombie.Init(spawnPosition);
    //            ++activeZombie;
    //        }
    //        else
    //        {
    //            Vector3 spawnPosition = SelectSpawnPoint();
    //            zombie = SpawnZombie(spawnPosition);
    //            zombie.Init(spawnPosition);
    //            zombieList.Add(zombie);
    //            ++activeZombie;
    //        }
    //    }
    //}

    // 오브젝트 풀링을 이용해서 최대 좀비 수를 딜레이를 주면서 하나씩 스폰함
    private IEnumerator PoolingCoroutine()
    {
        for (int i = 0; i < MaxSpawnCount; i++)
        {
            //if (timer < 0.2f) return;
            yield return new WaitForSeconds(spawntime);
            zombie = null;
            if (GetValidZombie(out zombie))
            {
                // 위치를 가중치 기반으로 선택
                Vector3 spawnPosition = SelectSpawnPoint();
                zombie.Init(spawnPosition);
                //++activeZombie;
            }
            else
            {
                Vector3 spawnPosition = SelectSpawnPoint();
                zombie = SpawnZombie(spawnPosition);
                zombie.Init(spawnPosition);
                zombieList.Add(zombie);
                //++activeZombie;
            }
        }
        activeCoroutine = false;
        // 다음번 생성되는 좀비의 최대 수를 5늘림
        MaxSpawnCount += 5;
        // 좀비 스폰후 좀비들의 가중치 변경
        IncreaseWeights();
    }

    private Vector3 SelectSpawnPoint()
    {
        // 랜덤 값 생성 (0 ~ totalWeight - 1)
        int randomValue = Random.Range(0, totalWeight);

        // 랜덤 값에 따라 스폰 포인트 선택
        int selectedIndex = 1; // 첫 번째(부모)는 제외
        int cumulativeWeight = 0;

        for (int i = 1; i < spawnWeights.Length; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomValue < cumulativeWeight)
            {
                selectedIndex = i;
                break;
            }
        }
        float x = spawnPoints[selectedIndex].position.x + Random.insideUnitCircle.x * spawnRange;
        float z = spawnPoints[selectedIndex].position.z + Random.insideUnitCircle.y * spawnRange;
        Vector3 randomspawn = new Vector3(x, spawnPoints[selectedIndex].position.y, z);
        return randomspawn;
    }

    private void IncreaseWeights()
    {
        weights[0] += increment1; // 첫 번째 프리팹의 가중치를 1씩 증가
        weights[1] += increment2; // 두 번째 프리팹의 가중치를 2씩 증가
        // 세 번째 프리팹의 가중치는 그대로 유지
    }

    private GameObject SelectPrefab()
    {
        int totalWeight = 0;

        // 전체 가중치 합 계산
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        // 랜덤 값 생성
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // 랜덤 값을 가중치에 따라 분배하여 프리팹 선택
        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return zombiePrefabs[i];
            }
        }

        // 기본값 (문제가 있을 경우)
        return zombiePrefabs[0];
    }

    private Zombie SpawnZombie(Vector3 _pos)
    {
        //GameObject randomZombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Count)];

        GameObject zombieGo = Instantiate(SelectPrefab(), _pos, Quaternion.identity, transform);


        return zombieGo.GetComponent<Zombie>();
    }

    private bool GetValidZombie(out Zombie _zombie)
    {
        foreach (Zombie zombie in zombieList)
        {
            if (zombie.IsDead)
            {
                _zombie = zombie;
                return true;
            }
        }

        _zombie = null; // out을 사용했으므로 반드시 null로 초기화
        return false;
    }

    private void ZombieClean()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Zombie zombie in zombieList)
            {
                zombie.Die();
            }
        }
    }
}
