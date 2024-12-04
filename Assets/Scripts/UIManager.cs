using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    private float surviveTime;
    private bool isDead;

    private void Start()
    {
        surviveTime = 0;
        isDead = false;

        
    }
    private void Update()
    {
        if (!isDead)
        {
            surviveTime += Time.deltaTime;
            timeText.text = "Time : " + surviveTime.ToString("F1");
        }

        //������
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

}
