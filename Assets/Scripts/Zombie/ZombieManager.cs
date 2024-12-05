using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField]
    private GameObject zombiePrefab = null;

    [SerializeField]
    private List<Zombie> zombieList = null;
    [SerializeField]
    private int MaxSpawnCount = 30;

    [SerializeField]
    private int highWeightValue = 200; // ���� ����ġ ��
    [SerializeField]
    private int lowWeightValue = 10; // ���� ����ġ ��
    [SerializeField]
    private float spawntime = 0.1f;

    private IEnumerator asd = null;
    private bool activeCoroutine = false;

    private Zombie zombie = null;
    private int activeZombie;
    private Transform[] spawnPoints = null;
    private int[] spawnWeights;
    private int totalWeight;

    float timer;

    public int ActiveZombieCount { get; private set; }

    // ���� �Ŵ����� Ȱ��ȭ �� �� ���� ī��Ʈ +/- �̺�Ʈ ���
    private void OnEnable()
    {
        Zombie.OnZombieEnabled += IncrementZombieCount;
        Zombie.OnZombieDisabled += DecrementZombieCount;
    }
    // ���� �Ŵ����� ��Ȱ��ȭ �� �� ���� ī��Ʈ +/- �̺�Ʈ ����
    private void OnDisable()
    {
        Zombie.OnZombieEnabled -= IncrementZombieCount;
        Zombie.OnZombieDisabled -= DecrementZombieCount;
    }

    // ��Ƽ�� ���� ī��Ʈ ���� �Լ�
    private void IncrementZombieCount()
    {
        ActiveZombieCount++;
        //Debug.Log("Active Zombies: " + ActiveZombieCount);
    }

    // ��Ƽ�� ���� ī��Ʈ ���� �Լ�
    private void DecrementZombieCount()
    {
        // ��Ƽ�� ���� ī��Ʈ�� 0���Ϸ� �������� ����
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

        if (ActiveZombieCount < MaxSpawnCount / 3.0f && activeCoroutine == false)
        {
            activeCoroutine = true;
            InitWeights(); // ����ġ �ʱ�ȭ �Լ� ȣ��
            asd = PoolingCoroutine();
            StartCoroutine(asd);
            //StartCoroutine(PoolingCoroutine());
        }

    }

    // ���� ������ �� ���� ��ġ�� ����ġ�� �����ϴ� �Լ�
    // ����ġ�� ���� ���� ���� Ȯ���� ����
    private void InitWeights()
    {
        spawnWeights = new int[spawnPoints.Length];

        // �������� �ϳ��� ����Ʈ�� ���� ����ġ ����
        int highWeightIndex = Random.Range(1, spawnPoints.Length); // ù ��°(�θ�) ����

        for (int i = 1; i < spawnPoints.Length; i++) // ù ��° ����
        {
            spawnWeights[i] = (i == highWeightIndex) ? highWeightValue : lowWeightValue;
        }

        // ����ġ �հ� ���
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
    //            // ��ġ�� ����ġ ������� ����
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

    // ������Ʈ Ǯ���� �̿��ؼ� �ִ� ���� ���� �����̸� �ָ鼭 �ϳ��� ������
    private IEnumerator PoolingCoroutine()
    {
        for (int i = 0; i < MaxSpawnCount; i++)
        {
            //if (timer < 0.2f) return;
            yield return new WaitForSeconds(spawntime);
            zombie = null;
            if (GetValidZombie(out zombie))
            {
                // ��ġ�� ����ġ ������� ����
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
        // ������ �����Ǵ� ������ �ִ� ���� 5�ø�
        MaxSpawnCount += 5;
    }

    private Vector3 SelectSpawnPoint()
    {
        // ���� �� ���� (0 ~ totalWeight - 1)
        int randomValue = Random.Range(0, totalWeight);

        // ���� ���� ���� ���� ����Ʈ ����
        int selectedIndex = 1; // ù ��°(�θ�)�� ����
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

        return spawnPoints[selectedIndex].position;
    }

    private Zombie SpawnZombie(Vector3 _pos)
    {
        GameObject zombieGo = Instantiate(zombiePrefab, _pos, Quaternion.identity, transform);

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

        _zombie = null; // out�� ��������Ƿ� �ݵ�� null�� �ʱ�ȭ
        return false;
    }

    private void ZombieClean()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Zombie zombie in zombieList)
            {
                zombie.Die();
                //--activeZombie;
            }
        }
    }
}