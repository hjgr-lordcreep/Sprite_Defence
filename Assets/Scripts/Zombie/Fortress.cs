using UnityEngine;

public class Fortress : LivingEntity
{
    [SerializeField]
    private FortressData fortressData = null;
    public void Setup(FortressData Fortressdata)
    {
        startingHealth = Fortressdata.health;
    }
}
