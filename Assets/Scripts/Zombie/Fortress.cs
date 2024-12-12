using System;
using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    [field: NonSerialized]
    public override float startingHealth { get; set; }
    [field: NonSerialized]
    public override float health { get; set; }
    protected override void OnEnable()
    {
        if (startingHealth == fortressData.health) return;
        IsDead = true;
        startingHealth = fortressData.health;
        health = startingHealth;
        //health = startingHealth;
        //base.OnEnable();
    }

    //private void Update()
    //{
    //    transform.position = Vector3.zero;
    //}

    public override void Die()
    {
        UIManager.instance.GameOver();

    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        Debug.Log(health);
    }

    public void Repair()
    {
        if (health >= startingHealth) return;

        health += startingHealth/3;
    }

}
