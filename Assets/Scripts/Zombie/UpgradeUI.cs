using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerTr = null; // 플레이어 Transform
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
    private float upAtkDmg = 5f; // 공격력 증가량
    [SerializeField]
    private float dis = 10f; // 반응 거리

    private TurretAI turret = null; // TurretAI 참조

    [SerializeField]
    private TurretAI[] turretArray;

    [Header("[Button Array]")]
    [SerializeField]
    private TextMeshProUGUI[] buttonTextArray;


    private static UpgradeUI closestUpgradeUI = null; // 가장 가까운 UpgradeUI
    private static float closestDistance = float.MaxValue; // 현재 가장 가까운 거리

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
            ManageUpgradeText(curdis < dis);
        }

        // 텍스트 알파값 애니메이션
        if (upgradeText.enabled)
        {
            Color currentColor = upgradeText.color;
            currentColor.a = Mathf.Clamp01((Mathf.Cos(Time.time * 3) + 1) / 2); // 0~1 사이 값
            upgradeText.color = currentColor;
        }
    }

    private void LateUpdate()
    {
        // 플레이어가 특정 거리 밖으로 나갔을 때 초기화
        if (closestUpgradeUI == this && Vector3.Distance(transform.position, playerTr.position) > dis)
        {
            closestUpgradeUI = null;                // 가장 가까운 UI 초기화
            closestDistance = float.MaxValue;      // 초기화
            upgradeImage.gameObject.SetActive(false); // 업그레이드 UI 비활성화
            upgradeText.gameObject.SetActive(false); // 업그레이드 텍스트 비활성화
        }
    }

    private void HandleUI()
    {
        // UI 활성화 및 상호작용
        if (Input.GetKeyDown(KeyCode.E) && !upgradeImage.gameObject.activeSelf)
        {
            upgradeImage.gameObject.SetActive(true);
            upgradeText.gameObject.SetActive(false); // UI가 활성화되면 텍스트는 비활성화
            Debug.Log("Upgrade UI Opened.");
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && upgradeImage.gameObject.activeSelf)
        {
            upgradeImage.gameObject.SetActive(false);
            upgradeText.gameObject.SetActive(true); // UI가 비활성화되면 텍스트는 활성화
            Debug.Log("Upgrade UI Closed.");
        }
    }

    private void ManageUpgradeText(bool withinDistance)
    {
        // 거리 조건에 따라 텍스트 상태 변경
        if (withinDistance && !upgradeImage.gameObject.activeSelf)
        {
            upgradeText.gameObject.SetActive(true); // 거리 안에 있고 UI가 비활성화일 때 텍스트 활성화
        }
        else if (!withinDistance || upgradeImage.gameObject.activeSelf)
        {
            upgradeText.gameObject.SetActive(false); // 거리 밖이거나 UI가 활성화일 때 텍스트 비활성화
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