using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    private int curs = 1;

    [SerializeField]
    private GameObject cursor;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (curs == 1)
            {
                cursor.transform.position = new Vector3(-5.75f, -11.6f, 37.2f);
                curs = 2;
            }
            else if (curs == 2)
            {
                cursor.transform.position = new Vector3(-5.75f, -2.6f, 37.2f);
                curs = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (curs == 1)
            {
                cursor.transform.position = new Vector3(-5.75f, -11.6f, 37.2f);
                curs = 2;
            }
            else if (curs == 2)
            {
                cursor.transform.position = new Vector3(-5.75f, -2.6f, 37.2f);
                curs = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (curs == 1)
            {
                SceneManager.LoadScene("TestWorld");
            }
            else if (curs == 2)
            {
                Debug.Log("QUIT");
                Application.Quit();
            }
        }
    }
}
