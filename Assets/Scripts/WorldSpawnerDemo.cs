using UnityEngine;
using System.Collections;

public class WorldSpawnerDemo : MonoBehaviour {

	public GameObject straightRoad;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 3; ++i) {
			Instantiate (straightRoad, new Vector3 (0, 0, i * 40), new Quaternion (0, 0, 0, 0));
		}
	}
}
