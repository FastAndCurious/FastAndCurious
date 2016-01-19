using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the intersection semaphores. 
/// An intersection consists of two streets: vertical and horizontal. 
/// An intersection can only be in four different states:
///     horizontal: cars and pedestrians are allowed to move in the horizontal street; 
///                 horizontal street has green lights, and vertical street has red lights
///     vertical: cars and pedestrians are allowed to move in the vertical street; 
///               vertical street has green lights, and horizontal street has red lights
///     horizontal_to_vertical: cars and pedestrians have the same permissions as in the state horizontal; 
///                             this state signifies that there is a transition from horizontal to vertical state 
///                             (everything is the same as in the state horizontal, except the yellow traffic lights are on)
///     vertical_to_horizontal: cars and pedestrians have the same permissions as in the state vertical; 
///                             this state signifies that there is a transition from vertical to horizontal state 
///                             (everything is the same as in the state vertical, except the yellow traffic lights are on)
/// </summary>
public class IntersectionController :MonoBehaviour {
    private const int VERTICAL = 1;//state in which pedestrians are allowed to move from point1 to point4 (or vice versa) or from point2 to point3 (or vice versa) (vertical street)
    private const int VERTICAL_TO_HORIZONTAL = 2;//state which signifies that the state will be set to HORIZONTAL soon, cars and pedestrians have the same permissions as in the state VERTICAL (yellow lights are turned on)
    private const int HORIZONTAL = 3;//state in which pedestrians are allowed to move from point1 to point2 (or vice versa) or from point3 to point4 (or vice versa) (horizontal street)
    private const int HORIZONTAL_TO_VERTICAL = 4;//state which signifies that the state will be set to VERTICAL soon, cars and pedestrians have the same permissions as in the state HORIZONTAL (yellow lights are turned on)
    private const int POINT_STATE1 = 1;//label of the waypoint; used to determine where is the pedestrian allowed to go from current point
    private const int POINT_STATE2 = 2;
    private float timer;
    private Transform waypoints;
    private Semaphore_1_2Controller semaphore1;
    private Semaphore_1_2Controller semaphore2;
    private Semaphore_1_2Controller semaphore3;
    private Semaphore_1_2Controller semaphore4;

    public int currentState;//starting state of the intersection
    public float lightDuration;//time between the red and green lights
    public float transitionDuration;//transition between the states (duration of the yellow light)

    /// <summary>
    /// Determines the starting state.
    /// </summary>
    void Start() {
        waypoints = transform.Find("Waypoints");
        semaphore1 = transform.Find("semaphore1").GetComponent<Semaphore_1_2Controller>();
        semaphore2 = transform.Find("semaphore2").GetComponent<Semaphore_1_2Controller>();
        semaphore3 = transform.Find("semaphore3").GetComponent<Semaphore_1_2Controller>();
        semaphore4 = transform.Find("semaphore4").GetComponent<Semaphore_1_2Controller>();

        if(lightDuration <= 0) lightDuration = 10;
        if(transitionDuration <= 0) transitionDuration = 2;
        if(currentState == 0) currentState = VERTICAL;

        manageExitAndRightPoints();

        timer = currentState == HORIZONTAL || currentState == VERTICAL ? lightDuration : transitionDuration;

        switch(currentState) {
            case VERTICAL:
                switchState(VERTICAL);
                break;
            case VERTICAL_TO_HORIZONTAL:
                switchState(VERTICAL_TO_HORIZONTAL);
                break;
            case HORIZONTAL:
                switchState(HORIZONTAL);
                break;
            case HORIZONTAL_TO_VERTICAL:
                switchState(HORIZONTAL_TO_VERTICAL);
                break;
        }
    }

    /// <summary>
    /// When the timer falls bellow 0, the intersection is set to the next state.
    /// </summary>
    void Update() {
        timer -= Time.deltaTime;
        if(timer <= 0) {
            timer = currentState == VERTICAL || currentState == HORIZONTAL ? transitionDuration : lightDuration;
            int newState;
            switch(currentState) {
                case VERTICAL:
                    newState = VERTICAL_TO_HORIZONTAL;
                    break;
                case VERTICAL_TO_HORIZONTAL:
                    newState = HORIZONTAL;
                    break;
                case HORIZONTAL:
                    newState = HORIZONTAL_TO_VERTICAL;
                    break;
                case HORIZONTAL_TO_VERTICAL:
                    newState = VERTICAL;
                    break;
                default:
                    newState = VERTICAL;
                    break;
            }
            switchState(newState);
        }
    }

