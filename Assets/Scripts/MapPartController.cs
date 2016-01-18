using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script which links waypoints on the edge of this map part to the waypoints on the edge 
/// of adjacent map parts. This script holds references to all adjacent map parts, which are 
/// set to null at the beginning. When a new map part is generated, this script has to be 
/// informed about it by calling one of its setter methods and providing a reference to a script 
/// that belongs to a new map part. For example, if a new part that is to the north of this one 
/// is generated, this script's setNorthPart method has to be called. The argument to this method 
/// should be reference to a MapPartController script that belongs to the new map part.
/// </summary>
public class MapPartController :MonoBehaviour {
    private MapPartController northPart = null;//north adjacent map part
    private MapPartController eastPart = null;//east adjacent map part
    private MapPartController southPart = null;//south adjacent map part
    private MapPartController westPart = null;//west adjacent map part

    public float rotation;

    void Awake() {
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    /// <summary>
    /// Sets the north adjacent map part. When it is determined what map part is to the north 
    /// of this one, link method is invoked, and waypoints that are on the north 
    /// side of this map part get the reference to the waypoints that are on the south side 
    /// of the north adjacent part.
    /// </summary>
    /// <param name="northPart">Reference to the script that controlls the map part to the north 
    /// of this one.</param>
    public void setNorthPart(MapPartController northPart) {
        this.northPart = northPart;
        link(northPart, "southLink", "northLink");
    }

    /// <summary>
    /// Returns a reference to the MapPartController script that belongs to the map part 
    /// that is to the north of this one.
    /// </summary>
    /// <returns>north adjacent map part MapPartController script</returns>
    public MapPartController getNorthPart() {
        return northPart;
    }

    /// <summary>
    /// Sets the east adjacent map part. When it is determined what map part is to the east 
    /// of this one, link method is invoked, and waypoints that are on the east 
    /// side of this map part get the reference to the waypoints that are on the west side 
    /// of the east adjacent part.
    /// </summary>
    /// <param name="eastPart">Reference to the script that controlls the map part to the east 
    /// of this one.</param>
    public void setEastPart(MapPartController eastPart) {
        this.eastPart = eastPart;
        link(eastPart, "westLink", "eastLink");
    }

    /// <summary>
    /// Returns a reference to the MapPartController script that belongs to the map part 
    /// that is to the east of this one.
    /// </summary>
    /// <returns>east adjacent map part MapPartController script</returns>
    public MapPartController getEastPart() {
        return eastPart;
    }

    /// <summary>
    /// Sets the south adjacent map part. When it is determined what map part is to the south 
    /// of this one, link method is invoked, and waypoints that are on the south 
    /// side of this map part get the reference to the waypoints that are on the north side 
    /// of the south adjacent part.
    /// </summary>
    /// <param name="southPart">Reference to the script that controlls the map part to the south 
    /// of this one.</param>
    public void setSouthPart(MapPartController southPart) {
        this.southPart = southPart;
        link(southPart, "northLink", "southLink");
    }

    /// <summary>
    /// Returns a reference to the MapPartController script that belongs to the map part 
    /// that is to the south of this one.
    /// </summary>
    /// <returns>south adjacent map part MapPartController script</returns>
    public MapPartController getSouthPart() {
        return southPart;
    }

    /// <summary>
    /// Sets the west adjacent map part. When it is determined what map part is to the west 
    /// of this one, link method is invoked, and waypoints that are on the west 
    /// side of this map part get the reference to the waypoints that are on the east side 
    /// of the west adjacent part.
    /// </summary>
    /// <param name="westPart">Reference to the script that controlls the map part to the west 
    /// of this one.</param>
    public void setWestPart(MapPartController westPart) {
        this.westPart = westPart;
        link(westPart, "eastLink", "westLink");
    }

    /// <summary>
    /// Returns a reference to the MapPartController script that belongs to the map part 
    /// that is to the west of this one.
    /// </summary>
    /// <returns>west adjacent map part MapPartController script</returns>
    public MapPartController getWestPart() {
        return westPart;
    }

    /// <summary>
    /// This method is invoked once a new map part is generated next to this one. Waypoints on 
    /// the edge of the new part (source part) will be linked to the waypoints on this part if 
    /// their link types match. For example, a new north part (source part) contains waypoints 
    /// of type southLink (newLinkType). Those waypoints are linked to waypoints on this part 
    /// that are of type northLink (thisLinkType).
    /// </summary>
    /// <param name="sourcePart">Script that holds references to new waypoints.</param>
    /// <param name="newLinkType">Waypoints of this type on the source part will get linked to the 
    /// waypoints of thisLinkType on this part.</param>
    /// <param name="thisLinkType">Waypoints of this type on this map part will be linked to the 
    /// waypoints on source part of type newLinkType.</param>
    private void link(MapPartController sourcePart, string newLinkType, string thisLinkType) {
        List<Transform> newWaypoints = new List<Transform>();
        foreach(Transform waypoint in sourcePart.transform.Find("Waypoints")) {
            PointDescriptor pointScript = waypoint.GetComponent<PointDescriptor>();
            switch(newLinkType) {
                case "northLink":
                    if(pointScript.isNorthLink) {
                        newWaypoints.Add(waypoint);
                    }
                    break;
                case "eastLink":
                    if(pointScript.isEastLink) {
                        newWaypoints.Add(waypoint);
                    }
                    break;
                case "southLink":
                    if(pointScript.isSouthLink) {
                        newWaypoints.Add(waypoint);
                    }
                    break;
                case "westLink":
                    if(pointScript.isWestLink) {
                        newWaypoints.Add(waypoint);
                    }
                    break;
            }
        }
        Transform waypoints = transform.Find("Waypoints");
        foreach(Transform waypoint in waypoints) {
            PointDescriptor pointScript = waypoint.GetComponent<PointDescriptor>();
            switch(thisLinkType) {
                case "northLink":
                    if(pointScript.isNorthLink) {
                        addNewWaypointsToCurrentOne(newWaypoints, pointScript);
                    }
                    break;
                case "eastLink":
                    if(pointScript.isEastLink) {
                        addNewWaypointsToCurrentOne(newWaypoints, pointScript);
                    }
                    break;
                case "southLink":
                    if(pointScript.isSouthLink) {
                        addNewWaypointsToCurrentOne(newWaypoints, pointScript);
                    }
                    break;
                case "westLink":
                    if(pointScript.isWestLink) {
                        addNewWaypointsToCurrentOne(newWaypoints, pointScript);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Links all of the waypoints from the list to the waypoint given as second parameter if at least 
    /// one of their link labels match (it doesn't count if both labels are empty strings).
    /// </summary>
    /// <param name="newWaypoints">list of waypoints that will be added to the current waypoint</param>
    /// <param name="pointScript">current waypoint, it will get references to all the waypoints from the list</param>
    private static void addNewWaypointsToCurrentOne(List<Transform> newWaypoints, PointDescriptor pointScript) {
        foreach(Transform newWaypoint in newWaypoints) {
            string point1FirstLabel = pointScript.firstLinkLabel;
            string point1SecondLabel = pointScript.secondLinkLabel;
            string point2FirstLabel = newWaypoint.GetComponent<PointDescriptor>().firstLinkLabel;
            string point2SecondLabel = newWaypoint.GetComponent<PointDescriptor>().secondLinkLabel;
            if((point1FirstLabel.Equals(point2FirstLabel) && !point1FirstLabel.Equals("") && !point2FirstLabel.Equals("")) || (point1SecondLabel.Equals(point2SecondLabel) && !point1SecondLabel.Equals("") && !point2SecondLabel.Equals(""))) {
                pointScript.addAdjacentWaypoint(newWaypoint);
            }
        }
    }

}
