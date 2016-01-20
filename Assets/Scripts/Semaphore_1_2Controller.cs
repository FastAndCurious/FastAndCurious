using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Semaphore_1_2Controller : MonoBehaviour, ISemaphore {

    public Color red;
    public Color yellow;
    public Color green;

    public bool carGreen;
    public bool carYellow;
    public bool carRed;

    public bool isCarGreen()
    {
        return carGreen;
    }
    public bool isCarYellow()
    {
        return carYellow;
    }
    public bool isCarRed()
    {
        return carRed;
    }

    public bool pedestrianHorizontalGreen;
    public bool pedestrianHorizontalRed;
    public bool pedestrianVerticalGreen;
    public bool pedestrianVerticalRed;

    private bool[] lights;

    private Material carGreenMaterial;
    private Material carYellowMaterial;
    private Material carRedMaterial;
    private Material pedestrianHorizontalGreenMaterial;
    private Material pedestrianHorizontalRedMaterial;
    private Material pedestrianVerticalRedMaterial;
    private Material pedestrianVerticalGreenMaterial;

    // Use this for initialization
    void Start () {
        lights = new bool[7];
        carGreenMaterial = transform.FindChild("carGreen").GetComponent<Renderer>().material;
        carYellowMaterial = transform.FindChild("carYellow").GetComponent<Renderer>().material;
        carRedMaterial = transform.FindChild("carRed").GetComponent<Renderer>().material;
        pedestrianHorizontalGreenMaterial = transform.FindChild("pedestrianHorizontalGreen").GetComponent<Renderer>().material;
        pedestrianHorizontalRedMaterial = transform.FindChild("pedestrianHorizontalRed").GetComponent<Renderer>().material;
        pedestrianVerticalGreenMaterial = transform.FindChild("pedestrianVerticalGreen").GetComponent<Renderer>().material;
        pedestrianVerticalRedMaterial = transform.FindChild("pedestrianVerticalRed").GetComponent<Renderer>().material;


        carGreenMaterial.EnableKeyword("_EMISSION");
        carYellowMaterial.EnableKeyword("_EMISSION");
        carRedMaterial.EnableKeyword("_EMISSION");
        pedestrianHorizontalGreenMaterial.EnableKeyword("_EMISSION");
        pedestrianHorizontalRedMaterial.EnableKeyword("_EMISSION");
        pedestrianVerticalGreenMaterial.EnableKeyword("_EMISSION");
        pedestrianVerticalRedMaterial.EnableKeyword("_EMISSION");
    }
	
	// Update is called once per frame
	void Update () {
        bool[] newLights = new bool[] {carGreen, carYellow, carRed, pedestrianHorizontalGreen, pedestrianHorizontalRed,
                                            pedestrianVerticalGreen, pedestrianVerticalRed};
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

        if (pedestrianHorizontalGreen)
            pedestrianHorizontalGreenMaterial.SetColor("_EmissionColor", green);
        else
            pedestrianHorizontalGreenMaterial.SetColor("_EmissionColor", Color.black);

        if (pedestrianHorizontalRed)
            pedestrianHorizontalRedMaterial.SetColor("_EmissionColor", red);
        else
            pedestrianHorizontalRedMaterial.SetColor("_EmissionColor", Color.black);


        if (pedestrianVerticalGreen)
            pedestrianVerticalGreenMaterial.SetColor("_EmissionColor", green);
        else
            pedestrianVerticalGreenMaterial.SetColor("_EmissionColor", Color.black);

        if (pedestrianVerticalRed)
            pedestrianVerticalRedMaterial.SetColor("_EmissionColor", red);
        else
            pedestrianVerticalRedMaterial.SetColor("_EmissionColor", Color.black);
    }
}
