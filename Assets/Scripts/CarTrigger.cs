using UnityEngine;
using System.Collections;

/// <summary>
/// Script used to detect any Rigidbodies that enter or exit the trigger collider
/// attached to this GameObject. The collider has to be trigger; "isTrigger" property
/// has to be set to true and the collider has to be on layer "Trigger". Once this 
/// script detects a Rigidbody near the collider, it notifies its parent GameObject,
/// which will be the car.
/// </summary>
public class CarTrigger : MonoBehaviour {
    private AICarController carScript;//car which will be notified about presence of another Rigidbody

    /// <summary>
    /// Determines which car will be kept informed of rigidbodies in its proximity.
    /// </summary>
	void Start() {
        carScript = transform.parent.GetComponent<AICarController>();
    }

    /// <summary>
    /// This method is called automatically once a Rigidbody enters the trigger collider 
    /// attached to this GameObject. After the Rigidbody is detected, this script calls 
    /// the method triggerEnter from ai-car script; this way the car is notified 
    /// that a Rigidbody is near (the car can then avoid that Rigidbody, or perform 
    /// some other action).
    /// </summary>
    /// <param name="other">Collider that has entered the trigger.</param>
    void OnTriggerEnter(Collider other) {
        carScript.triggerEnter(name, other);
    }

    /// <summary>
    /// This method is called automatically once a Rigidbody exits the trigger collider 
    /// attached to this GameObject. After the Rigidbody is no longer detected, this 
    /// script calls the method triggerExit from ai-car script; this way the car
    /// is notified that there are no no more Rigidbodies near (the car can then stop 
    /// avoiding the Rigidbody, or perform some other action).
    /// </summary>
    /// <param name="other">Collider that has exited the trigger.</param>
    void OnTriggerExit(Collider other) {
        carScript.triggerExit(name, other);
    }

}
