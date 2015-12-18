using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

	[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4] ;
	[SerializeField] private Vector3 centerOfMass = new Vector3(0, 0, 0);
	[SerializeField] private float maxSteeringAngle = 30f;
	[SerializeField] private float maxTorque = 1000f;
	[Range(0, 1)] [SerializeField] private float traction = 0f;
	[SerializeField] private float maxReverseTorque = 10f;
	[SerializeField] private float maxBrakeTorque = 500f;
	//[SerializeField] private float maxHandbrakeTorque = 0f;
	[SerializeField] private float downForce = 100f;
	[SerializeField] private float topGearSpeed = 30;
	[SerializeField] private int numberOfGears = 5;
	[SerializeField] private float slipLimit = 0f;

	[SerializeField] private GameObject steeringWheel = null;
	[SerializeField] private Camera playerCamera = null;

	private float currentSteeringAngle;
	private int currentGear = 1;
	private float gearFactor;
	private float currentTorque;
	private Rigidbody rigidBody;
	private float topSpeed;

	public float BrakeInput { get; private set; }
	public float CurrentSteerAngle{ get { return currentSteeringAngle; }}
	public float CurrentSpeed{ get { return rigidBody.velocity.magnitude * 3.6f; }}
	public float Revs { get; private set; }
	public float AccelInput { get; private set; }

	void Start () {
		wheelColliders [0].attachedRigidbody.centerOfMass = centerOfMass;
		//maxHandbrakeTorque = float.MaxValue;
		rigidBody = GetComponent<Rigidbody> ();
		currentTorque = maxTorque - (traction * maxTorque);
	}

	void FixedUpdate () {
		float accelerate = Input.GetAxis ("Accelerate");
		float steering = Input.GetAxis ("Steering");
		float brake = Input.GetAxis ("Brake");
		float shift = Input.GetAxis ("Shift");
		float camera = Input.GetAxis ("Camera");
		float gears = Input.GetAxis ("Gears");

		controlCar (accelerate, steering, brake, shift, camera, gears);
		displayInfo ();
	}

	void controlCar(float accelerate, float steering, float brake, float shift, float camera, float gears) {
		topSpeed = topGearSpeed * currentGear;
		applySteering (steering);
		applyDrive (accelerate, brake);
		applyGearChanging(shift, gears);
		addDownForce ();
		tractionControl();
		lookAround (camera);
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
		steeringWheel.transform.localRotation = new Quaternion (0, 0, -currentSteeringAngle, 50);
		wheelColliders[0].steerAngle = currentSteeringAngle;
		wheelColliders[1].steerAngle = currentSteeringAngle;
	}

	void applyDrive(float accelerate, float brake) {
		wheelColliders[0].motorTorque = wheelColliders[1].motorTorque = accelerate * (currentTorque / 2f);
		for (int i = 0; i < 4; i++)
		{
			if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, rigidBody.velocity) < 50f)
			{
				wheelColliders[i].brakeTorque = maxBrakeTorque * brake;
			}
			else if (brake > 0)
			{
				wheelColliders[i].brakeTorque = 0f;
				wheelColliders[i].motorTorque = -maxReverseTorque*brake;
			}
		}
		capSpeed ();
	}

	void capSpeed() {
		if (CurrentSpeed > topSpeed)
			rigidBody.velocity = (topSpeed/3.6f) * rigidBody.velocity.normalized;
	}

	void applyGearChanging(float shift, float gears) {
		if (shift > 0f) {
			float f = Mathf.Abs (CurrentSpeed / topSpeed);
			float upgearlimit = (1 / (float)numberOfGears) * (currentGear + 1);
			float downgearlimit = (1 / (float)numberOfGears) * currentGear;
		
			if (currentGear > 0 && f < downgearlimit) {
				if (gears < 0f)
					currentGear--;
			}
		
			if (f > upgearlimit && (currentGear < (numberOfGears - 1))) {
				if (gears > 0f)
					currentGear++;
			}

			if (currentGear == 0)
				if (gears > 0f)
					currentGear++;
		}
		calculateGearFactor ();
	}

	void calculateGearFactor()
	{
		float f = (1/(float) numberOfGears);
		var targetGearFactor = Mathf.InverseLerp(f*currentGear, f*(currentGear + 1), Mathf.Abs(CurrentSpeed/topSpeed));
		gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime*5f);
	}

	void addDownForce() {
		wheelColliders[0].attachedRigidbody.AddForce(-transform.up*downForce*wheelColliders[0].attachedRigidbody.velocity.magnitude);
	}

	void tractionControl() {
		WheelHit wheelHit;
		wheelColliders[0].GetGroundHit(out wheelHit);
		adjustTorque(wheelHit.forwardSlip);
		
		wheelColliders[1].GetGroundHit(out wheelHit);
		adjustTorque(wheelHit.forwardSlip);
	}

	void adjustTorque(float forwardSlip) {
		if (forwardSlip >= slipLimit && currentTorque >= 0)
		{
			currentTorque -= 10 * traction;
		}
		else
		{
			currentTorque += 10 * traction;
			if (currentTorque > maxTorque)
			{
				currentTorque = maxTorque;
			}
		}
	}

	void lookAround(float camera) {
		float viewAngle = 10;
		if (camera > 0.0f)
			playerCamera.transform.Rotate (new Vector3 (0, viewAngle, 0));
		else if (camera < 0.0f)
			playerCamera.transform.Rotate (new Vector3 (0, -viewAngle, 0));
	}

	void displayInfo() {
		Debug.Log ("Gear: " + currentGear + "; Current speed = " + CurrentSpeed + "; Motor = " + gearFactor * 3000);
	}
}
