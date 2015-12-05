﻿using UnityEngine;

//Demo skripta za Hrvoja.
//Skripta najprije čeka 2 sekunde, zatim stvori novi komad mape, i na kraju o tome obavijesti i novi komad i već postojeći komad.
public class GameController : MonoBehaviour {
    public Transform MapPart;//objekt koji će biti stvoren (dio mape)
    public Transform southPart;//ovaj objekt već postoji - to je početni komad mape

    private Transform northPart;//null na početku - kad se novi komad mape stvori, ovdje će se spremiti referenca na njega
    private float timer = 2f;
    private bool created = false;

	void Update () {
        timer -= Time.deltaTime;
        if(timer <= 0 && !created) {
            created = true;
            northPart = (Transform)Instantiate(MapPart, new Vector3(0, 0, 35.95f), Quaternion.identity);//stvori novi komad mape (sjeverni)
            notifyParts();//obavijesti dijelove mape da su dobili susjedni dio
        }	
	}

    private void notifyParts() {
        MapPartController southPartController = southPart.GetComponent<MapPartController>();
        MapPartController northPartController = northPart.GetComponent<MapPartController>();

        southPartController.setNorthPart(northPartController);//ova linija obavještava početni komad (južni) da je sjeverno od njega stvoren novi dio (sjeverni). 
                                                              //Ta metoda je bitna za povezivanje dijelova mape (konkretno - ovom linijom točke na sjevernom rubu 
                                                              //južnog dijela će biti spojene s točkama na južnom rubu sjevernog dijela).
                                                              //Kad ove metode ne bi bile pozvane, pješaci ne bi mogli prelaziti s jednih dijelova mape na druge.

        northPartController.setSouthPart(southPartController);//analogno prethodnoj liniji
    }
}
