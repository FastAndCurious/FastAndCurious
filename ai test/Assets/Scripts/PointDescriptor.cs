using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains the description of this waypoint and a list of waypoints that are connected to it (pedestrians are 
/// allowed to move from this waypoint only to the ones that are connected to it). Waypoint is described by its label. 
/// Waypoint can be a link to another map part. For example, if this waypoint is on the north edge of a map part, then 
/// its isNorthLink property has to be set to true. Waypoint also contains two link labels - a waypoint on the edge can 
/// only be connected to another waypoint on the edge (of another map part) if at least one of those values are the same. 
/// This class contains getter and setter methods for the label property, a method that connects a new waypoint to this one, 
/// and a method that returns all the waypoints that are connected to this one.
/// </summary>
public class PointDescriptor : MonoBehaviour {
    private static Dictionary<string, List<string>> dictionary;//dictionary that holds pairs of type (waypoint -> list of waypoints) where key waypoint is waypoint name and value list of waypoints is a list of waypoints that are connected to key waypoint
    private static System.Random indexGenerator;//generates a random index of a list; used to randomly chose one of the waypoints that are connected to this one
    private int label = 0;//Pedestrians are allowed to move from current waypoint only to waypoints with the same label.    
    private List<Transform> adjacentWaypoints;//list of waypoints that are accessible from this waypoint

    public Transform[] additionalAdjacentWaypoints;//array of manually inserted additional waypoints that are accessible from this waypoint    
    public bool isNorthLink;//if this waypoint is on the north side of the map part, this variable will be set to true; waypoints that are a north link have references to all the waypoints on the south side of the north adjacent map part
    public bool isEastLink;
    public bool isSouthLink;
    public bool isWestLink;
    public string firstLinkLabel;//two waypoints can't be connected if at least one of their link labels doesn't match
    public string secondLinkLabel;

    static PointDescriptor() {
        indexGenerator = new System.Random();
        dictionary = new Dictionary<string, List<string>>();
        dictionary.Add("1", new List<string> { "2" });
        dictionary.Add("2", new List<string> { "1", "3" });
        dictionary.Add("3", new List<string> { "2" });
        dictionary.Add("4", new List<string> { "5" });
        dictionary.Add("5", new List<string> { "6", "4" });
        dictionary.Add("6", new List<string> { "5" });
        dictionary.Add("7", new List<string> { "8" });
        dictionary.Add("8", new List<string> { "7", "9" , "13"});
        dictionary.Add("9", new List<string> { "8" });
        dictionary.Add("10", new List<string> { "11" });
        dictionary.Add("11", new List<string> { "10", "12", "20"});
        dictionary.Add("12", new List<string> { "11" });
        dictionary.Add("13", new List<string> { "8" });
        dictionary.Add("14", new List<string> { "15" });
        dictionary.Add("15", new List<string> { "14", "16" });
        dictionary.Add("16", new List<string> { "15" });
        dictionary.Add("17", new List<string> { "18" });
        dictionary.Add("18", new List<string> { "17", "19" });
        dictionary.Add("19", new List<string> { "18" });
        dictionary.Add("20", new List<string> { "11" });
    }

    void Awake() {
        adjacentWaypoints = new List<Transform>();
        if(dictionary.ContainsKey(name)) {
            List<string> adjacentPointNames = dictionary[name];
            foreach(var pointName in adjacentPointNames) {
                Transform adjacentWaypoint = transform.parent.Find(pointName);
                if(adjacentWaypoint != null) adjacentWaypoints.Add(adjacentWaypoint);
            }
        }
        adjacentWaypoints.AddRange(additionalAdjacentWaypoints);
    }

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

    /// <summary>
    /// Adds a new waypoint to the list of adjacent waypoints.
    /// </summary>
    /// <param name="newWaypoint">new adjacent waypoint</param>
    public void addAdjacentWaypoint(Transform newWaypoint) {
        adjacentWaypoints.Add(newWaypoint);
    }

    /// <summary>
    /// Method which will be used by pedestrians to determine their next target waypoint.
    /// One waypoint from the array of surrounding waypoints is randomly chosen and 
    /// returned to the pedestrian who called this method.
    /// </summary>
    /// <param name="alreadyVisitedWaypoint">Waypoint that has just been visited, already 
    /// visited waypoint will not be returned again.</param>
    /// <returns>Returns a randomly chosen waypoint adjacent to this one that has not 
    /// already been visited.</returns>
    public Transform getAdjacentWaypoint(string alreadyVisitedWaypoint) {
        int randomIndex = indexGenerator.Next(0, adjacentWaypoints.Count);
        return adjacentWaypoints[randomIndex].name != alreadyVisitedWaypoint ? 
            adjacentWaypoints[randomIndex] : adjacentWaypoints[(randomIndex+1)%adjacentWaypoints.Count];
    }

}
