using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{

    [SerializeField]
    private Text endScore;

    [SerializeField]
    private Text survived;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        endScore.text = ""+ GameManager.score;
        string minutes = Mathf.Floor(GameManager.endTime / 60).ToString("00");
        string seconds = (GameManager.endTime % 60).ToString("00");
        survived.text = minutes + ":" + seconds;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
