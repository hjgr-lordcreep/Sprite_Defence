using UnityEngine;

public class Item_Coin : MonoBehaviour
{
    private Vector3 dir;
    private float acceleration;
    private float velocity;

    public GameObject player;


    private float p_position_x;
    private float p_position_z;
    private bool GetplayerPosition()
    {
        p_position_x = player.transform.position.x;
        p_position_z = player.transform.position.z;

        if( Mathf.Abs(p_position_x) > 10 || Mathf.Abs(p_position_z) > 10)
            return true;
        return false;
    }


    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");  // Player가 태그 "Player"로 설정되어 있어야 함
        }
    }

    private void Update()
    {
        if (player == null) return;
        transform.Rotate(Vector3.forward);
        if (GetplayerPosition()) 
            CoinMove();
    }
    public void CoinMove()
    {
        if (player == null) return; // null일 경우 함수 종료

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
