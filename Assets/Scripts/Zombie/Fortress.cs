using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    protected override void OnEnable()
    {
        startingHealth = fortressData.health;
        base.OnEnable();
    }
}
