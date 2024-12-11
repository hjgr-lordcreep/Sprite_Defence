using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerTr = null; // �÷��̾� Transform
    [SerializeField]
    private Fortress fortress = null;

    [SerializeField]
    private Canvas upgradeCanvas = null; // UI Canvas
    [SerializeField]
    private Image upgradeImage = null;
    [SerializeField]
    private TextMeshProUGUI upgradeText = null;
    [SerializeField]
    private UIManager uiManager = null;
    [SerializeField]
    private Slider dmgSlider = null;
    [SerializeField]
    private Slider rpmSlider = null;
    [SerializeField]
    private Slider rangeSlider = null;


    [SerializeField]
    private float upAtkDmg = 5f; // ���ݷ� ������
    [SerializeField]
    private float dis = 10f; // ���� �Ÿ�

    private TurretAI turret = null; // TurretAI ����

    [SerializeField]
    private TurretAI[] turretArray;

    [Header("[Button Array]")]
    [SerializeField]
    private TextMeshProUGUI[] buttonTextArray;


    private static UpgradeUI closestUpgradeUI = null; // ���� ����� UpgradeUI
    private static float closestDistance = float.MaxValue; // ���� ���� ����� �Ÿ�

    private float maxDmg = 100f;
    private float maxRPM = 0.1f;
    private float maxRange = 50f;

    private int dmgMoney = 500;
    private int rpmMoney = 1000;
    private int rangeMoney = 2000;
    private int repairMoney = 500;

    private void Start()
    {
        turret = GetComponent<TurretAI>();
        dmgSlider.value = Mathf.Lerp(0, 1, turret.AttackDamage / maxDmg);
        rpmSlider.value = Mathf.Lerp(0, 1, maxRPM / turret.RPM);
        rangeSlider.value = Mathf.Lerp(0,1,turret.Range / maxRange);

        buttonTextArray[0].text = dmgMoney + "G";
        buttonTextArray[1].text = rpmMoney + "G";
        buttonTextArray[2].text = rangeMoney + "G";
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
            ManageUpgradeText(curdis < dis);
        }

        // �ؽ�Ʈ ���İ� �ִϸ��̼�
        if (upgradeText.enabled)
        {
            Color currentColor = upgradeText.color;
            currentColor.a = Mathf.Clamp01((Mathf.Cos(Time.time * 3) + 1) / 2); // 0~1 ���� ��
            upgradeText.color = currentColor;
        }
    }

    private void LateUpdate()
    {
        // �÷��̾ Ư�� �Ÿ� ������ ������ �� �ʱ�ȭ
        if (closestUpgradeUI == this && Vector3.Distance(transform.position, playerTr.position) > dis)
        {
            closestUpgradeUI = null;                // ���� ����� UI �ʱ�ȭ
            closestDistance = float.MaxValue;      // �ʱ�ȭ
            upgradeImage.gameObject.SetActive(false); // ���׷��̵� UI ��Ȱ��ȭ
            upgradeText.gameObject.SetActive(false); // ���׷��̵� �ؽ�Ʈ ��Ȱ��ȭ
        }
    }

    private void HandleUI()
    {
        // UI Ȱ��ȭ �� ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.E) && !upgradeImage.gameObject.activeSelf)
        {
            upgradeImage.gameObject.SetActive(true);
            upgradeText.gameObject.SetActive(false); // UI�� Ȱ��ȭ�Ǹ� �ؽ�Ʈ�� ��Ȱ��ȭ
            Debug.Log("Upgrade UI Opened.");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && upgradeImage.gameObject.activeSelf)
        {
            upgradeImage.gameObject.SetActive(false);
            upgradeText.gameObject.SetActive(true); // UI�� ��Ȱ��ȭ�Ǹ� �ؽ�Ʈ�� Ȱ��ȭ
            Debug.Log("Upgrade UI Closed.");
        }
    }

    private void ManageUpgradeText(bool withinDistance)
    {
        // �Ÿ� ���ǿ� ���� �ؽ�Ʈ ���� ����
        if (withinDistance && !upgradeImage.gameObject.activeSelf)
        {
            upgradeText.gameObject.SetActive(true); // �Ÿ� �ȿ� �ְ� UI�� ��Ȱ��ȭ�� �� �ؽ�Ʈ Ȱ��ȭ
        }
        else if (!withinDistance || upgradeImage.gameObject.activeSelf)
        {
            upgradeText.gameObject.SetActive(false); // �Ÿ� ���̰ų� UI�� Ȱ��ȭ�� �� �ؽ�Ʈ ��Ȱ��ȭ
        }
    }

    public void ExitUI()
    {
        upgradeImage.gameObject.SetActive(false);
        upgradeText.gameObject.SetActive(true);
    }

    public void UpgradeAttackDmg()
    {
        if (maxDmg <= turret.AttackDamage) return;
        if (UIManager.instance.money <= dmgMoney) return;

        foreach (TurretAI turret in turretArray)
        {
            turret.AttackDamage += upAtkDmg;
        }

        UIManager.instance.money -= dmgMoney;
        UIManager.instance.moneyText.text = "money: " + UIManager.instance.money.ToString();

        dmgSlider.value = Mathf.Lerp(0, 1, turret.AttackDamage / maxDmg);

        Debug.Log("Upgraded Attack Damage: " + turret.AttackDamage);
    }

    public void UpgradeRPM()
    {
        if (maxRPM >= turret.RPM) return;
        if (UIManager.instance.money <= rpmMoney) return;

        foreach (TurretAI turret in turretArray)
        {
            turret.RPM -= 0.01f;
        }

        UIManager.instance.money -= rpmMoney;
        UIManager.instance.moneyText.text = "money: " + UIManager.instance.money.ToString();

        rpmSlider.value = Mathf.Lerp(0, 1, maxRPM / turret.RPM);

        Debug.Log("Upgraded RPM: " + turret.RPM);
    }

    public void UpgradeRange()
    {
        if (maxRange <= turret.Range) return;
        if (UIManager.instance.money <= rangeMoney) return;

        foreach (TurretAI turret in turretArray)
        {
            turret.Range += 1f;
            turret.InitRange();
        }

        UIManager.instance.money -= rangeMoney;
        UIManager.instance.moneyText.text = "money: " + UIManager.instance.money.ToString();

        rangeSlider.value = Mathf.Lerp(0, 1, turret.Range / maxRange);

        Debug.Log("Upgraded Range: " + turret.Range);
    }

    public void Repair()
    {
        if (UIManager.instance.money <= repairMoney) return;
        UIManager.instance.money -= repairMoney;
        UIManager.instance.moneyText.text = "money: " + UIManager.instance.money.ToString();
        fortress.Repair();
        Debug.Log(fortress.health);
        UIManager.instance.FortressHPUpdate();
    }
}