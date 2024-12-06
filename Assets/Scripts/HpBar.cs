using UnityEngine;

public class HpBar : MonoBehaviour
{
    private RectTransform frontTr = null;
    private float maxWidth = 0f;


    private void Awake()
    {
        RectTransform[] trs =
            GetComponentsInChildren<RectTransform>();
        frontTr = trs[2];

        maxWidth = frontTr.sizeDelta.x;
    }

    public void UpdateHp(float _curHp, float _maxHp)
    {
        float ratio = (float)_curHp / _maxHp;
        frontTr.sizeDelta = new Vector2(
            maxWidth * ratio,
            frontTr.sizeDelta.y);
    }

    public void UpdatePosition(Vector3 _pos)
    {
        Vector3 worldToScreen =
            Camera.main.WorldToScreenPoint(_pos);
        worldToScreen.y += 50f;
        transform.position = worldToScreen;
    }
}
