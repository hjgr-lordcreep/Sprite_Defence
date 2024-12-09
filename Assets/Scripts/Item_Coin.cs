using UnityEngine;

public class Item_Coin : MonoBehaviour
{
    private Vector3 dir;
    private float acceleration;
    private float velocity;

    public GameObject player;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");  // Player�� �±� "Player"�� �����Ǿ� �־�� ��
        }
    }

    private void Update()
    {
        CoinMove();
    }
    public void CoinMove()
    {
        if (player.CompareTag("Player"))
        {
            dir = (player.transform.position - transform.position).normalized;
            acceleration = 5f;
            velocity = (velocity + acceleration * Time.deltaTime);
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= 5.0f)
            {
                transform.position =
                    new Vector3(transform.position.x + (dir.x * velocity),
                                transform.position.y,
                                transform.position.z + (dir.z * velocity));
            }
            else
            {
                velocity = 0.0f;
            }

        }
    }
}