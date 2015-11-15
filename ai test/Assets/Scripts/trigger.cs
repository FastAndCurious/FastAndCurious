using UnityEngine;
using System.Collections;

/// <summary>
/// Script used to detect any Rigidbodies that enter or exit the trigger collider
/// attached to this GameObject. The collider has to be trigger; "isTrigger" property
/// has to be set to true and the collider has to be on layer "Trigger". Once this 
/// script detects a Rigidbody near the collider, it notifies its parent GameObject,
/// which will be the character.
/// </summary>
public class trigger : MonoBehaviour {
    private PedestrianController pedestrianScript;//character which will be notified about presence of another Rigidbody

    /// <summary>
    /// Assigns a character which this script will keep informed about presence of 
    /// other Rigidbodies.
    /// </summary>
	void Start () {
        pedestrianScript = transform.parent.GetComponent<PedestrianController>();
	}

    /// <summary>
    /// This method is called automatically once a Rigidbody enters the trigger collider 
    /// attached to this GameObject. After the Rigidbody is detected, this script calls 
    /// the method triggerEnter from character script; this way the character is notified 
    /// that a Rigidbody is near (the character can then avoid that Rigidbody, or perform 
    /// some other action).
    /// </summary>
    /// <param name="other">Collider that has entered the trigger.</param>
    void OnTriggerEnter(Collider other) {
        pedestrianScript.triggerEnter(name, other);
    }

    /// <summary>
    /// This method is called automatically once a Rigidbody exits the trigger collider 
    /// attached to this GameObject. After the Rigidbody is no longer detected, this 
    /// script calls the method triggerExit from character script; this way the character
    /// is notified that there are no no more Rigidbodies near (the character can then stop 
    /// avoiding the Rigidbody, or perform some other action).
    /// </summary>
    /// <param name="other">Collider that has exited the trigger.</param>
    void OnTriggerExit(Collider other) {
        pedestrianScript.triggerExit(name, other);
    }

}
