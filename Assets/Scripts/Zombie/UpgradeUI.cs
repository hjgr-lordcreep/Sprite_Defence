using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerTr = null; // 플레이어 Transform
    [SerializeField]
    private Canvas upgradeCanvas = null; // UI Canvas
    [SerializeField]
    private float dis = 10f; // 반응 거리

    [SerializeField]
    private float upAtkDmg = 5f; // 공격력 증가량

    private TurretAI turret = null; // TurretAI 참조

    private static UpgradeUI closestUpgradeUI = null; // 가장 가까운 UpgradeUI
    private static float closestDistance = float.MaxValue; // 현재 가장 가까운 거리

    private void Start()
    {
        turret = GetComponent<TurretAI>();
    }

    private void Update()
    {
        if (!playerTr.gameObject.activeSelf) return;

        float curdis = Vector3.Distance(transform.position, playerTr.position);

        // 가장 가까운 UpgradeUI 판단
        if (curdis < dis)
        {
            if (curdis < closestDistance || closestUpgradeUI == null)
            {
                closestUpgradeUI = this;
                closestDistance = curdis;
            }
        }

        // 가장 가까운 UpgradeUI만 UI를 관리
        if (this == closestUpgradeUI)
        {
            HandleUI();
        }
    }

    private void HandleUI()
    {
        // UI 활성화 및 상호작용
        if (Input.GetKeyDown(KeyCode.E) && !upgradeCanvas.gameObject.activeSelf)
        {
            upgradeCanvas.gameObject.SetActive(true);
            Debug.Log("Upgrade UI Opened.");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && upgradeCanvas.gameObject.activeSelf)
        {
            upgradeCanvas.gameObject.SetActive(false);
            Debug.Log("Upgrade UI Closed.");
        }
    }

    private void LateUpdate()
    {
        // LateUpdate에서 가장 가까운 UpgradeUI를 초기화
        if (closestUpgradeUI == this && Vector3.Distance(transform.position, playerTr.position) > dis)
        {
            closestUpgradeUI = null;
            closestDistance = float.MaxValue;
            upgradeCanvas.gameObject.SetActive(false);
        }
    }

    public void ExitUI()
    {
        upgradeCanvas.gameObject.SetActive(false);
    }

    public void UpgradeAttackDmg()
    {
        //if (closestUpgradeUI == null)
        //{
        //    Debug.Log("No closest turret available.");
        //    return;
        //}

        //TurretAI closestTurret = closestUpgradeUI.turret;

        //if (closestTurret == null)
        //{
        //    Debug.Log("Turret is Null for the closest UI.");
        //    return;
        //}

        // 공격력 업그레이드 처리
        turret.AttackDamage += upAtkDmg;
        Debug.Log("Upgraded Attack Damage: " + turret.AttackDamage);
    }

    public void UpgradeRPM()
    {
        turret.RPM -= 0.01f;
        Debug.Log("Upgraded RPM: " + turret.RPM);
    }

    public void UpgradeRange()
    {
        turret.Range += 1f;
        turret.InitRange();
        Debug.Log("Upgraded Range: " + turret.Range);
    }
}