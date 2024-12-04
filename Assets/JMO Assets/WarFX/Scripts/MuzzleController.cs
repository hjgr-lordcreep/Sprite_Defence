using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class MuzzleController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem muzzleFlash; // ��ƼŬ �ý����� �巡�� �� ������� �����ϼ���.
    [SerializeField]
    private float flickerTime = 0.05f;

    private float timer;
    private Light gunLight;
    private bool isFlickering = false;

    private void Start()
    {
        timer = flickerTime;
        gunLight = GetComponent<Light>();
        gunLight.enabled = false; // ���� �� ���� ��Ȱ��ȭ�մϴ�.
    }

    private void Update()
    {
        if (isFlickering)
        {
            if (!muzzleFlash.isPlaying)
            {
                muzzleFlash.Play();
            }
            StartCoroutine("Flicker");
        }
        else
        {
            if (muzzleFlash.isPlaying)
            {
                muzzleFlash.Stop();
            }
            StopCoroutine("Flicker");
            gunLight.enabled = false;
        }
    }

    public void SetFlicker(bool _state)
    {
        isFlickering = _state;
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            gunLight.enabled = !gunLight.enabled;

            do
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            while (timer > 0);
            timer = flickerTime;
        }
    }
}
