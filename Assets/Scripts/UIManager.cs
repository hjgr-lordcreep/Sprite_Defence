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
    public GameObject VictoryUI;

    public Slider fortressSlider;
    public Fortress fortressHP;

    private float hpValueRatio;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI MainText;
    private float surviveTime;
    public bool isLive;

    public int timeout = 10;

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
        // Mathf.Sin() 값을 0 ~ 1로 변환해 알파값으로 설정
        Color currentColor = MainText.color;       // 현재 색상을 가져옵니다.
        currentColor.a = Mathf.Min(1,(Mathf.Cos(Time.time * 3) + 1)); // 알파 값만 변경
        MainText.color = currentColor;            // 변경된 색상을 다시 설정


        if (!isLive) return;
        
        surviveTime += Time.deltaTime;
        //Debug.Log(surviveTime);
        //timeout -= surviveTime;
        timeText.text = "Time : " + surviveTime.ToString("F1");
        //killText.text = "Kill: " + kill.ToString();
        //moneyText.text = "money: " + money.ToString();


        // 제한 시간을 넘을 경우 승리 텍스트 표시
        if ((int)surviveTime >= timeout)
        {
            Debug.Log(timeout);
            Victory();
        }

        //hpValueRatio = fortressHP.health / fortressHP.startingHealth;
        //fortressSlider.value = Mathf.Lerp(0, 1, hpValueRatio);

        //Debug.Log(fortressHP.health);
        //Debug.Log(hpValueRatio);

        //if (surviveTime > 5)
        //{
        //    isLive = false;
        //    GameOver();
        //}



        //디버깅용
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Retry();
        }
    }

    private void Victory()
    {
        StartCoroutine(VictoryRoutine());
    }

    IEnumerator VictoryRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(1f);


        inGameUI.gameObject.SetActive(false);
        endGameUI.gameObject.SetActive(false);
        VictoryUI.gameObject.SetActive(true);
        Time.timeScale = 0;


    }


    private void Retry()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
