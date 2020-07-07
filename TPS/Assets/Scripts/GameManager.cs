using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    static GameManager m_Instance;

    GameObject gm;
    public static GameManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public WaveSpawner waveSpawner;

    public GameObject quit;
    public GameObject endGameCanvas;

    public PlayerContorller pc;

    public PlayerStates playerStates;

    private bool quitActive = true;
    private int tscale = 0;

    void Start()
    {
        m_Instance = new GameManager();
        m_Instance.gm = GameObject.FindGameObjectWithTag("GameManager");
        m_Instance.waveSpawner = m_Instance.gm.GetComponent<WaveSpawner>();
        m_Instance.playerStates = m_Instance.gm.GetComponent<PlayerStates>();
        quit = GameObject.FindGameObjectWithTag("QuitButton");
        quit.SetActive(false);
        endGameCanvas = GameObject.FindGameObjectWithTag("EndGame");
        endGameCanvas.SetActive(false);
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContorller>();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quit.SetActive(quitActive);
            Cursor.visible =  quitActive;
            Time.timeScale = tscale;
            quitActive = !quitActive;
            tscale = (tscale + 1) % 2;
        }
        if (pc.Dead)
        {
            endGameCanvas.SetActive(true);
            Cursor.visible = true;
        }
        
    }

    public void RestartGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        print("clicked");
    }

    public void CloseGame()
    {
        print("clicked1");
        Application.Quit();
       
    }
}
