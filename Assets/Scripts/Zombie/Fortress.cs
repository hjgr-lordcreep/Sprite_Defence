using System;
using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    protected override float startingHealth { get; set; }
    protected override float health { get; set; }
    protected override void OnEnable()
    {
        IsDead = true;
        startingHealth = fortressData.health;
        health = startingHealth;
        //health = startingHealth;
        //base.OnEnable();
    }

    private void Update()
    {
        Debug.Log(health);
    }


}
