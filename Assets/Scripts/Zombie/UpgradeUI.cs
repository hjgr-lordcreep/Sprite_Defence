using System.Collections;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField]
    private Transform playerTr = null;
    [SerializeField]
    private Canvas upgradeCanvas = null;
    [SerializeField]
    private float dis = 5f;

    [SerializeField]
    private float upAtkDmg = 5f;

    private TurretAI turret = null;

    private void Start()
    {
        //StartCoroutine(UIdisPlayerCoroutine());
        turret = GetComponent<TurretAI>();
    }

    //private IEnumerator UIdisPlayerCoroutine()
    //{
    //    while (true)
    //    {
    //        UIdisPlayer();
    //        yield return new WaitForSeconds(0.25f);
    //    }
    //}

    private void Update()
    {
        UIdisPlayer();
    }

    private void UIdisPlayer()
    {
        if (!playerTr.gameObject.activeSelf) return;
        // ������ ������ ���� ���� �Ǵ� �Ÿ� ���ϱ�
        float curdis = Vector3.Distance(transform.position, playerTr.position);

        // ���� �Ÿ�(curdis)���� �������� UI�� �߰� ����
        if (curdis < dis)
        {
            // ������ ������ UI�� ��ȣ�ۿ� Ű�� ������� ǥ�ð� ��

            // ��ȣ �ۿ��� ������� UI Ȱ��ȭ
            if (Input.GetKeyDown(KeyCode.E) && !upgradeCanvas.gameObject.activeSelf)
            {
                // ���׷��̵� UI Ȱ��ȭ
                upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);


            }
            // UI Ȱ��ȭ �� ���¿��� ESC Ű ������ ��Ȱ��ȭ
            if (Input.GetKeyDown(KeyCode.Escape) && upgradeCanvas.gameObject.activeSelf)
            {
                upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);
            }
        }
        // UI�� �� ����(Ȱ��ȭ �Ǿ� �ִ� ����)���� ���� �Ÿ� �̻� �ٽ� �־����� UI�� ����
        else if(upgradeCanvas.gameObject.activeSelf)
        {
            upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);   
        }
        // esc �Ǵ� X ��ư�� ���� ��� UI�� ������ ������ ��

        
    }

    public void ExitUI()
    {
        upgradeCanvas.gameObject.SetActive(false);
    }

    public void UpgradeAttackDmg()
    {
        if( turret == null)
        {
            Debug.Log("Turret is Null");
        }

        // �� ���� �����;���
        // if ������ ���� �� �̻��� ���� ������ �ֵ��� ����
        turret.AttackDamage += upAtkDmg;
        Debug.Log("Attack Damage : "+turret.AttackDamage);
    }
}
