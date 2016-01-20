using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour {
    [SerializeField] private Text speedAndGearInfo = null;
    [SerializeField] private GameObject car = null;
    public int faultsToLose;
    private int score;
    private int faults;
    private int lvl;
    private bool ended = false;

    private bool paused = false;

    public GameObject pauseHolder;
    public GameObject nickHolder;

    public GameObject text;
    public InputField nickName;

    private Text gText;
    private Text gNick;

    public int timeForPoints = 5;
    public int pointsForTime = 10;

    private float lastPointedTime;

    void Start()
    {
        score = 0;
        gText = text.GetComponent<Text>();
        car = GameObject.Find("Player");
        lastPointedTime = Time.time;
    }

    void Update()
    {
        //if (!ended) AddScore(1);
        if (Time.time > lastPointedTime + timeForPoints)
        {
            lastPointedTime = Time.time;
            AddScore(pointsForTime);
        }
        if (paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = false;
                pauseHolder.SetActive(false);
                Time.timeScale = 1;
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = true;
                pauseHolder.SetActive(true);
                Time.timeScale = 0;
            }
        }

    }

    void End(string nick)
    {
        SendFinalScore(nick);
    }

    void SendFinalScore(string nick)
    {
        var form = new WWWForm();
        form.AddField("score", score);
        form.AddField("player", nick);

        Debug.Log("pravim request");
        WWW www = new WWW("http://dito.ninja/postscore", form);

        StartCoroutine(SendScoreToWebsite(www));
        Debug.Log("poslo request");
    }

    IEnumerator SendScoreToWebsite(WWW www)
    {
        yield return www;
    }

    // Update is called once per frame
    void FixedUpdate() {
        speedAndGearInfo.text = "Speed: " + car.gameObject.GetComponent<CarController>().CurrentSpeed.ToString("0")
            + "\nGear: " + car.gameObject.GetComponent<CarController>().currentGear;

        //if (score >= 100) End();
    }

    public void PederstrianHit()
    {
        faults += 5;
        AddScore(-1000);
        CheckFaults();
    }

    public void AICarHit()
    {
        faults += 5;
        AddScore(-800);
        CheckFaults();
    }

    public void RedLightPassed()
    {
        faults += 2;
        AddScore(-500);
        CheckFaults();
    }

    void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        gText.text = score.ToString();
    }

    void CheckFaults()
    {
        if (faults >= 4)
        {
            /// FINISH GAME
        }
    }

    public void ReturnToMainMenu()
    {
        nickHolder.SetActive(true);
    }

    public void SendHighscore()
    {
        string nick = nickName.text;
        End(nick);
        Debug.Log("valjd sam poslo");
        SceneManager.LoadScene("MainMenu");
    }

}

