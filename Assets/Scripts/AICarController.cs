using UnityEngine;
using System.Collections;

/// <summary>
/// Script used to manage character movement.
/// Characters require waypoints to be able to move. They have the predefined waypoints, but can be ordered to use the user defined waypoints.
/// </summary>
public class AICarController :MonoBehaviour {
    private static GameObject[] allWaypoints;//array of all waypoints in the scene

    private const float MINIMAL_DISTANCE_TO_POINT = 15f;//when the pedestrian is at this distance from waypoint, he will be considered to be at that waypoint; used to prevent characters to be at the same exact coordinates while waiting on a waypoint
    private Transform currentWaypoint;//current target waypoint
    private Transform previousWaypoint;//last visited waypoint
    private bool isPedestrianInFront;//set to true if there is another pedestrian in front, this is used in order to avoid the pedestrian in front if there is one
    private bool shouldStop;//if set to true, this pedestrian has to stop (if he is in someone's personal space, but hasn't been in front of him first)
    private bool alreadyWalking;//set to true if a pedestrian is allowed to move on in the moment of setting the new waypoint; used to prevent pedestrians to suddenly stop in the middle of the road because labels have changed in the meantime
    private float currentRotationSpeed;
    public float currentDrivingSpeed;

    public float maxRotationSpeed;//rotation speed
    public float maxDrivingSpeed;//movement speed
    public float acceleration;
    public float deceleration;

    /// <summary>
    /// Sets the starting waypoint to be the neares one.
    /// </summary>
    void Start() {
        allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        Transform nearestWaypoint = allWaypoints[0].transform;
        foreach(var wayPoint in allWaypoints) {
            Transform wayPointTransform = wayPoint.transform;
            if(Vector3.SqrMagnitude(wayPointTransform.position - transform.position) < Vector3.SqrMagnitude(nearestWaypoint.position - transform.position)) {
                nearestWaypoint = wayPointTransform;
            }
        }
        previousWaypoint = nearestWaypoint;
        currentWaypoint = nearestWaypoint;
        isPedestrianInFront = false;
        shouldStop = false;
        alreadyWalking = false;
        currentDrivingSpeed = 0;
        currentRotationSpeed = maxRotationSpeed;
    }

    /// <summary>
    /// Rotates and moves the character a little every frame.
    /// </summary>
    void Update() {
        if(isPedestrianInFront) {
            avoidPedestrian();
        } else {
            if(shouldStop) {
                brake();
            } else {
                rotate();
            }
        }
        if(!shouldStop) drive();
    }

    /// <summary>
    /// When this method is called (from other scripts), it means that some Rigidbody is near 
    /// this pedestrian. The method obtains information on what trigger collider has been activated
    /// (string "name"), and what other collider has entered it (collider "other").
    /// </summary>
    /// <param name="name">Name of the child trigger collider that has been activated.</param>
    /// <param name="other">Collider attached to a Rigidbody that has activated the trigger.</param>
    public void triggerEnter(string name, Collider other) {
        if(name == "FrontSpace" && other.tag == "Pedestrian") {
            isPedestrianInFront = true;
        }
        if(name == "PersonalSpace") {
            if(!isPedestrianInFront) {
                shouldStop = true;
            }
        }
    }

    /// <summary>
    /// Opposite of "triggerEnter", when this method is called (from other scripts), it means that a Rigidbody
    /// has exited the child trigger collider that has called this script. The method obtains information on what 
    /// trigger collider has been deactivated (string "name"), and what other collider has exited it (collider "other").
    /// </summary>
    /// <param name="name">Name of the child trigger collider that has been deactivated.</param>
    /// <param name="other">Collider attached to a Rigidbody that has exited the trigger.</param>
    public void triggerExit(string name, Collider other) {
        if(name == "FrontSpace" && other.tag == "Pedestrian") {
            isPedestrianInFront = false;
        }
        if(name == "PersonalSpace") {
            shouldStop = false;
        }
    }

    /// <summary>
    /// Sets the current waypoint. The previous one becomes the current one.
    /// </summary>
    private void setNextWayPoint() {
        string previousWaypointName = previousWaypoint.name;
        previousWaypoint = currentWaypoint;
        currentWaypoint = currentWaypoint.GetComponent<PointDescriptor>().getAdjacentWaypoint(previousWaypointName);
    }

    /// <summary>
    /// Rotates the character towards the next waypoint.
    /// </summary>
    private void rotate() {
        Vector3 target = new Vector3(currentWaypoint.position.x, 0, currentWaypoint.position.z);
        Vector3 current = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetDir = target - current;
        float step = currentRotationSpeed * Time.deltaTime;//amount of degrees per second (step) depends on rotation speed
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// <summary>
    /// Rotates the character to the right in order to avoid the pedestrian in front.
    /// </summary>
    private void avoidPedestrian() {
        Vector3 target = new Vector3(transform.position.x + transform.right.x, 0, transform.position.z + transform.forward.z);
        Vector3 current = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetDir = target - current;
        float step = currentRotationSpeed * Time.deltaTime;//amount of degrees per second (step) depends on rotation speed
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// <summary>
    /// Moves the character towards the next waypoint and starts the walking animation. If a pedestrian 
    /// is not allowed to move, he stops. If he is not allowed to move, but has already started walking, 
    /// he doesn't stop (for example, when the traffic light changes and a pedestrian is in the middle of 
    /// the road - he will not stop in the middle of the road).
    /// </summary>
    private void drive() {
        float distanceToPoint = (currentWaypoint.position - transform.position).sqrMagnitude;//it is not necessary for character to reach the exact waypoint location
        if(distanceToPoint <= MINIMAL_DISTANCE_TO_POINT) {
            setNextWayPoint();
            alreadyWalking = allowedToWalk();
        }

        if(!alreadyWalking) {
            if(!allowedToWalk()) {
                brake();
                return;
            } else {
                alreadyWalking = true;
            }
        }

        accelerate();
        transform.Translate(Vector3.forward * Time.deltaTime * currentDrivingSpeed);
    }

    /// <summary>
    /// Checks if a pedestrian is allowed to move to the next waypoint. The pedestrian will 
    /// not be allowed to move to the next waypoint if labels of current and previous waypoints 
    /// differ or at least one of them is not 0.
    /// </summary>
    /// <returns></returns>
    private bool allowedToWalk() {
        int previousWaypointLabel = previousWaypoint.GetComponent<PointDescriptor>().getLabel();
        int currentWaypointLabel = currentWaypoint.GetComponent<PointDescriptor>().getLabel();
        return !(currentWaypointLabel != previousWaypointLabel && currentWaypointLabel != 0 && previousWaypointLabel != 0);
    }

    private void brake() {
        alreadyWalking = false;
        float deltaSpeed = -deceleration * Time.deltaTime;
        currentDrivingSpeed = currentDrivingSpeed + deltaSpeed <= 0 ? 0 : currentDrivingSpeed + deltaSpeed;
        setRotationSpeed();
        return;
    }

    private void accelerate() {
        float deltaSpeed = acceleration * Time.deltaTime;
        currentDrivingSpeed = currentDrivingSpeed + deltaSpeed >= maxDrivingSpeed ? maxDrivingSpeed : currentDrivingSpeed + deltaSpeed;
        setRotationSpeed();
    }

    private void setRotationSpeed() {
        currentRotationSpeed = maxRotationSpeed * currentDrivingSpeed / maxDrivingSpeed;
    }

}
