using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public Vector3 offset;    // 카메라와 플레이어 사이의 오프셋(상대적인 위치)
    public float smoothSpeed = 0.125f;  // 카메라 이동 속도

    void Awake()
    {
        offset = transform.position - player.position;
    }
    void FixedUpdate()
    {
        // 카메라가 따라갈 목표 위치
        Vector3 desiredPosition = player.position + offset;

        // 부드럽게 카메라를 목표 위치로 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 카메라 위치 설정
        transform.position = smoothedPosition;
    }
}
