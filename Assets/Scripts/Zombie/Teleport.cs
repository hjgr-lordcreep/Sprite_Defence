using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Transform TeleportTr = null;
    private BoxCollider boxCol = null;

    private void Start()
    {
        boxCol = GetComponent<BoxCollider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = TeleportTr.transform.position;
    }

}