    /// <summary>
    /// Sets labels of auxiliary waypoints to 1. This means that the cars can reach them only if the 
    /// traffic light is green.
    /// </summary>
    private void manageExitAndRightPoints() {
        PointDescriptor carExit1Point = waypoints.Find("carExit1").GetComponent<PointDescriptor>();
        PointDescriptor carExit2Point = waypoints.Find("carExit2").GetComponent<PointDescriptor>();
        PointDescriptor carExit3Point = waypoints.Find("carExit3").GetComponent<PointDescriptor>();
        PointDescriptor carExit4Point = waypoints.Find("carExit4").GetComponent<PointDescriptor>();


        PointDescriptor carRight1Point = waypoints.Find("carRight1").GetComponent<PointDescriptor>();
        PointDescriptor carRight2Point = waypoints.Find("carRight2").GetComponent<PointDescriptor>();
        PointDescriptor carRight3Point = waypoints.Find("carRight3").GetComponent<PointDescriptor>();
        PointDescriptor carRight4Point = waypoints.Find("carRight4").GetComponent<PointDescriptor>();

        carExit1Point.setLabel(POINT_STATE1);
        carExit2Point.setLabel(POINT_STATE1);
        carExit3Point.setLabel(POINT_STATE1);
        carExit4Point.setLabel(POINT_STATE1);

        carRight1Point.setLabel(POINT_STATE1);
        carRight2Point.setLabel(POINT_STATE1);
        carRight3Point.setLabel(POINT_STATE1);
        carRight4Point.setLabel(POINT_STATE1);
    }

    /// <summary>
    /// Sets the intersection to a new state. (sets appropriate waypoint labels and changes lights). 
    /// </summary>
    /// <param name="newState">Parameter which sets this intersection either to the new state.</param>
    private void switchState(int newState) {
        PointDescriptor point1Descriptor = waypoints.Find("point1").GetComponent<PointDescriptor>();
        PointDescriptor point2Descriptor = waypoints.Find("point2").GetComponent<PointDescriptor>();
        PointDescriptor point3Descriptor = waypoints.Find("point3").GetComponent<PointDescriptor>();
        PointDescriptor point4Descriptor = waypoints.Find("point4").GetComponent<PointDescriptor>();
        switch(newState) {
            case VERTICAL:
            case VERTICAL_TO_HORIZONTAL:
                point1Descriptor.setLabel(POINT_STATE1);
                point2Descriptor.setLabel(POINT_STATE2);
                point3Descriptor.setLabel(POINT_STATE2);
                point4Descriptor.setLabel(POINT_STATE1);
                break;
            case HORIZONTAL:
            case HORIZONTAL_TO_VERTICAL:
                point1Descriptor.setLabel(POINT_STATE1);
                point2Descriptor.setLabel(POINT_STATE1);
                point3Descriptor.setLabel(POINT_STATE2);
                point4Descriptor.setLabel(POINT_STATE2);
                break;
        }
        currentState = newState;
        changeLights(currentState);
        manageDecelerationPoints(currentState);
    }

    /// <summary>
    /// Changes label states of points whose name begins with "carDecelerate". This allows the cars 
    /// to move when the light is green and orders them to stop when it is red.
    /// </summary>
    /// <param name="newState">New intersection state</param>
    private void manageDecelerationPoints(int newState) {
        PointDescriptor carDecelerate1Descriptor = waypoints.Find("carDecelerate1").GetComponent<PointDescriptor>();
        PointDescriptor carDecelerate2Descriptor = waypoints.Find("carDecelerate2").GetComponent<PointDescriptor>();
        PointDescriptor carDecelerate3Descriptor = waypoints.Find("carDecelerate3").GetComponent<PointDescriptor>();
        PointDescriptor carDecelerate4Descriptor = waypoints.Find("carDecelerate4").GetComponent<PointDescriptor>();

        switch(newState) {
            case VERTICAL:
                carDecelerate1Descriptor.setLabel(POINT_STATE1);
                carDecelerate2Descriptor.setLabel(POINT_STATE2);
                carDecelerate3Descriptor.setLabel(POINT_STATE1);
                carDecelerate4Descriptor.setLabel(POINT_STATE2);
                break;
            case HORIZONTAL:
                carDecelerate1Descriptor.setLabel(POINT_STATE2);
                carDecelerate2Descriptor.setLabel(POINT_STATE1);
                carDecelerate3Descriptor.setLabel(POINT_STATE2);
                carDecelerate4Descriptor.setLabel(POINT_STATE1);
                break;
            default:
                carDecelerate1Descriptor.setLabel(POINT_STATE2);
                carDecelerate2Descriptor.setLabel(POINT_STATE2);
                carDecelerate3Descriptor.setLabel(POINT_STATE2);
                carDecelerate4Descriptor.setLabel(POINT_STATE2);
                break;
        }
    }

    /// <summary>
    /// Changes semaphore lihgts to match the current intersection state.
    /// </summary>
    /// <param name="newState">New state of the intersection, it determines light colors.</param>
    private void changeLights(int newState) {
        switch(newState) {
            case VERTICAL:
                setLightsToVertical();
                break;
            case VERTICAL_TO_HORIZONTAL:
                setLigtsToVerticalToHorizontal();
                break;
            case HORIZONTAL:
                setLightsToHorizontal();
                break;
            case HORIZONTAL_TO_VERTICAL:
                setLightsToHorizontalToVertical();
                break;
        }
    }

    /// <summary>
    /// Sets the lights so that both cars and pedestrians are free to move in the vertical street.
    /// </summary>
    private void setLightsToVertical() {
        setCarLightsToVertical();
        setPedestrianLightsToVertical();
    }

