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

        CarController car = GetComponent<CarController>();
        if(car.TopSpeed != 0)
        {
            carEngine.pitch = car.CurrentSpeed / car.TopSpeed;
        }
        //Debug.Log(carEngine.clip.name + " " + carEngine.pitch);
    }
}
