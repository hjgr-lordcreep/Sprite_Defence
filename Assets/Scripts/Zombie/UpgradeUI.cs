using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerTr = null; // �÷��̾� Transform
    [SerializeField]
    private Canvas upgradeCanvas = null; // UI Canvas
    [SerializeField]
    private float dis = 10f; // ���� �Ÿ�

    [SerializeField]
    private float upAtkDmg = 5f; // ���ݷ� ������

    private TurretAI turret = null; // TurretAI ����

    private static UpgradeUI closestUpgradeUI = null; // ���� ����� UpgradeUI
    private static float closestDistance = float.MaxValue; // ���� ���� ����� �Ÿ�

    private void Start()
    {
        turret = GetComponent<TurretAI>();
    }

    private void Update()
    {
        if (!playerTr.gameObject.activeSelf) return;

        float curdis = Vector3.Distance(transform.position, playerTr.position);

        // ���� ����� UpgradeUI �Ǵ�
        if (curdis < dis)
        {
            if (curdis < closestDistance || closestUpgradeUI == null)
            {
                closestUpgradeUI = this;
                closestDistance = curdis;
            }
        }

        // ���� ����� UpgradeUI�� UI�� ����
        if (this == closestUpgradeUI)
        {
            HandleUI();
        }
    }

    private void HandleUI()
    {
        // UI Ȱ��ȭ �� ��ȣ�ۿ�
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
        // LateUpdate���� ���� ����� UpgradeUI�� �ʱ�ȭ
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

        // ���ݷ� ���׷��̵� ó��
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