using UnityEngine;

public interface IDamageable
{
    // 히트포인트와 히트노멀은 총알의 발사방식에 따라 필요할수도 아닐 수도 있음
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
    //void OnDamage(float damage);
}
