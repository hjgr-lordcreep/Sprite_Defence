using UnityEngine;

public interface IDamageable
{
    // ��Ʈ����Ʈ�� ��Ʈ����� �Ѿ��� �߻��Ŀ� ���� �ʿ��Ҽ��� �ƴ� ���� ����
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
    //void OnDamage(float damage);
}
