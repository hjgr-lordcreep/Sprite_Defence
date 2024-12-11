using UnityEngine;

public class Player : MonoBehaviour
{
    //Item item;
    int coin;
    int speedUp;
    int damageUp;
    int other;

    UIManager manager;

    //public Vector3 position { get { return transform.position; } }
    public Vector3 position => transform.position;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Coin:
                    UIManager.instance.money += item.value;
                    UIManager.instance.moneyText.text = UIManager.instance.money.ToString();
                    break;
                case Item.Type.SpeedUp:
                    speedUp += item.value;
                    break;
                case Item.Type.DamageUp:
                    damageUp += item.value;
                    break;
                case Item.Type.Other:
                    break;
            }
            Destroy(other.gameObject);
        }
    }
}
