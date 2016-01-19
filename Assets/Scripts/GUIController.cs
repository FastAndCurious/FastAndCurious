using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {
	[SerializeField] private Text speedAndGearInfo = null;
	[SerializeField] private GameObject car = null;
    private int score;

    public GameObject text;

    private Text gText;

    void Start ()
    {
        score = 0;
        gText = text.GetComponent<Text>();
    }

	// Update is called once per frame
	void FixedUpdate () {
		speedAndGearInfo.text = "Speed: " + car.gameObject.GetComponent<CarController>().CurrentSpeed.ToString("0")
			+ "\nGear: " + car.gameObject.GetComponent<CarController>().currentGear;
	}

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        gText.text = score.ToString();
        Debug.Log("Score:" + score);
    }
}
