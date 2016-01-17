using UnityEngine;
using System.Collections;

/// <summary>
/// Script used to manage car movement.
/// </summary>
public class AICarController :MonoBehaviour {
    private static GameObject[] allWaypoints;//array of all waypoints in the scene
    private const float MINIMAL_DISTANCE_TO_POINT = 15f;//when the car is at this distance from waypoint, it will be considered to be at that waypoint; used to prevent cars to be at the same exact coordinates while waiting on a waypoint
    private Transform currentWaypoint;//current target waypoint
    private Transform previousWaypoint;//last visited waypoint
    private bool isPedestrianInFront;//set to true if there is a pedestrian in front, this is used in order to slow down on time
    private bool isAICarInFront;
    private bool pedestrianTooClose;//if a pedestrian is really close, the car will stop
    private bool aiCarTooClose;
    private bool alreadyDriving;//set to true if the car is allowed to move on in the moment of setting the new waypoint; used to prevent cars to suddenly stop in the middle of the road because labels have changed in the meantime
    private float currentRotationSpeed;
    private float currentDrivingSpeed;

    public float maxRotationSpeed;//rotation speed
    public float maxDrivingSpeed;//movement speed
    public float acceleration;
    public float deceleration;

    /// <summary>
    /// Sets the starting waypoint to be the nearest one.
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
        isAICarInFront = false;
        alreadyDriving = false;
        pedestrianTooClose = false;
        aiCarTooClose = false;
        currentDrivingSpeed = 0;
        setRotationSpeed();
    }

    /// <summary>
    /// Controls the car.
    /// </summary>
    void Update() {
        if(shouldSlowDown()) {
            brake();
        } else {
            rotate();
            accelerate();
        }
        if(shouldStop()) stop(); // the car will stop if a pedestrian or another car is too close
        drive();
    }

    /// <summary>
    /// When this method is called (from other scripts), that means that some Rigidbody is close
    /// to this car. The method obtains information on what trigger collider has been activated
    /// (string "name"), and what other collider has entered it (collider "other").
    /// </summary>
    /// <param name="name">Name of the child trigger collider that has been activated.</param>
    /// <param name="other">Collider attached to a Rigidbody that has activated the trigger.</param>
    public void triggerEnter(string name, Collider other) {
        if(name == "FrontSpace" && other.tag == "Pedestrian") {
            isPedestrianInFront = true;
        }
        if(name == "FrontSpace" && other.tag == "AICar") {
            isAICarInFront = true;
        }
        if(name == "ProximitySpace" && other.tag == "Pedestrian") {
            pedestrianTooClose = true;
        }
        if(name == "ProximitySpace" && other.tag == "AICar") {
            aiCarTooClose = true;
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
        if(name == "FrontSpace" && other.tag == "AICar") {
            isAICarInFront = false;
        }
        if(name == "ProximitySpace" && other.tag == "Pedestrian") {
            pedestrianTooClose = false;
        }
        if(name == "ProximitySpace" && other.tag == "AICar") {
            aiCarTooClose = false;
        }
    }

    /// <summary>
    /// Sets the current waypoint.
    /// </summary>
    private void setNextWayPoint() {
        string previousWaypointName = previousWaypoint.name;
        previousWaypoint = currentWaypoint;
        currentWaypoint = currentWaypoint.GetComponent<PointDescriptor>().getAdjacentWaypoint(previousWaypointName);
    }

    /// <summary>
    /// Rotates the car towards the next waypoint.
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
    /// Moves the car towards the next waypoint.
    /// </summary>
    private void drive() {
        float distanceToPoint = (currentWaypoint.position - transform.position).sqrMagnitude;//it is not necessary for character to reach the exact waypoint location
        if(distanceToPoint <= MINIMAL_DISTANCE_TO_POINT) {
            setNextWayPoint();
            alreadyDriving = allowedToDrive();
        }

        if(!alreadyDriving && allowedToDrive()) alreadyDriving = true; //if the car started driving just at the moment when the light switched from green to yellow

        transform.Translate(Vector3.forward * Time.deltaTime * currentDrivingSpeed);
    }

    /// <summary>
    /// Checks if the car is allowed to move to the next waypoint. The car will not 
    /// be allowed to move to the next waypoint if labels of current and previous waypoints 
    /// differ or if at least one of them is not 0.
    /// </summary>
    /// <returns>true if the car is allowed to go, false otherwise</returns>
    private bool allowedToDrive() {
        int previousWaypointLabel = previousWaypoint.GetComponent<PointDescriptor>().getLabel();
        int currentWaypointLabel = currentWaypoint.GetComponent<PointDescriptor>().getLabel();
        return !(currentWaypointLabel != previousWaypointLabel && currentWaypointLabel != 0 && previousWaypointLabel != 0);
    }

    /// <summary>
    /// Reduces the current driving speed based on the deceleration value.
    /// </summary>
    private void brake() {
        alreadyDriving = false;
        float deltaSpeed = deceleration * Time.deltaTime;
        currentDrivingSpeed = currentDrivingSpeed - deltaSpeed <= 0 ? 0 : currentDrivingSpeed - deltaSpeed;
        setRotationSpeed();
    }

    /// <summary>
    /// Increases current driving speed based on the acceleration value.
    /// </summary>
    private void accelerate() {
        float deltaSpeed = acceleration * Time.deltaTime;
        currentDrivingSpeed = currentDrivingSpeed + deltaSpeed >= maxDrivingSpeed ? maxDrivingSpeed : currentDrivingSpeed + deltaSpeed;
        setRotationSpeed();
    }

    /// <summary>
    /// Sets current rotation speed. Higher driving speed results in higher rotation speed.
    /// </summary>
    private void setRotationSpeed() {
        currentRotationSpeed = maxRotationSpeed * currentDrivingSpeed / maxDrivingSpeed;
    }

    /// <summary>
    /// Tests if a car should slow down (if there is a pedestrian or anoter car inside "FrontSpace" collider).
    /// </summary>
    /// <returns>true if the car is supposed to slow down, false otherwise</returns>
    private bool shouldSlowDown() {
        return isPedestrianInFront || isAICarInFront || (!alreadyDriving && !allowedToDrive());
    }

    /// <summary>
    /// Checks if the car has to stop (if there is a pedestrian or anoter car inside "ProximitySpace" collider). 
    /// </summary>
    /// <returns>true if the car must stop, false otherwise</returns>
    private bool shouldStop() {
        return aiCarTooClose || pedestrianTooClose;
    }

    /// <summary>
    /// Stops the car by setting its driving speed to 0.
    /// </summary>
    private void stop() {
        currentDrivingSpeed = 0;
        setRotationSpeed();
    }

}
