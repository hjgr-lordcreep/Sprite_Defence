using System.Collections;
using UnityEngine;

// ���� �����Ѵ�
[ExecuteInEditMode]
public class Gun : MonoBehaviour
{
    // ���� ���¸� ǥ���ϴµ� ����� Ÿ���� �����Ѵ�
    public enum State
    {
        Ready, // �߻� �غ��
        //Empty, // źâ�� ��
        //Reloading // ������ ��
    }

    public State state { get; private set; } // ���� ���� ����

    public Transform fireTransform; // �Ѿ��� �߻�� ��ġ

    [SerializeField]
    private ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    //public ParticleSystem shellEjectEffect; // ź�� ���� ȿ��

    private LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ������

    private AudioSource gunAudioPlayer; // �� �Ҹ� �����

    public GunData gunData; // ���� ���� ������

    public Light muzzleLight = null;

    [SerializeField]
    private float fireDistance = 50f; // �����Ÿ�

    public int ammoRemain = 100; // ���� ��ü ź��
    public int magAmmo; // ���� źâ�� �����ִ� ź��

    private float lastFireTime; // ���� ���������� �߻��� ����

    private float knockBack = 0.07f;

    private void Awake()
    {
        // ����� ������Ʈ���� ������ ��������
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� �������� ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        // ��ü ���� ź�� ���� �ʱ�ȭ
        ammoRemain = gunData.startAmmoRemain;
        // ���� źâ�� ����ä���
        magAmmo = gunData.magCapacity;

        // ���� ���� ���¸� ���� �� �غ� �� ���·� ����
        state = State.Ready;
        // ���������� ���� �� ������ �ʱ�ȭ
        lastFireTime = 0;
    }

    private void Update()
    {
        Debug.DrawRay(fireTransform.position, fireTransform.forward * fireDistance, Color.red);
    }

