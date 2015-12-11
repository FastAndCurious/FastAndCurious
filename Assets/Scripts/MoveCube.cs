using UnityEngine;
using System.Collections;

public class MoveCube : MonoBehaviour
{
    void OnCollisionEnter (Collision col)
    {
       if(col.collider.tag == "Cube")
        {
			col.collider.attachedRigidbody.AddForce(1,1,1000, ForceMode.Acceleration);
            
        }
    }
	
}
