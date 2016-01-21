using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

	[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4] ;
	[SerializeField] private Vector3 centerOfMass = new Vector3(0, 0, 0);
	[SerializeField] private float maxSteeringAngle = 30f;
	[SerializeField] private float currentTorque = 1000f;
	[SerializeField] private float maxReverseTorque = 200f;
	[SerializeField] private float maxBrakeTorque = 1000f;
	[SerializeField] private float downForce = 100f;
	[SerializeField] public float topGearSpeed = 25;
	[SerializeField] public int numberOfGears = 5;

	[SerializeField] private GameObject steeringWheel = null;
	[SerializeField] private Camera playerCamera = null;

    private bool[] lastGears = { false, false, false, false, false, false, false };
	private float currentSteeringAngle;
	public int currentGear = 0;
	private Rigidbody rigidBody;
	public float topSpeed;

	public float CurrentSpeed{ get { return rigidBody.velocity.magnitude * 3.6f; }}
    public float TopSpeed { get { return currentGear*topGearSpeed; } }
    public int upaljenAuto = 0;

    void Start () {
		wheelColliders [0].attachedRigidbody.centerOfMass = centerOfMass;
		rigidBody = GetComponent<Rigidbody> ();
        
	}

	void FixedUpdate () {
		float accelerate =(float)( -(Input.GetAxis ("Accelerate")-1)*0.9 + 0.1);
        //Debug.Log("ubrzanje: " + accelerate);
        float steering = Input.GetAxis ("Steering");
		float brake = -(Input.GetAxis ("Brake") - 1)/2;
        //Debug.Log("kocnica: " + brake);
		float shift = -(Input.GetAxis ("Shift") - 1)/2;
		float camera = Input.GetAxis ("Camera");
  
        bool gumbUpaliAuto = Input.GetButtonDown("upaliAuto");
        Debug.Log("upali Auto" + gumbUpaliAuto + "\nupaljenAuto" +upaljenAuto);
        if (gumbUpaliAuto == true && upaljenAuto == 0)
            upaliAuto();

        

        bool[] gears = { false, false, false, false, false, false, false };
        gears[0] = Input.GetButton("Gear1");
        gears[1] = Input.GetButton("Gear2");
        gears[2] = Input.GetButton("Gear3");
        gears[3] = Input.GetButton("Gear4");
        gears[4] = Input.GetButton("Gear5");
        gears[5] = Input.GetButton("Gear6");
        gears[6] = Input.GetButton("Gear7");


        applySteering(steering);
        applyGearChanging(shift, gears);

        if (upaljenAuto == 0 || currentGear == 0 || shift > 0.5)
            accelerate = 0;

        applyDrive (accelerate, brake);
        
        lookAround (camera);

        for (int i = 0; i < 7; i++)
            lastGears[i] = gears[i];
	}

	void applySteering(float steering) {
		for (int i = 0; i < 4; i++)
		{
			Quaternion quat;
			Vector3 position;
			wheelColliders[i].GetWorldPose(out position, out quat);
			wheelColliders[i].transform.transform.rotation = quat;
		}
		currentSteeringAngle = steering * maxSteeringAngle;
		steeringWheel.transform.localRotation = Quaternion.Euler(0,0, -steering*450);
		wheelColliders[0].steerAngle = currentSteeringAngle;
		wheelColliders[1].steerAngle = currentSteeringAngle;
	}

	void applyDrive(float accelerate, float brake) {
        if (currentGear != 0)
            topSpeed = topGearSpeed * currentGear;
        else
            topSpeed = topGearSpeed * numberOfGears;
        //Debug.Log(accelerate);
        wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = 0f;
		if (currentGear == -1) {
			wheelColliders [0].motorTorque = wheelColliders [1].motorTorque = -maxReverseTorque * accelerate;
			for (int i = 0; i < 2; i++) {
				if (brake > 0) {
					wheelColliders [i].brakeTorque = maxBrakeTorque * brake;
					wheelColliders [i].motorTorque = 0f;
				}
			}
		} else {
			wheelColliders [0].motorTorque = wheelColliders [1].motorTorque = currentTorque * accelerate;
			for (int i = 0; i < 2; i++) {
				if (CurrentSpeed > 5 && Vector3.Angle (transform.forward, rigidBody.velocity) < 50f) {
					wheelColliders [i].brakeTorque = maxBrakeTorque * brake;
				} else if (brake > 0) {
					wheelColliders [i].brakeTorque = maxBrakeTorque * brake;
					wheelColliders [i].motorTorque = 0f;
				}
			}
			if (CurrentSpeed > topSpeed)
				rigidBody.velocity = (topSpeed / 3.6f) * rigidBody.velocity.normalized;
		}
	}

	void applyGearChanging(float shift, bool[] gears) {
        //Debug.Log("kvacilo: " + shift);
        bool neutral = true;
        for (int i = 0; i < 7; i++)
            if (gears[i]) neutral = false;

        if (neutral || gears[5] == true)
            currentGear = 0;


        if (shift > 0.5) { 
           for (int i = 0; i < numberOfGears; i++)
                {
                    if (gears[i]) { 
                        currentGear = i + 1;
                        if (CurrentSpeed < i * topGearSpeed * 0.8 - 10)
                            ugasiAuto();
                   
                    }
                }
            if (gears[numberOfGears + 1])
            {
                currentGear = -1;
            }
        }
    }

    void ugasiAuto()
    {
        upaljenAuto = 0;
    }

    void upaliAuto()
    {
        upaljenAuto = 1;
    }

    void lookAround(float camera) {
		float viewAngle = 0.5f;
		if (camera > 0.0f)
			playerCamera.transform.localRotation = new Quaternion (0, viewAngle, 0, 1);
		else if (camera < 0.0f)
			playerCamera.transform.localRotation = new Quaternion (0, -viewAngle, 0, 1);
		else
			playerCamera.transform.localRotation = new Quaternion (0, 0, 0, 1);
	}
}
