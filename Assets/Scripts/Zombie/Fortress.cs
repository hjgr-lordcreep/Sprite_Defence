using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    public override float startingHealth;
    public override float health;
    protected override void OnEnable()
    {
        IsDead = true;
        startingHealth = fortressData.health;
        health = startingHealth;
    }
}
