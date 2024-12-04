using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class MuzzleController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem muzzleFlash; // 파티클 시스템을 드래그 앤 드롭으로 연결하세요.
    [SerializeField]
    private float flickerTime = 0.05f;

    private float timer;
    private Light gunLight;
    private bool isFlickering = false;

    private void Start()
    {
        timer = flickerTime;
        gunLight = GetComponent<Light>();
        gunLight.enabled = false; // 시작 시 빛을 비활성화합니다.
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
