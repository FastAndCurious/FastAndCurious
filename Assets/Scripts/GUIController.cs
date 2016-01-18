using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIController : MonoBehaviour {
	[SerializeField] private Text speedAndGearInfo = null;
	[SerializeField] private GameObject car = null;
	
	// Update is called once per frame
	void FixedUpdate () {
		speedAndGearInfo.text = "Speed: " + car.gameObject.GetComponent<CarController>().CurrentSpeed.ToString("0")
			+ "\nGear: " + car.gameObject.GetComponent<CarController>().currentGear;
	}
}
