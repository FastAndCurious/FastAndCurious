using UnityEngine;
using System.Collections;

/// <summary>
/// Contains the description of this waypoint. Waypoint is described only by its label. 
/// This class contains getter and setter methods for the label property.
/// </summary>
public class PointDescriptor : MonoBehaviour {
    private int label = 0;//Pedestrians are allowed to move from current waypoint only to waypoints with the same label.

    public Transform[] surroundingWaypoints;//array of waypoints that are accessible from this waypoint
    public bool isNorthLink;//if this waypoint is on the north side of the map part, this variable will be set to true; waypoints that are a north link have references to all the waypoints on the south side of the north adjacent map part
    public bool isEastLink;
    public bool isSouthLink;
    public bool isWestLink;

    /// <summary>
    /// Returns this waypoint's label.
    /// </summary>
    /// <returns>waypoint label</returns>
    public int getLabel() {
        return label;
    }

    /// <summary>
    /// Sets the label of this waypoint to a given value.
    /// </summary>
    /// <param name="newLabel">New value of the waypoint label</param>
    public void setLabel(int newLabel) {
        label = newLabel;
    }
 /*  
    /// <summary>
    /// Method which will be used by pedestrians to determine their next target waypoint.
    /// One waypoint from the array of surrounding waypoints is chosen and returned to 
    /// the pedestrian who called this method.
    /// </summary>
    /// <returns></returns>
    public Transform getAdjacentWaypoint() {

    }
*/
}
