using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject player = null;

    private Vector3 offset;


    private void Awake()
    {
        offset = new Vector3(0f, 1.5f, -3f);
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
