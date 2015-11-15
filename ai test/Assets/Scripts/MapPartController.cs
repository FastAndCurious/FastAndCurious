using UnityEngine;
using System.Collections;

public class MapPartController : MonoBehaviour {
    private MapPartController northPart = null;//north adjacent map part
    private MapPartController eastPart = null;//east adjacent map part
    private MapPartController southPart = null;//south adjacent map part
    private MapPartController westPart = null;//west adjacent map part

    void Start () {

	}

	void Update () {
	
	}

    /// <summary>
    /// Sets the north adjacent map part. When it is determined what map part is to the north 
    /// of this one, linkToNorth() method is invoked, and waypoints that are on the north 
    /// side of this map part get the reference to the waypoints that are on the south side 
    /// of the north adjacent part.
    /// </summary>
    /// <param name="northPart">Reference to the script that controlls the map part to the north 
    /// of this one.</param>
    public void setNorthPart(MapPartController northPart) {
        this.northPart = northPart;
        linkToNorth();
    }
    public MapPartController getNorthPart() {
        return northPart;
    }

    public void setEastPart(MapPartController eastPart) {
        this.eastPart = eastPart;
    }
    public MapPartController getEastPart() {
        return eastPart;
    }

    public void setSouthPart(MapPartController southPart) {
        this.southPart = southPart;
    }
    public MapPartController getSouthPart() {
        return southPart;
    }

    public void setWestPart(MapPartController westPart) {
        this.westPart = westPart;
    }
    public MapPartController getWestPart() {
        return westPart;
    }

    /// <summary>
    /// This method is invoked once the north part of the map is known. Waypoints 
    /// that are on the north side of this map part get the reference to the waypoints 
    /// that are on the south side of the north adjacent part.
    /// </summary>
    private void linkToNorth() {
        //not yet implemented
    }

}
