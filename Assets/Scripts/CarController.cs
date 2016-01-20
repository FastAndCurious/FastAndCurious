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

	private float currentSteeringAngle;
	public int currentGear = 1;
	private Rigidbody rigidBody;
	public float topSpeed;

	public float CurrentSpeed{ get { return rigidBody.velocity.magnitude * 3.6f; }}

	void Start () {
		wheelColliders [0].attachedRigidbody.centerOfMass = centerOfMass;
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () {
		float accelerate = Input.GetAxis ("Accelerate");
		float steering = Input.GetAxis ("Steering");
		float brake = Input.GetAxis ("Brake");
		float shift = Input.GetAxis ("Shift");
		float camera = Input.GetAxis ("Camera");
		float gears = Input.GetAxis ("Gears");
		float reverse = Input.GetAxis ("Reverse");

		applySteering (steering);
		applyDrive (accelerate, brake);
		applyGearChanging(shift, gears, reverse);
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
		steeringWheel.transform.localRotation = Quaternion.Euler(0,0, -steering*450);
		wheelColliders[0].steerAngle = currentSteeringAngle;
		wheelColliders[1].steerAngle = currentSteeringAngle;
	}

	void applyDrive(float accelerate, float brake) {
		topSpeed = topGearSpeed * currentGear;
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

	void applyGearChanging(float shift, float gears, float reverse) {
		if (shift > 0f) {
			float f = Mathf.Abs (CurrentSpeed / topSpeed);
			float upgearlimit = 0.8f;
			float downgearlimit = (1 / (float)numberOfGears) * currentGear;
		
			if (currentGear > 0 && f < downgearlimit) {
				if (gears < 0f)
					currentGear--;
			}
		
			if (f >= upgearlimit && (currentGear < numberOfGears)) {
				if (gears > 0f)
					currentGear++;
			}

			if (currentGear == 0) {
				if (gears > 0f)
					currentGear++;
				else if (reverse > 0f)
					currentGear--;
			}

			if (currentGear == -1) {
				if (gears > 0f)
					currentGear++;
			}
		}
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
