using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;  // �÷��̾��� Transform
    public Vector3 offset;    // ī�޶�� �÷��̾� ������ ������(������� ��ġ)
    public float smoothSpeed = 0.125f;  // ī�޶� �̵� �ӵ�

    void Awake()
    {
        offset = transform.position - player.position;
    }
    void FixedUpdate()
    {
        // ī�޶� ���� ��ǥ ��ġ
        Vector3 desiredPosition = player.position + offset;

        // �ε巴�� ī�޶� ��ǥ ��ġ�� �̵�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // ī�޶� ��ġ ����
        transform.position = smoothedPosition;
    }
}
