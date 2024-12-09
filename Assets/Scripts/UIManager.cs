using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
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
    private float surviveTime;
    public bool isLive;

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
        if (!isLive) return;
        
        surviveTime += Time.deltaTime;
        timeText.text = "Time : " + surviveTime.ToString("F1");
        //killText.text = "Kill: " + kill.ToString();
        //moneyText.text = "money: " + money.ToString();

        
        //hpValueRatio = fortressHP.health / fortressHP.startingHealth;
        //fortressSlider.value = Mathf.Lerp(0, 1, hpValueRatio);

        //Debug.Log(fortressHP.health);
        //Debug.Log(hpValueRatio);

        //if (surviveTime > 5)
        //{
        //    isLive = false;
        //    GameOver();
        //}



        //µð¹ö±ë¿ë
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

}
