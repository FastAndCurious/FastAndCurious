using UnityEngine;
using System.Collections;

/// <summary>
/// Contains the description of this waypoint. Waypoint is described only by its label. 
/// This class contains getter and setter methods for the label property.
/// </summary>
public class PointDescriptor : MonoBehaviour {
    private int label = 0;//Pedestrians are allowed to move from current waypoint only to waypoints with the same label.

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

}
