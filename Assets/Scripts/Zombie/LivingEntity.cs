using System;
using UnityEngine;

// ����ü�μ� ������ ���� ������Ʈ���� ���� ���븦 ����
// ü��, ������ �޾Ƶ��̱�, ��� ���, ��� �̺�Ʈ�� ����
public class LivingEntity : MonoBehaviour, IDamageable {
    public virtual float startingHealth { get; set; } = 100; // ���� ü��
    public virtual float health { get; set; } // ���� ü��
    public bool IsDead { get { return !gameObject.activeSelf; } set { gameObject.SetActive(value); } } // ��� ����
    //public event Action onDeath; // ����� �ߵ��� �̺�Ʈ

    // ����ü�� Ȱ��ȭ�ɶ� ���¸� ����
    protected virtual void OnEnable() {
        // ������� ���� ���·� ����
        IsDead = true;
        // ü���� ���� ü������ �ʱ�ȭ
        health = startingHealth;
    }

    // �������� �Դ� ���
    public virtual void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitNormal) {
        // ��������ŭ ü�� ����
        health -= damage;

        // ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
        if (health <= 0 && !IsDead)
        {
            Die();
        }
    }

    // ü���� ȸ���ϴ� ���
    public virtual void RestoreHealth(float newHealth) {
        if (IsDead)
        {
            // �̹� ����� ��� ü���� ȸ���� �� ����
            return;
        }

        // ü�� �߰�
        health += newHealth;
    }

    // ��� ó��
    public virtual void Die() {
        // onDeath �̺�Ʈ�� ��ϵ� �޼��尡 �ִٸ� ����
        //if (onDeath != null)
        //{
        //    onDeath();
        //}


        // ��� ���¸� ������ ����
        IsDead = false;
    }
}