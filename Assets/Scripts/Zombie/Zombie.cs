using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

// 좀비 AI 구현
public class Zombie : LivingEntity
{
    public GameObject coin = null;

    public GameObject HealthBar = null;

    public LayerMask whatIsTarget; // 추적 대상 레이어

    private LivingEntity targetEntity; // 추적할 대상
    private NavMeshAgent navMeshAgent; // 경로계산 AI 에이전트

    public ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    //public AudioClip deathSound; // 사망시 재생할 소리
    //public AudioClip hitSound; // 피격시 재생할 소리

    private Animator zombieAnimator; // 애니메이터 컴포넌트
    //private AudioSource zombieAudioPlayer; // 오디오 소스 컴포넌트
    //private Renderer zombieRenderer; // 렌더러 컴포넌트


    public ZombieData zombieData = null;

    private float senseRange; // 인식 범위
    private float damage; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    // 좀비 활성화/비활성화 이벤트
    public static event System.Action OnZombieEnabled;
    public static event System.Action OnZombieDisabled;

    private Coroutine pathCoroutine;
    private Collider zombieCol = null;

    // 좀비가 활성화되면 좀비 액티브 카운터가 증가하는 이벤트 전달
    protected override void OnEnable()
    {
        // LivingEntity에서 이미 구현되어있기 때문에
        // Invoke만 추가로 실행되게 오버라이드 함.
        base.OnEnable();
        OnZombieEnabled?.Invoke();
        zombieCol.enabled = true;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;

        Setup(zombieData);
        // 코루틴이 이미 실행 중이면 중지
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
        }