    // �߻� �õ�
    public void Fire()
    {
        // ���� ���°� �߻� ������ ����
        // && ������ �� �߻� �������� timeBetFire �̻��� �ð��� ����
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            // ������ �� �߻� ������ ����
            lastFireTime = Time.time;
            // ���� �߻� ó�� ����
            Shot();
        }
    }

    // ���� �߻� ó��
    private void Shot()
    {
        // ����ĳ��Ʈ�� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit hit;
        // �Ѿ��� ���� ���� ������ ����
        Vector3 hitPosition = Vector3.zero;

        // ����ĳ��Ʈ(��������, ����, �浹 ���� �����̳�, �����Ÿ�)
        if (Physics.Raycast(fireTransform.position,
            fireTransform.forward, out hit, fireDistance, 
            1 << LayerMask.NameToLayer("Zombie")|
            1<<LayerMask.NameToLayer("Fortress")))
        {
            // ���̰� � ��ü�� �浹�� ���

            // �浹�� �������κ��� IDamageable ������Ʈ�� �������� �õ�
            IDamageable target =
                hit.collider.GetComponent<IDamageable>();

            Debug.Log(hit.collider.name);

            // �������� ���� IDamageable ������Ʈ�� �������µ� �����ߴٸ�
            if (target != null && !hit.collider.CompareTag("Fortress"))
            {
                // ���� Ÿ���� ���
                gunAudioPlayer.PlayOneShot(gunData.zombieHitClip);
                // ������ OnDamage �Լ��� ������Ѽ� ���濡�� ������ �ֱ�
                target.OnDamage(gunData.damage, hit.point, hit.normal);
                // ���̰� �浹�� ��ġ ����
                Vector3 dir = hit.transform.position - transform.position;
                //Vector3 knockBackPos = other.transform.position * (-dir.normalized * knockBack);
                Vector3 knockBackPos = hit.transform.position + (dir.normalized * knockBack);
                knockBackPos.y = 0;
                hit.transform.position = knockBackPos;
            }
            else
            {
                Debug.Log("No hit detected");
            }

            
            hitPosition = hit.point;
        }
        else
        {
            // ���̰� �ٸ� ��ü�� �浹���� �ʾҴٸ�
            // �Ѿ��� �ִ� �����Ÿ����� ���ư������� ��ġ�� �浹 ��ġ�� ���
            hitPosition = fireTransform.position +
                          fireTransform.forward * fireDistance;
        }

        // �߻� ����Ʈ ��� ����
        StartCoroutine(ShotEffect(hitPosition));

        //// ���� źȯ�� ���� -1
        //magAmmo--;
        //if (magAmmo <= 0)
        //{
        //    // źâ�� ���� ź���� ���ٸ�, ���� ���� ���¸� Empty���� ����
        //    state = State.Empty;
        //}
    }

    // �߻� ����Ʈ�� �Ҹ��� ����ϰ� �Ѿ� ������ �׸���
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        //// �ѱ� ȭ�� ȿ�� ���
        muzzleFlashEffect.Play();
        muzzleLight.enabled = true;
        //// ź�� ���� ȿ�� ���
        //shellEjectEffect.Play();

        //// �Ѱ� �Ҹ� ���
        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        // ���� �������� �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // ���� ������ �Է����� ���� �浹 ��ġ
        bulletLineRenderer.SetPosition(1, hitPosition);
        // ���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���
        bulletLineRenderer.enabled = true;

        // �Ѿ� ���� ����
        float duration = 0.1f;
        float fadeSpeed = 1.0f / duration;
        float alpha = 1.0f;

        while (alpha > 0.0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            Color startColor = new Color(1, 1, 1, alpha);
            bulletLineRenderer.startColor = startColor;
            bulletLineRenderer.endColor = startColor;
            yield return null;
        }

        bulletLineRenderer.enabled = false;
        muzzleLight.enabled = false;

        //// 0.03�� ���� ��� ó���� ���
        //yield return new WaitForSeconds(0.03f);
        //
        //// ���� �������� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����
        //bulletLineRenderer.enabled = false;
        //StopAllCoroutines();
        //muzzleLight.enabled = false;
    }

    //// ������ �õ�
    //public bool Reload()
    //{
    //    if (state == State.Reloading ||
    //        ammoRemain <= 0 || magAmmo >= gunData.magCapacity)
    //    {
    //        // �̹� ������ ���̰ų�, ���� �Ѿ��� ���ų�
    //        // źâ�� �Ѿ��� �̹� ������ ��� ������ �Ҽ� ����
    //        return false;
    //    }

    //    // ������ ó�� ����
    //    StartCoroutine(ReloadRoutine());
    //    return true;
    //}

    //// ���� ������ ó���� ����
    //private IEnumerator ReloadRoutine()
    //{
    //    // ���� ���¸� ������ �� ���·� ��ȯ
    //    state = State.Reloading;
    //    // ������ �Ҹ� ���
    //    gunAudioPlayer.PlayOneShot(gunData.reloadClip);

    //    // ������ �ҿ� �ð� ��ŭ ó���� ����
    //    yield return new WaitForSeconds(gunData.reloadTime);

    //    // źâ�� ä�� ź���� ����Ѵ�
    //    int ammoToFill = gunData.magCapacity - magAmmo;

    //    // źâ�� ä������ ź���� ���� ź�ຸ�� ���ٸ�,
    //    // ä������ ź�� ���� ���� ź�� ���� ���� ���δ�
    //    if (ammoRemain < ammoToFill)
    //    {
    //        ammoToFill = ammoRemain;
    //    }

    //    // źâ�� ä���
    //    magAmmo += ammoToFill;
    //    // ���� ź�࿡��, źâ�� ä�ŭ ź���� �A��
    //    ammoRemain -= ammoToFill;

    //    // ���� ���� ���¸� �߻� �غ�� ���·� ����
    //    state = State.Ready;
    //}
}