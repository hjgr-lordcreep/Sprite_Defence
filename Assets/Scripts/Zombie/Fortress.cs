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
        IsDead = true;
        startingHealth = fortressData.health;
        health = startingHealth;
        //health = startingHealth;
        //base.OnEnable();
    }

    public override void Die()
    {
        UIManager.instance.GameOver();

    }

}
