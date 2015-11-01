using UnityEngine;
using System.Collections;

/// <summary>
/// Script used to manage character movement.
/// Characters require waypoints to be able to move. They have the predefined waypoints, but can be ordered to use the user defined waypoints.
/// </summary>
public class AI :MonoBehaviour {
    private const float MINIMAL_DISTANCE_TO_POINT = 1f;
    private const int INSERT_WAYPOINTS_MANUALLY = 1;
    private Transform[] wayPoints;//array of all waypoints this pedestrian is going to visit
    private int currentPosition;//index of the current waypoint in the array of waypoints
    private Transform currentWaypoint;//current target waypoint

    public int insertWayPointsManually;//if set to 1, this pedestrian uses only the user defined waypoints, otherwise he uses both the user defined waypoints and the predefined ones
    public float rotationSpeed;//rotation speed
    public float walkSpeed;//movement speed
    public Transform[] userDefinedWayPoints;//array of user defined waypoints

    
    /// <summary>
    /// Determines what waypoints to use and sets the starting waypoint.
    /// </summary>
    void Start() {
        currentPosition = 0;
        if(insertWayPointsManually == INSERT_WAYPOINTS_MANUALLY) {
            sortUserWayPoints();
        } else {
            sortAllWayPoints();
        }
        currentWaypoint = wayPoints[0];   
    }

    /// <summary>
    /// Rotates and moves the character a little every frame.
    /// </summary>
    void Update() {
        rotate();
        walk();
    }

    /// <summary>
    /// Fills an array of waypoints with manually inserted waypoints (predefined waypoints are not used).
    /// </summary>
    private void sortUserWayPoints() {
        wayPoints = new Transform[userDefinedWayPoints.Length];
        for(int i = 0; i < userDefinedWayPoints.Length; i++) {
            wayPoints[i] = userDefinedWayPoints[i];
        }
    }

    /// <summary>
    /// Fills an array of waypoints with both user defined waypoints and predefined ones. 
    /// Predefined waypoints are visited first, and user defined ones are visited after all the predefined ones have been visited.
    /// Predefined waypoints are sorted lexicographically. User defined waypoints are not sorted.
    /// </summary>
    private void sortAllWayPoints() {
        Transform wayPointsParent = transform.parent.Find("WayPoints");
        Transform[] temporaryWayPoints = wayPointsParent.GetComponentsInChildren<Transform>();
        wayPoints = new Transform[temporaryWayPoints.Length + userDefinedWayPoints.Length];
        for(int i = 0; i < temporaryWayPoints.Length; i++) {
            Transform minimum = temporaryWayPoints[i];
            for(int j = i; j < temporaryWayPoints.Length; j++) {
                if(string.Compare(temporaryWayPoints[j].name,  minimum.name) < 0) {
                    minimum = temporaryWayPoints[j];
                }
                wayPoints[i] = minimum;
            }
        }
        for(int i = 0; i < userDefinedWayPoints.Length; i++) {
            wayPoints[i + temporaryWayPoints.Length] = userDefinedWayPoints[i];
        }
    }

    /// <summary>
    /// Sets the next waypoint. If the label of the next waypoint differs from the label of the current waypoint, 
    /// the character waits (next waypoint is not set until its label changes to the adequate value); 
    /// it is not allowed for pedestrians to move between waypoints with different labels (except if the label is equal to 0). 
    /// If the label of current waypoint or the label of the next waypoint is equal to 0, the pedestrian is not forced to wait; 
    /// pedestrians are allowed to move to waypoints with label equal to 0 or from them at any time.
    /// </summary>
    private void setNextWayPoint() {
        int currentWaypointLabel = currentWaypoint.GetComponent<PointDescriptor>().getLabel();//label of the current waypoint
        int nextWaypointLabel = currentPosition+1==wayPoints.Length ?
                                wayPoints[0].GetComponent<PointDescriptor>().getLabel() :
                                wayPoints[currentPosition + 1].GetComponent<PointDescriptor>().getLabel();//label of the next waypoint
        if(currentWaypointLabel != nextWaypointLabel && currentWaypointLabel != 0 && nextWaypointLabel != 0) {//if you are not allowed to move on, wait
            return;
        }
        currentPosition++;
        if(currentPosition >= wayPoints.Length) {
            currentPosition = 0;
        }
        currentWaypoint = wayPoints[currentPosition];
    }

    /// <summary>
    /// Rotates the character towards the next waypoint.
    /// </summary>
    private void rotate() {
        Vector3 target = new Vector3(currentWaypoint.position.x, 0, currentWaypoint.position.z);
        Vector3 current = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetDir = target - current;
        float step = rotationSpeed * Time.deltaTime;//degrees per second value (step) depends on rotation speed
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// <summary>
    /// Moves the character towards the next waypoint.
    /// </summary>
    private void walk() {
        float distanceToPoint = (currentWaypoint.position - transform.position).sqrMagnitude;//it is not necessary for character to reach the exact waypoint location
        if(distanceToPoint <= MINIMAL_DISTANCE_TO_POINT) {
            setNextWayPoint();
            return;
        }
        transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);

    }

}


