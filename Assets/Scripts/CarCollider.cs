using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarCollider : MonoBehaviour {

    private int carHit = 0;
    private int pedestrianHit = 0;
    private int wallHit = 0;
    private int streetObjectHit = 0;
    
    int _carHit { get; set; }
    int _pedestrainHit { get; set; }
    int _wallHit { get; set; }
    int _streetObjectHit { get; set; }

    public Text collisionText;

	void OnCollisionEnter(Collision col)
    {
        
        if (col.collider.tag == "Pedestrian")
        {
            col.collider.attachedRigidbody.AddForce(1, 1, 1000, ForceMode.Acceleration);

            StartCoroutine(Blink(col));
            
            pedestrianHit++;
            
            Debug.Log("Hit a pedestrian!");
        }

        if(col.collider.tag == "Car")
        {
            StartCoroutine(Blink(col));
           
            carHit++;
            Debug.Log("Hit a car!");
        }

        if(col.collider.tag == "Wall")
        {
            StartCoroutine(Blink(col));

            wallHit++;
            Debug.Log("Hit a wall!");
        }

        if(col.collider.tag == "StreetObject")
        {
            StartCoroutine(Blink(col));

            streetObjectHit++;
            Debug.Log("Hit a street object!");
        }
    }

   
    IEnumerator Blink(Collision col)
    {
       
        float blink = 0.1f;
        GameObject ob = col.gameObject;
        Renderer renderer;
        if(col.collider.tag == "Car")
        {
            
            renderer = ob.transform.Find("model").GetComponent<Renderer>();
           
            Color color = renderer.material.color;
           
            for (int i = 0; i < 3; i++)
            {

                renderer.material.SetColor("_Color", Color.black);
                yield return new WaitForSeconds(blink);

                renderer.material.SetColor("_Color", color);
                yield return new WaitForSeconds(blink);
                
            }

        }

        if(col.collider.tag == "Pedestrian")
        {
            renderer = col.gameObject.transform.Find("Character").GetComponent<Renderer>();
            Material material = renderer.material;
            Color color = material.color;

            for(int i = 0; i < 3; i++)
            {
                renderer.material.SetColor("_Color", Color.red);
                yield return new WaitForSeconds(blink);

                renderer.material.SetColor("_Color", color);
                yield return new WaitForSeconds(blink);

            }
        }

        if(col.collider.tag == "Wall")
        {
            renderer = col.gameObject.GetComponent<Renderer>();
            Material material = renderer.material;
            Color color = material.color;

            for(int i = 0; i < 3; i++)
            {
                renderer.material.SetColor("_Color", Color.yellow);
                yield return new WaitForSeconds(blink);

                renderer.material.SetColor("_Color", color);
                yield return new WaitForSeconds(blink);

            }
        }

    }
    
}