        // 코루틴을 다시 시작
        pathCoroutine = StartCoroutine(UpdatePath());
    }

    // 좀비가 비활성화되면 좀비 액티브 카운터가 감소하는 이벤트 전달
    private void OnDisable()
    {
        // 코루틴 정지
        //if (pathCoroutine != null)
        //{
        //    StopCoroutine(pathCoroutine);
        //}

        OnZombieDisabled?.Invoke();
        //targetEntity = null;
    }

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.IsDead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }



    private void Awake()
    {
        // 게임 오브젝트로부터 사용할 컴포넌트들을 가져오기
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieCol = GetComponent<Collider>();
        //zombieAudioPlayer = GetComponent<AudioSource>();

        // 렌더러 컴포넌트는 자식 게임 오브젝트에게 있으므로
        // GetComponentInChildren() 메서드를 사용
        //zombieRenderer = GetComponentInChildren<Renderer>();
    }

    public void Init(Vector3 _pos)
    {
        transform.position = _pos;

        gameObject.SetActive(true);

    }

    // 좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(ZombieData zombieData)
    {
        // 체력 설정
        startingHealth = zombieData.health;
        health = zombieData.damage;
        // 공격력 설정
        damage = zombieData.damage;
        // 내비메시 에이전트의 이동 속도 설정
        navMeshAgent.speed = zombieData.speed;
        // 인식 범위 설정
        senseRange = zombieData.senseRange;
        // 렌더러가 사용중인 머테리얼의 컬러를 변경, 외형 색이 변함
        //zombieRenderer.material.color = zombieData.skinColor;
    }

    private void Start()
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        //zombieAnimator.SetBool("HasTarget", hasTarget);
    }


    private IEnumerator UpdatePath()
    {
        while (!IsDead)
        {
            // 가장 가까운 LivingEntity를 찾기 위한 변수 초기화
            float closestDistance = Mathf.Infinity;
            LivingEntity closestLiving = null;

            // 주변 콜라이더 탐색
            Collider[] colliders = Physics.OverlapSphere(transform.position, senseRange, whatIsTarget);

            for (int i = 0; i < colliders.Length; i++)
            {
                // Fortress 또는 Player 태그가 아니면 건너뜀
                if (!colliders[i].CompareTag("Fortress") && !colliders[i].CompareTag("Player"))
                    continue;

                // LivingEntity 컴포넌트 가져오기
                LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();

                // 살아 있는 엔티티인지 확인
                if (livingEntity != null && !livingEntity.IsDead)
                {
                    // 현재 엔티티와의 거리 계산
                    float distance = Vector3.Distance(transform.position, colliders[i].transform.position);

                    // 더 가까운 LivingEntity를 발견하면 갱신
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestLiving = livingEntity;
                    }
                }
            }

            // 가장 가까운 LivingEntity를 새로운 추적 대상으로 설정
            targetEntity = closestLiving; // targetEntity 갱신

            // 추적 대상이 존재하면 계속 추적
            if (hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                navMeshAgent.isStopped = true; // 이동 중지
            }

            // 0.25초 주기로 반복
            yield return new WaitForSeconds(0.25f);
        }
    }
    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    //private IEnumerator UpdatePath()
    //{
    //    // 살아있는 동안 무한 루프
    //    while (!IsDead)
    //    {
    //        if (hasTarget)
    //        {
    //            // 추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
    //            navMeshAgent.isStopped = false;
    //            navMeshAgent.SetDestination(
    //                targetEntity.transform.position);
    //        }
    //        else
    //        {
    //            // 추적 대상 없음 : AI 이동 중지
    //            navMeshAgent.isStopped = true;

    //            // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
    //            // 단, whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
    //            Collider[] colliders =
    //                Physics.OverlapSphere(transform.position, senseRange, whatIsTarget);
    //            float distAway = Mathf.Infinity;
    //            LivingEntity closestLiving = null;

    //            // 모든 콜라이더들을 순회하면서, 살아있는 LivingEntity 찾기
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

    //        // 0.25초 주기로 처리 반복
    //        yield return new WaitForSeconds(0.25f);
    //    }
    //}

    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage,
        Vector3 hitPoint, Vector3 hitNormal)
    {
        //// 아직 사망하지 않은 경우에만 피격 효과 재생
        if (!IsDead)
        {
            // 공격 받은 지점과 방향으로 파티클 효과를 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation
                = Quaternion.LookRotation(hitNormal);
            if(!hitEffect.isPlaying)
                hitEffect.Play();

            //    // 피격 효과음 재생
            //    zombieAudioPlayer.PlayOneShot(hitSound);
            //}

            // LivingEntity의 OnDamage()를 실행하여 데미지 적용
            base.OnDamage(damage, hitPoint, hitNormal);
        }
    }

    // 사망 처리
    public override void Die()
    {
        // 사망 애니메이션 재생
        zombieAnimator.SetTrigger("Die");
        // 약 3초 뒤 실제 사망 스크립트 작동
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
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();
        UIManager.instance.kill++;
        UIManager.instance.killText.text = "Kill: " + UIManager.instance.kill.ToString();
        Instantiate(coin, transform.position, Quaternion.Euler(90, 0, 0));

        // 다른 AI들을 방해하지 않도록 자신의 모든 콜라이더들을 비활성화
        //Collider[] zombieColliders = GetComponents<Collider>();
        //for (int i = 0; i < zombieColliders.Length; i++)
        //{
        //    zombieColliders[i].enabled = false;
        //}

        // AI 추적을 중지하고 내비메쉬 컴포넌트를 비활성화
        //navMeshAgent.isStopped = true;
        //navMeshAgent.enabled = false;

        //// 사망 효과음 재생
        //zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        // 자신이 사망하지 않았으며,
        // 최근 공격 시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
        if (!IsDead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // 상대방으로부터 LivingEntity 타입을 가져오기 시도
            LivingEntity attackTarget
                = other.GetComponent<LivingEntity>();

            // 상대방의 LivingEntity가 자신의 추적 대상이라면 공격 실행
            if (attackTarget != null && attackTarget == targetEntity)
            {
                // 공격 애니메이션 On
                zombieAnimator.SetBool("Attack", true);

                // 최근 공격 시간을 갱신
                lastAttackTime = Time.time;

                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint
                    = other.ClosestPoint(transform.position);
                Vector3 hitNormal
                    = transform.position - other.transform.position;

                // 공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
                
                // UI업데이트, 기지체력
                UIManager.instance.FortressHPUpdate();
            }

            // 공격 애니메이션 Off
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

//    // 비활성화로 만듬
//    // 다음에 애니메이션과 통합하면 죽는 애니메이션 추가한 뒤
//    // 비활성화 되도록 바꿔야 함
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
