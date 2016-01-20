using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

    public float scaleFactor;

	// Use this for initialization
	void Start () {
        this.gameObject.GetComponent<Renderer>().material.SetTextureScale(
            "_MainTex",
            new Vector2(this.gameObject.transform.localScale.x  * scaleFactor, this.gameObject.transform.localScale.y * scaleFactor)
        );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
