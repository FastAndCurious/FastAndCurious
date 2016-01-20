using UnityEngine;
using System.Collections;

public class RedLightCollider : MonoBehaviour {

    public GUIController guiController;

    public ISemaphore semaphore;

    // Use this for initialization
    void Start()
    {
        semaphore = GetComponentInParent<ISemaphore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (guiController == null)
        {
            GameObject foundCanvas = GameObject.FindGameObjectWithTag("GUI");
            if (foundCanvas != null)
            {
                guiController = foundCanvas.GetComponent<GUIController>();
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("PlayerCar") && semaphore.isCarRed())
        {
            guiController.RedLightPassed();
        }
    }
}
