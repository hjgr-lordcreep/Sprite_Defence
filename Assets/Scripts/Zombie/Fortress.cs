using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    public override float health { get; set; }
    protected override void OnEnable()
    {
        IsDead = true;
        //startingHealth = fortressData.health;
        health = fortressData.health;
        //health = startingHealth;
        //base.OnEnable();
    }

    //private void Update()
    //{
    //    Debug.Log(health);
    //}


}
