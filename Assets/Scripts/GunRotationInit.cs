using UnityEngine;

public class MaintainWeaponPosition : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        // �ʱ� Transform ����
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        // �ʱ� Transform���� ���� ����
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }
}
