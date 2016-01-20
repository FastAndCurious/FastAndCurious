using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {
    [SerializeField] private Text speedAndGearInfo = null;
    [SerializeField] private GameObject car = null;
    private int score;
    private int faults;
    private int lvl;
    private bool ended = false;

    public GameObject text;

    private Text gText;

    void Start ()
    {
        score = 0;
        gText = text.GetComponent<Text>();
    }

    void Update()
    {
        //if (!ended) AddScore(1);
    }

    void End ()
    {
        SendFinalScore();
    }

    void SendFinalScore()
    {
        var form = new WWWForm();
        form.AddField("finalScore", score);

        WWW www = new WWW("http://localhost:3333/postscore", form);

        StartCoroutine(SendScoreToWebsite(www));
    }

    IEnumerator SendScoreToWebsite(WWW www)
    {
        yield return www;
    }

	// Update is called once per frame
	void FixedUpdate () {
		speedAndGearInfo.text = "Speed: " + car.gameObject.GetComponent<CarController>().CurrentSpeed.ToString("0")
			+ "\nGear: " + car.gameObject.GetComponent<CarController>().currentGear;

        if (score >= 100) End();
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
}
