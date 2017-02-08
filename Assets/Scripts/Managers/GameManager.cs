using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static int score;
    public static float endTime;

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Slider HP;
    [SerializeField]
    private Slider NRG;
    [SerializeField]
    private Text Timer;
    [SerializeField]
    private Image pausescreen;

    [SerializeField]
    private Text GOver;
    [SerializeField]
    private Text PauseText;
    [SerializeField]
    private Text pauseinstruct;
    [SerializeField]
    private Text GOverinstruct;

    //Private Variables
    private bool paused = false;
    private float startTime;
    private bool gameOver;




    // Use this for initialization
    void Start()
    {
        pauseinstruct.gameObject.SetActive(false);
        GOverinstruct.gameObject.SetActive(false);
        GOver.gameObject.SetActive(false);
        PauseText.gameObject.SetActive(false);
        pausescreen.gameObject.SetActive(false);
        gameOver = false;
        Time.timeScale = 1;
        PlayerBehavior.health = 100;
        PlayerBehavior.energy = 100;
        startTime = Time.time;
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Pause();
        wrapUp();
        TimerUpdate();
        ScoreUpdate();
        HP.value = Mathf.Lerp(HP.value, PlayerBehavior.health, .1f);
        NRG.value = Mathf.Lerp(NRG.value, PlayerBehavior.energy, .1f);
    }

    void ScoreUpdate()
    {
        string filler = "00000000";
        if (score >= 10000000)
            filler = "";
        if (score >= 1000000)
            filler = "0";
        if (score >= 100000)
            filler = "00";
        if (score >= 10000)
            filler = "000";
        if (score >= 1000)
            filler = "0000";
        if (score >= 100)
            filler = "00000";
        scoreText.text = filler + score;

    }

    void TimerUpdate()
    {
        float timerUp = Time.time - startTime;
        string minutes = Mathf.Floor(timerUp / 60).ToString("00");
        string seconds = (timerUp % 60).ToString("00");
        Timer.text = minutes + ":" + seconds;
    }



    void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused == false)
            {
                Time.timeScale = 0;
                pausescreen.gameObject.SetActive(true);
                PauseText.gameObject.SetActive(true);
                pauseinstruct.gameObject.SetActive(true);
                paused = true;
            }
            else if (paused == true)
            {
                endTime = Time.time - startTime;
                SceneManager.LoadScene("Scoreboard");
            }


        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (paused == true)
            {
                Time.timeScale = 1;
                pauseinstruct.gameObject.SetActive(false);
                PauseText.gameObject.SetActive(false);
                pausescreen.gameObject.SetActive(false);
                paused = false;


            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (paused == true)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }

    void wrapUp()
    {
        if (PlayerBehavior.health == 0)
        {
            GOverinstruct.gameObject.SetActive(true);
            GOver.gameObject.SetActive(true);
            HP.value = 0;
            gameOver = true;
            Time.timeScale = 0;
            endTime = Time.time - startTime;
            pausescreen.gameObject.SetActive(true);
        }
        if (gameOver == true && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Scoreboard");

        }
        //initiate score screen
    }
}
