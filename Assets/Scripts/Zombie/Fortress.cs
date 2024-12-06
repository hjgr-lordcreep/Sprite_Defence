using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    protected override void OnEnable()
    {
        IsDead = true;
        startingHealth = fortressData.health;
        float health = base.health;
        health = startingHealth;
        //base.OnEnable();
    }

    
}
