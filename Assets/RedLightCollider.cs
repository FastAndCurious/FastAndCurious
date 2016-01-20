using UnityEngine;
using System.Collections;

public class RedLightCollider : MonoBehaviour {

    private GUIController guiController;

    public ISemaphore semaphore;

    // Use this for initialization
    void Start()
    {
        guiController = GameObject.Find("GUICanvas").GetComponent<GUIController>();
        semaphore = GetComponentInParent<ISemaphore>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("qqqqq");
        //Debug.Log(semaphore.carRed);

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("PlayerCar") && semaphore.isCarRed())
        {
            guiController.BrokeLaw(-1000);
        }
    }
}
