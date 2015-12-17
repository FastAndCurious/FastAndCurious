using UnityEngine;
using System.Collections;

public class WorldSpawnerDemo : MonoBehaviour {

	public GameObject straightRoad, curvedRoad;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 3; ++i) {
			Instantiate (straightRoad, new Vector3 (0, 0, i * 40), new Quaternion (0, 0, 0, 0));
			Instantiate (straightRoad, new Vector3 (-40, 0, i * 40), new Quaternion (0, 0, 0, 0));
		}
		Instantiate (curvedRoad, new Vector3 (0, 0, 120), new Quaternion (0, -90, 0, 0));
		Instantiate (curvedRoad, new Vector3 (-40, 0, 120), new Quaternion (0, 180, 0, 0));
		Instantiate (curvedRoad, new Vector3 (-40, 0, -40), new Quaternion (0, 90, 0, 0));
		Instantiate (curvedRoad, new Vector3 (0, 0, -40), new Quaternion (0, 0, 0, 0));
	}
}
