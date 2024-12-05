using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject startGameUI;
    public GameObject inGameUI;
    public GameObject endGameUI;


    public TextMeshProUGUI timeText;
    private float surviveTime;
    private bool isLive;

    private void Awake()
    {
        isLive = false;

    }



    public void GameStart()
    {
        surviveTime = 0;
        Time.timeScale = 1;
        inGameUI.gameObject.SetActive(true);
        isLive = true;

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


    private void Update()
    {
        if (!isLive) return;
        
        surviveTime += Time.deltaTime;
        timeText.text = "Time : " + surviveTime.ToString("F1");
        

        if (surviveTime > 5)
        {
            isLive = false;
            GameOver();
        }
        
        

        //µð¹ö±ë¿ë
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

}
