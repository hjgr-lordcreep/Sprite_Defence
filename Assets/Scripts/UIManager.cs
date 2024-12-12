using System.Collections;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject player;

    public static UIManager instance;
    public int kill;
    public int money;

    public GameObject startGameUI;
    public GameObject inGameUI;
    public GameObject endGameUI;

    public Slider fortressSlider;
    public Fortress fortressHP;

    private float hpValueRatio;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI MainText;
    public TextMeshProUGUI VictoryText;
    private float surviveTime;
    public bool isLive;

    public float timeout = 180f;

    private void Awake()
    {
        instance = this;
        isLive = false;
        player.gameObject.SetActive(false);
    }



    public void GameStart()
    {
        surviveTime = 0;
        Time.timeScale = 1;
        inGameUI.gameObject.SetActive(true);
        isLive = true;
        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());

    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        inGameUI.gameObject.SetActive(false);
        endGameUI.gameObject.SetActive(true);
        Time.timeScale = 0;


    }


    public void GameRetry()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void FortressHPUpdate()
    {
        hpValueRatio = fortressHP.health / fortressHP.startingHealth;
        fortressSlider.value = Mathf.Lerp(0, 1, hpValueRatio);
    }

    private void Update()
    {
        if (!MainText.enabled) return;
        // Mathf.Sin() ���� 0 ~ 1�� ��ȯ�� ���İ����� ����
        Color currentColor = MainText.color;       // ���� ������ �����ɴϴ�.
        currentColor.a = Mathf.Min(1,(Mathf.Cos(Time.time * 3) + 1)); // ���� ���� ����
        MainText.color = currentColor;            // ����� ������ �ٽ� ����


        if (!isLive) return;
        
        surviveTime += Time.deltaTime;
        timeText.text = "Time : " + surviveTime.ToString("F1");
        //killText.text = "Kill: " + kill.ToString();
        //moneyText.text = "money: " + money.ToString();


        // ���� �ð��� ���� ��� �¸� �ؽ�Ʈ ǥ��
        Victory();

        //hpValueRatio = fortressHP.health / fortressHP.startingHealth;
        //fortressSlider.value = Mathf.Lerp(0, 1, hpValueRatio);

        //Debug.Log(fortressHP.health);
        //Debug.Log(hpValueRatio);

        //if (surviveTime > 5)
        //{
        //    isLive = false;
        //    GameOver();
        //}



        //������
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Retry();
        }
    }

    private void Victory()
    {
        if(surviveTime > timeout)
        {
            VictoryText.gameObject.SetActive(true);
        }
    }

    private void Retry()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Exit()
    {
        Application.Quit();
    }
}
