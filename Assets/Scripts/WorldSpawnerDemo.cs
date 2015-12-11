using UnityEngine;
using System.Collections;

public class WorldSpawnerDemo : MonoBehaviour {

  public GameObject straightRoad;
  public GameObject curvedRoad;
  
  public GameObject car;
	// Use this for initialization
	void Start () {
    // Add car
    Instantiate(car, Vector3.zero, Quaternion.identity);
    
    for (int i = 0; i < 3; ++i) {
      for (int j = 0; j < 3; ++j) {
        Instantiate(straightRoad, new Vector3(j * 40, 0, i * 40), new Quaternion);
      }
    }
	}
}
