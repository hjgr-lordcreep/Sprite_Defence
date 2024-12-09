using System.Collections;
using UnityEngine;

public class Distanceturret : MonoBehaviour
{
    [SerializeField]
    private Transform[] turret;
    [SerializeField]
    private Transform player;


    private IEnumerator distanceCoroutine()
    {
        while (true)
        {
            foreach (Transform t in turret)
            {
                Vector3.Distance(t.position, player.position);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
