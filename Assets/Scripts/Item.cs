using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Coin, SpeedUp, DamageUp, Other }
    public Type type;
    public int value;

}