    /// <summary>
    /// Sets the lights so that cars are free to move in the vertical street.
    /// </summary>
    private void setCarLightsToVertical() {
        semaphore1.carGreen = true;
        semaphore1.carYellow = false;
        semaphore1.carRed = false;

        semaphore2.carGreen = false;
        semaphore2.carYellow = false;
        semaphore2.carRed = true;

        semaphore3.carGreen = true;
        semaphore3.carYellow = false;
        semaphore3.carRed = false;

        semaphore4.carGreen = false;
        semaphore4.carYellow = false;
        semaphore4.carRed = true;
    }

    /// <summary>
    ///  Sets the lights so that pedestrians are free to move in the vertical street.
    /// </summary>
    private void setPedestrianLightsToVertical() {
        semaphore1.pedestrianVerticalGreen = true;
        semaphore1.pedestrianVerticalRed = false;
        semaphore1.pedestrianHorizontalGreen = false;
        semaphore1.pedestrianHorizontalRed = true;

        semaphore2.pedestrianVerticalGreen = false;
        semaphore2.pedestrianVerticalRed = true;
        semaphore2.pedestrianHorizontalGreen = true;
        semaphore2.pedestrianHorizontalRed = false;

        semaphore3.pedestrianVerticalGreen = true;
        semaphore3.pedestrianVerticalRed = false;
        semaphore3.pedestrianHorizontalGreen = false;
        semaphore3.pedestrianHorizontalRed = true;

        semaphore4.pedestrianVerticalGreen = false;
        semaphore4.pedestrianVerticalRed = true;
        semaphore4.pedestrianHorizontalGreen = true;
        semaphore4.pedestrianHorizontalRed = false;
    }

    /// <summary>
    /// Sets the lights so that both cars and pedestrians are free to move in the horizontal street.
    /// </summary>
    private void setLightsToHorizontal() {
        setCarLightsToHorizontal();
        setPedestrianLightsToHorizontal();
    }

    /// <summary>
    ///  Sets the lights so that cars are free to move in the horizontal street.
    /// </summary>
    private void setCarLightsToHorizontal() {
        semaphore1.carGreen = false;
        semaphore1.carYellow = false;
        semaphore1.carRed = true;

        semaphore2.carGreen = true;
        semaphore2.carYellow = false;
        semaphore2.carRed = false;

        semaphore3.carGreen = false;
        semaphore3.carYellow = false;
        semaphore3.carRed = true;

        semaphore4.carGreen = true;
        semaphore4.carYellow = false;
        semaphore4.carRed = false;
    }

    /// <summary>
    /// Sets the lights so that pedestrians are free to move in the horizontal street.
    /// </summary>
    private void setPedestrianLightsToHorizontal() {
        semaphore1.pedestrianVerticalGreen = false;
        semaphore1.pedestrianVerticalRed = true;
        semaphore1.pedestrianHorizontalGreen = true;
        semaphore1.pedestrianHorizontalRed = false;

        semaphore2.pedestrianVerticalGreen = true;
        semaphore2.pedestrianVerticalRed = false;
        semaphore2.pedestrianHorizontalGreen = false;
        semaphore2.pedestrianHorizontalRed = true;

        semaphore3.pedestrianVerticalGreen = false;
        semaphore3.pedestrianVerticalRed = true;
        semaphore3.pedestrianHorizontalGreen = true;
        semaphore3.pedestrianHorizontalRed = false;

        semaphore4.pedestrianVerticalGreen = true;
        semaphore4.pedestrianVerticalRed = false;
        semaphore4.pedestrianHorizontalGreen = false;
        semaphore4.pedestrianHorizontalRed = true;
    }

    /// <summary>
    /// Manges yellow traffic lights so that they match the current intersection state (VERTICAL_TO_HORIZONTAL).
    /// </summary>
    private void setLigtsToVerticalToHorizontal() {
        semaphore1.carGreen = false;
        semaphore1.carYellow = true;
        semaphore1.carRed = false;

        semaphore2.carGreen = false;
        semaphore2.carYellow = true;
        semaphore2.carRed = true;

        semaphore3.carGreen = false;
        semaphore3.carYellow = true;
        semaphore3.carRed = false;

        semaphore4.carGreen = false;
        semaphore4.carYellow = true;
        semaphore4.carRed = true;
    }

    /// <summary>
    /// Manges yellow traffic lights so that they match the current intersection state (HORIZONTAL_TO_VERTICAL).
    /// </summary>
    private void setLightsToHorizontalToVertical() {
        semaphore1.carGreen = false;
        semaphore1.carYellow = true;
        semaphore1.carRed = true;

        semaphore2.carGreen = false;
        semaphore2.carYellow = true;
        semaphore2.carRed = false;

        semaphore3.carGreen = false;
        semaphore3.carYellow = true;
        semaphore3.carRed = true;

        semaphore4.carGreen = false;
        semaphore4.carYellow = true;
        semaphore4.carRed = false;
    }
}
