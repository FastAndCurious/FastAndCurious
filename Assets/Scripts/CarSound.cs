using UnityEngine;
using System.Collections;

public class CarSound : MonoBehaviour {

    private AudioSource[] sound;
    private AudioSource carEngine;
    private AudioSource backgroundSound;

	void Start () {
        sound = transform.Find("Sound").GetComponents<AudioSource>();
        backgroundSound = sound[3];
        carEngine = sound[0];

    }
	
	void Update () {
        
        //popraviti da bude po brzini
        carEngine.pitch = GetComponent<CarController>().CurrentSpeed / 30; 
        Debug.Log(carEngine.clip.name + " " + carEngine.pitch);
    }
}
