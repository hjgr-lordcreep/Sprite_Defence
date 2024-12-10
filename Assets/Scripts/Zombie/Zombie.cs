using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, ������̼� �ý��� ���� �ڵ带 ��������

// ���� AI ����
public class Zombie : LivingEntity
{
    public GameObject coin = null;

    public GameObject HealthBar = null;

    public LayerMask whatIsTarget; // ���� ��� ���̾�

    private LivingEntity targetEntity; // ������ ���
    private NavMeshAgent navMeshAgent; // ��ΰ�� AI ������Ʈ

    public ParticleSystem hitEffect; // �ǰݽ� ����� ��ƼŬ ȿ��
    //public AudioClip deathSound; // ����� ����� �Ҹ�
    //public AudioClip hitSound; // �ǰݽ� ����� �Ҹ�

    private Animator zombieAnimator; // �ִϸ����� ������Ʈ
    //private AudioSource zombieAudioPlayer; // ����� �ҽ� ������Ʈ
    //private Renderer zombieRenderer; // ������ ������Ʈ


    public ZombieData zombieData = null;

    private float senseRange; // �ν� ����
    private float damage; // ���ݷ�
    public float timeBetAttack = 0.5f; // ���� ����
    private float lastAttackTime; // ������ ���� ����

    // ���� Ȱ��ȭ/��Ȱ��ȭ �̺�Ʈ
    public static event System.Action OnZombieEnabled;
    public static event System.Action OnZombieDisabled;

    private Coroutine pathCoroutine;
    private Collider zombieCol = null;

    // ���� Ȱ��ȭ�Ǹ� ���� ��Ƽ�� ī���Ͱ� �����ϴ� �̺�Ʈ ����
    protected override void OnEnable()
    {
        // LivingEntity���� �̹� �����Ǿ��ֱ� ������
        // Invoke�� �߰��� ����ǰ� �������̵� ��.
        base.OnEnable();
        OnZombieEnabled?.Invoke();
        zombieCol.enabled = true;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;

        Setup(zombieData);
        // �ڷ�ƾ�� �̹� ���� ���̸� ����
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
        }

