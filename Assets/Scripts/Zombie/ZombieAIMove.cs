using UnityEngine;
using UnityEngine.AI;

public class ZombieAIMove : MonoBehaviour
{
    private NavMeshAgent zbAgent = null;
    private Transform target = null;

    private void Awake()
    {
        zbAgent = GetComponent<NavMeshAgent>();
        //target = GetComponent<Transform>();
    }

    private void Start()
    {
        GameObject targetOb = GameObject.FindWithTag("Player");
        if (targetOb != null)
        {
            target = targetOb.transform;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            zbAgent.SetDestination(target.position);
        }
    }
}
