﻿using System.Collections;
using System.Collections.Generic;
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

    public float AttackDamage { get { return attackDamage; } }

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

    private void CheckForTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, attackDist);
        float distAway = Mathf.Infinity;

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].tag == "Zombie" && colls[i].gameObject.activeSelf)
            {
                float dist = Vector3.Distance(transform.position, colls[i].transform.position);
                if (dist < distAway)
                {
                    currentTarget = colls[i].gameObject;
                    distAway = dist;
                }
            }
            //else
            //{
            //    currentTarget = null;
            //}
        }
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
        Gizmos.DrawWireSphere(transform.position, attackDist);
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
