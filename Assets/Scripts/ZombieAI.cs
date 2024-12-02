using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent zbAgent = null;
    [SerializeField]
    private Transform player = null;

    private void Awake()
    {
        zbAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    { 
        //if (player != null)
        //{
            zbAgent.SetDestination(player.position);
        //}
    }
}