        // �ڷ�ƾ�� �ٽ� ����
        pathCoroutine = StartCoroutine(UpdatePath());
    }

    // ���� ��Ȱ��ȭ�Ǹ� ���� ��Ƽ�� ī���Ͱ� �����ϴ� �̺�Ʈ ����
    private void OnDisable()
    {
        // �ڷ�ƾ ����
        //if (pathCoroutine != null)
        //{
        //    StopCoroutine(pathCoroutine);
        //}

        OnZombieDisabled?.Invoke();
        //targetEntity = null;
    }

    // ������ ����� �����ϴ��� �˷��ִ� ������Ƽ
    private bool hasTarget
    {
        get
        {
            // ������ ����� �����ϰ�, ����� ������� �ʾҴٸ� true
            if (targetEntity != null && !targetEntity.IsDead)
            {
                return true;
            }

            // �׷��� �ʴٸ� false
            return false;
        }
    }



    private void Awake()
    {
        // ���� ������Ʈ�κ��� ����� ������Ʈ���� ��������
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieCol = GetComponent<Collider>();
        //zombieAudioPlayer = GetComponent<AudioSource>();

        // ������ ������Ʈ�� �ڽ� ���� ������Ʈ���� �����Ƿ�
        // GetComponentInChildren() �޼��带 ���
        //zombieRenderer = GetComponentInChildren<Renderer>();
    }

    public void Init(Vector3 _pos)
    {
        transform.position = _pos;

        gameObject.SetActive(true);

    }

    // ���� AI�� �ʱ� ������ �����ϴ� �¾� �޼���
    public void Setup(ZombieData zombieData)
    {
        // ü�� ����
        startingHealth = zombieData.health;
        health = zombieData.damage;
        // ���ݷ� ����
        damage = zombieData.damage;
        // ����޽� ������Ʈ�� �̵� �ӵ� ����
        navMeshAgent.speed = zombieData.speed;
        // �ν� ���� ����
        senseRange = zombieData.senseRange;
        // �������� ������� ���׸����� �÷��� ����, ���� ���� ����
        //zombieRenderer.material.color = zombieData.skinColor;
    }

    private void Start()
    {
        // ���� ������Ʈ Ȱ��ȭ�� ���ÿ� AI�� ���� ��ƾ ����
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        // ���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼��� ���
        //zombieAnimator.SetBool("HasTarget", hasTarget);
    }


    private IEnumerator UpdatePath()
    {
        while (!IsDead)
        {
            // ���� ����� LivingEntity�� ã�� ���� ���� �ʱ�ȭ
            float closestDistance = Mathf.Infinity;
            LivingEntity closestLiving = null;

            // �ֺ� �ݶ��̴� Ž��
            Collider[] colliders = Physics.OverlapSphere(transform.position, senseRange, whatIsTarget);

            for (int i = 0; i < colliders.Length; i++)
            {
                // Fortress �Ǵ� Player �±װ� �ƴϸ� �ǳʶ�
                if (!colliders[i].CompareTag("Fortress") && !colliders[i].CompareTag("Player"))
                    continue;

                // LivingEntity ������Ʈ ��������
                LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();

                // ��� �ִ� ��ƼƼ���� Ȯ��
                if (livingEntity != null && !livingEntity.IsDead)
                {
                    // ���� ��ƼƼ���� �Ÿ� ���
                    float distance = Vector3.Distance(transform.position, colliders[i].transform.position);

                    // �� ����� LivingEntity�� �߰��ϸ� ����
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestLiving = livingEntity;
                    }
                }
            }

            // ���� ����� LivingEntity�� ���ο� ���� ������� ����
            targetEntity = closestLiving; // targetEntity ����

            // ���� ����� �����ϸ� ��� ����
            if (hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                navMeshAgent.isStopped = true; // �̵� ����
            }

            // 0.25�� �ֱ�� �ݺ�
            yield return new WaitForSeconds(0.25f);
        }
    }
    // �ֱ������� ������ ����� ��ġ�� ã�� ��θ� ����
    //private IEnumerator UpdatePath()
    //{
    //    // ����ִ� ���� ���� ����
    //    while (!IsDead)
    //    {
    //        if (hasTarget)
    //        {
    //            // ���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
    //            navMeshAgent.isStopped = false;
    //            navMeshAgent.SetDestination(
    //                targetEntity.transform.position);
    //        }
    //        else
    //        {
    //            // ���� ��� ���� : AI �̵� ����
    //            navMeshAgent.isStopped = true;

    //            // 20 ������ �������� ���� ������ ���� �׷�����, ���� ��ġ�� ��� �ݶ��̴��� ������
    //            // ��, whatIsTarget ���̾ ���� �ݶ��̴��� ���������� ���͸�
    //            Collider[] colliders =
    //                Physics.OverlapSphere(transform.position, senseRange, whatIsTarget);
    //            float distAway = Mathf.Infinity;
    //            LivingEntity closestLiving = null;

    //            // ��� �ݶ��̴����� ��ȸ�ϸ鼭, ����ִ� LivingEntity ã��
    //            for (int i = 0; i < colliders.Length; i++)
    //            {
    //                if (!colliders[i].CompareTag("Fortress") && !colliders[i].CompareTag("Player"))
    //                {
    //                    continue;
    //                }
    //                LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
    //                if (livingEntity != null && !livingEntity.IsDead)
    //                {
    //                    float dist = Vector3.Distance(transform.position, colliders[i].transform.position);
    //                    if (dist < distAway)
    //                    {
    //                        distAway = dist;
    //                        closestLiving = livingEntity;
    //                    }
    //                }
    //            }

    //            if (closestLiving != null) 
    //            {
    //                targetEntity = closestLiving;
    //            }
    //        }

    //        // 0.25�� �ֱ�� ó�� �ݺ�
    //        yield return new WaitForSeconds(0.25f);
    //    }
    //}

    // �������� �Ծ����� ������ ó��
    public override void OnDamage(float damage,
        Vector3 hitPoint, Vector3 hitNormal)
    {
        //// ���� ������� ���� ��쿡�� �ǰ� ȿ�� ���
        if (!IsDead)
        {
            // ���� ���� ������ �������� ��ƼŬ ȿ���� ���
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation
                = Quaternion.LookRotation(hitNormal);
            if(!hitEffect.isPlaying)
                hitEffect.Play();

            //    // �ǰ� ȿ���� ���
            //    zombieAudioPlayer.PlayOneShot(hitSound);
            //}

            // LivingEntity�� OnDamage()�� �����Ͽ� ������ ����
            base.OnDamage(damage, hitPoint, hitNormal);
        }
    }

    // ��� ó��
    public override void Die()
    {
        // ��� �ִϸ��̼� ���
        zombieAnimator.SetTrigger("Die");
        // �� 3�� �� ���� ��� ��ũ��Ʈ �۵�
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
        }
        
        zombieCol.enabled = false;  
        navMeshAgent.isStopped = true;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.velocity = Vector3.zero;

        Invoke("DelayedDie", 2f);
    }

    private void DelayedDie()
    {
        // LivingEntity�� Die()�� �����Ͽ� �⺻ ��� ó�� ����
        base.Die();
        UIManager.instance.kill++;
        UIManager.instance.killText.text = "Kill: " + UIManager.instance.kill.ToString();
        Instantiate(coin, transform.position, Quaternion.Euler(90, 0, 0));

        // �ٸ� AI���� �������� �ʵ��� �ڽ��� ��� �ݶ��̴����� ��Ȱ��ȭ
        //Collider[] zombieColliders = GetComponents<Collider>();
        //for (int i = 0; i < zombieColliders.Length; i++)
        //{
        //    zombieColliders[i].enabled = false;
        //}

        // AI ������ �����ϰ� ����޽� ������Ʈ�� ��Ȱ��ȭ
        //navMeshAgent.isStopped = true;
        //navMeshAgent.enabled = false;

        //// ��� ȿ���� ���
        //zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        // �ڽ��� ������� �ʾ�����,
        // �ֱ� ���� �������� timeBetAttack �̻� �ð��� �����ٸ� ���� ����
        if (!IsDead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // �������κ��� LivingEntity Ÿ���� �������� �õ�
            LivingEntity attackTarget
                = other.GetComponent<LivingEntity>();

            // ������ LivingEntity�� �ڽ��� ���� ����̶�� ���� ����
            if (attackTarget != null && attackTarget == targetEntity)
            {
                // ���� �ִϸ��̼� On
                zombieAnimator.SetBool("Attack", true);

                // �ֱ� ���� �ð��� ����
                lastAttackTime = Time.time;

                // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint
                    = other.ClosestPoint(transform.position);
                Vector3 hitNormal
                    = transform.position - other.transform.position;

                // ���� ����
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
                
                // UI������Ʈ, ����ü��
                UIManager.instance.FortressHPUpdate();
            }

            // ���� �ִϸ��̼� Off
            zombieAnimator.SetBool("Attack", false);
        }
    }
}

//using UnityEngine;

//public class Zombie : MonoBehaviour, IDamageable
//{
//    private float MaxHP = 100;
//    private float currentHP;

//    public bool IsDead { get {  return !gameObject.activeSelf; } }

//    private void Start()
//    {
//        currentHP = MaxHP;
//    }

//    public void Init(Vector3 _pos)
//    {
//        transform.position = _pos;

//        gameObject.SetActive(true);
//    }

//    // ��Ȱ��ȭ�� ����
//    // ������ �ִϸ��̼ǰ� �����ϸ� �״� �ִϸ��̼� �߰��� ��
//    // ��Ȱ��ȭ �ǵ��� �ٲ�� ��
//    public void Dead()
//    {
//        gameObject.SetActive(false);
//    }

//    public void OnDamage(float damage)
//    {
//        currentHP -= damage;
//        if (currentHP <= 0 && !IsDead)
//        {
//            Dead();
//        }
//    }
//}
