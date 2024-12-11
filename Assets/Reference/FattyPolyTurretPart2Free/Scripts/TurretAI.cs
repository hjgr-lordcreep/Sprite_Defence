using UnityEngine;

public class TurretAI : MonoBehaviour {

    public enum TurretType
    {
        Single = 1,
        Dual = 2,
        Catapult = 3,
    }

    public GameObject currentTarget;
    public Transform turnHead;

    public float attackDist = 10.0f;
    public float attackDamage;
    public float shootCoolDown;
    public float checkTargetTime = 0.5f;
    private float timer;
    [SerializeField]
    private float lockSpeed;
    public Transform checkTargetTr = null;
    public Transform rangeTr = null;

    //public Quaternion randomRot;
    public Vector3 randomRot;
    public Animator animator;

    [Header("[Turret Type]")]
    public TurretType turretType = TurretType.Single;

    public Transform muzzleMain;
    public Transform muzzleSub;
    public GameObject muzzleEff;
    public GameObject bullet;
    private bool shootLeft = true;

    private Transform lockOnPos;

    //public TurretShoot_Base shotScript;

    public float AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public float RPM { get { return shootCoolDown; } set { shootCoolDown = value; } }
    public float Range { get { return attackDist; } set { attackDist = value; } }

    private void OnEnable()
    {
        InitRange();
    }

    void Start() {
        InvokeRepeating("CheckForTarget", 0, checkTargetTime);
        //shotScript = GetComponent<TurretShoot_Base>();

        if (transform.GetChild(0).GetComponent<Animator>())
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }

        randomRot = new Vector3(0, Random.Range(0, 359), 0);
    }

    void Update() {
        if (currentTarget != null)
        {
            FollowTarget();

            float currentTargetDist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (currentTargetDist > attackDist)
            {
                currentTarget = null;
            }
        }
        else
        {
            IdleRitate();
        }

        timer += Time.deltaTime;
        if (timer >= shootCoolDown)
        {
            if (currentTarget != null)
            {
                timer = 0;

                if (animator != null)
                {
                    animator.SetTrigger("Fire");
                    ShootTrigger();
                }
                else
                {
                    ShootTrigger();
                }
            }
        }
    }

    public void InitRange()
    {
        rangeTr.localScale = new Vector3(attackDist * 2, attackDist * 2, attackDist * 2);
    }
    private void CheckForTarget()
    {
        Collider[] colls = Physics.OverlapSphere(checkTargetTr.position, attackDist, 1 << LayerMask.NameToLayer("Zombie"));
        float distAway = Mathf.Infinity;

        for (int i = 0; i < colls.Length; i++)
        {
            if (/*colls[i].tag == "Zombie" && */colls[i].gameObject.activeSelf)
            {
                float dist = Vector3.Distance(checkTargetTr.position, colls[i].transform.position);
                //Vector3 directionToCollider = (colls[i].transform.position - checkTargetTr.position).normalized;
                if (dist < distAway /*&& IsInQuarterCircle(directionToCollider, Vector3.forward, Vector3.right)*/)
                {
                    currentTarget = colls[i].gameObject;
                    distAway = dist;
                }
            }

        }
    }

    private bool IsInQuarterCircle(Vector3 targetDirection, Vector3 forward, Vector3 right)
    {
        // 전방(+Z)와 우측(+X) 방향으로 나뉜 1/4 구역 체크
        float forwardDot = Vector3.Dot(targetDirection, forward); // 전방 기준
        float rightDot = Vector3.Dot(targetDirection, right);     // 우측 기준
        return forwardDot > 0 && rightDot > 0; // 전방 우측 1/4
    }

    private void FollowTarget() //todo : smooth rotate
    {
        Vector3 targetDir = currentTarget.transform.position - turnHead.position;
        //targetDir.y = 0;
        //turnHead.forward = targetDir;
        if (turretType == TurretType.Single)
        {
            turnHead.forward = targetDir;
        }
        else
        {
            turnHead.forward = targetDir;
            //turnHead.transform.rotation = Quaternion.RotateTowards(turnHead.rotation, Quaternion.LookRotation(targetDir), lockSpeed * Time.deltaTime);
        }
    }

    private void ShootTrigger()
    {
        //shotScript.Shoot(currentTarget);
        Shoot(currentTarget);
        //Debug.Log("We shoot some stuff!");
    }

    Vector3 CalculateVelocity(Vector3 target, Vector3 origen, float time)
    {
        Vector3 distance = target - origen;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkTargetTr.position, attackDist);
    }

    public void IdleRitate()
    {
        bool refreshRandom = false;

        if (turnHead.rotation != Quaternion.Euler(randomRot))
        {
            turnHead.rotation = Quaternion.RotateTowards(turnHead.transform.rotation, Quaternion.Euler(randomRot), lockSpeed * Time.deltaTime * 0.2f);
        }
        else
        {
            refreshRandom = true;

            if (refreshRandom)
            {

                int randomAngle = Random.Range(0, 359);
                randomRot = new Vector3(0, randomAngle, 0);
                refreshRandom = false;
            }
        }
    }

    public void Shoot(GameObject go)
    {
        if (turretType == TurretType.Catapult)
        {
            lockOnPos = go.transform;
            InstantProjectile().target = lockOnPos;
        }
        else if (turretType == TurretType.Dual)
        {
            if (shootLeft)
            {
                InstantProjectile().target = transform.GetComponent<TurretAI>().currentTarget.transform;
            }
            else
            {
                Instantiate(muzzleEff, muzzleSub.transform.position, muzzleSub.rotation);
                GameObject missleGo = Instantiate(bullet, muzzleSub.transform.position, muzzleSub.rotation);
                missleGo.transform.SetParent(transform);
                Projectile projectile = missleGo.GetComponent<Projectile>();
                projectile.target = transform.GetComponent<TurretAI>().currentTarget.transform;
            }

            shootLeft = !shootLeft;
        }
        else
        {
            InstantProjectile().target = currentTarget.transform;
        }
    }

    private Projectile InstantProjectile()
    {
        Instantiate(muzzleEff, muzzleMain.transform.position, muzzleMain.rotation);
        GameObject missleGo = Instantiate(bullet, muzzleMain.transform.position, muzzleMain.rotation);
        missleGo.transform.SetParent(transform);
        Projectile projectile = missleGo.GetComponent<Projectile>();

        return projectile;
    }
}
