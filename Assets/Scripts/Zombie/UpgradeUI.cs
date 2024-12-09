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
        // 유저가 가까이 왔을 때의 판단 거리 구하기
        float curdis = Vector3.Distance(transform.position, playerTr.position);

        // 일정 거리(curdis)보다 낮아지면 UI가 뜨게 만듦
        if (curdis < dis)
        {
            // 가까이 왔으면 UI로 상호작용 키를 누르라는 표시가 뜸

            // 상호 작용을 누르라는 UI 활성화
            if (Input.GetKeyDown(KeyCode.E) && !upgradeCanvas.gameObject.activeSelf)
            {
                // 업그레이드 UI 활성화
                upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);


            }
            // UI 활성화 된 상태에서 ESC 키 누를시 비활성화
            if (Input.GetKeyDown(KeyCode.Escape) && upgradeCanvas.gameObject.activeSelf)
            {
                upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);
            }
        }
        // UI가 뜬 상태(활성화 되어 있는 상태)에서 일정 거리 이상 다시 멀어지면 UI가 꺼짐
        else if(upgradeCanvas.gameObject.activeSelf)
        {
            upgradeCanvas.gameObject.SetActive(!upgradeCanvas.gameObject.activeSelf);   
        }
        // esc 또는 X 버튼을 누를 경우 UI가 꺼지게 만들어야 함

        
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

        // 돈 정보 가져와야함
        // if 문으로 일정 돈 이상일 때만 누를수 있도록 만듬
        turret.AttackDamage += upAtkDmg;
        Debug.Log("Attack Damage : "+turret.AttackDamage);
    }
}
