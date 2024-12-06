using UnityEngine;

public class MaintainWeaponPosition : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        // 초기 Transform 저장
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        // 초기 Transform으로 강제 고정
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }
}
