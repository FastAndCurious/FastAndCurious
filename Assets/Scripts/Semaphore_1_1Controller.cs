using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Semaphore_1_1Controller : MonoBehaviour {

    public Color red;
    public Color yellow;
    public Color green;

    public bool carGreen;
    public bool carYellow;
    public bool carRed;
    public bool pedestrianGreen;
    public bool pedestrianRed;

    private bool[] lights;

    private Material carGreenMaterial;
    private Material carYellowMaterial;
    private Material carRedMaterial;
    private Material pedestrianGreenMaterial;
    private Material pedestrianRedMaterial;

    // Use this for initialization
    void Start () {
        lights = new bool[5];
        carGreenMaterial = transform.FindChild("carGreen").GetComponent<Renderer>().material;
        carYellowMaterial = transform.FindChild("carYellow").GetComponent<Renderer>().material;
        carRedMaterial = transform.FindChild("carRed").GetComponent<Renderer>().material;
        pedestrianGreenMaterial = transform.FindChild("pedestrianGreen").GetComponent<Renderer>().material;
        pedestrianRedMaterial = transform.FindChild("pedestrianRed").GetComponent<Renderer>().material;

        carGreenMaterial.EnableKeyword("_EMISSION");
        carYellowMaterial.EnableKeyword("_EMISSION");
        carRedMaterial.EnableKeyword("_EMISSION");
        pedestrianGreenMaterial.EnableKeyword("_EMISSION");
        pedestrianRedMaterial.EnableKeyword("_EMISSION");
    }
	
	// Update is called once per frame
	void Update () {
        bool[] newLights = new bool[] {carGreen, carYellow, carRed, pedestrianGreen, pedestrianRed};
        if (!ArraysEqual<Boolean>(lights, newLights))
        {
            lights = newLights;
            ChangeTextures();
        }
	}

    static bool ArraysEqual<T>(T[] a1, T[] a2)
    {
        if (ReferenceEquals(a1, a2))
            return true;

        if (a1 == null || a2 == null)
            return false;

        if (a1.Length != a2.Length)
            return false;

        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < a1.Length; i++)
        {
            if (!comparer.Equals(a1[i], a2[i])) return false;
        }
        return true;
    }

    private void ChangeTextures()
    {
        if (carGreen)
            carGreenMaterial.SetColor("_EmissionColor", green);
        else
            carGreenMaterial.SetColor("_EmissionColor", Color.black);

        if (carYellow)
            carYellowMaterial.SetColor("_EmissionColor", yellow);
        else
            carYellowMaterial.SetColor("_EmissionColor", Color.black);

        if (carRed)
            carRedMaterial.SetColor("_EmissionColor", red);
        else
            carRedMaterial.SetColor("_EmissionColor", Color.black);

        if (pedestrianGreen)
            pedestrianGreenMaterial.SetColor("_EmissionColor", green);
        else
            pedestrianGreenMaterial.SetColor("_EmissionColor", Color.black);

        if (pedestrianRed)
            pedestrianRedMaterial.SetColor("_EmissionColor", red);
        else
            pedestrianRedMaterial.SetColor("_EmissionColor", Color.black);

        
    }
}
