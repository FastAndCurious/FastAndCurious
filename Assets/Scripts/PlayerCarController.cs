using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor; // is this wheel attached to motor?
	public bool steering; // does this wheel apply steer angle?
}

public class PlayerCarController : MonoBehaviour {
	public Camera playerCamera;
	public GameObject steeringWheel;
	public List<AxleInfo> axleInfos;
	public float maxMotorTorque, maxSteeringAngle, maxRPM = 500.0f;
	[Range(-1,5)]
	public int gear = 1;

	// Debug info
	public float rpm, speed, motor;

	public void applyGas() {
		float gas = maxMotorTorque * Input.GetAxis ("Gas Pedal");
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.motor) {
				if(rpm < maxRPM * gear) {
					axleInfo.leftWheel.motorTorque = gas;
					axleInfo.rightWheel.motorTorque = gas;
				}
				else {
					axleInfo.leftWheel.motorTorque = 0.0f;
					axleInfo.rightWheel.motorTorque = 0.0f;
				}
			}
		}
	}

	public void applyBrake() {
		float brake = maxMotorTorque * Input.GetAxis ("Brake Pedal");
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.motor) {
				axleInfo.leftWheel.brakeTorque = brake;
				axleInfo.rightWheel.brakeTorque = brake;
			}
		}
	}

	public void applyRotation()
	{		
		float steering = maxSteeringAngle * Input.GetAxis("Wheel");
		steeringWheel.transform.rotation = new Quaternion (0, 0, -steering, 10);
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}

			Transform visualLeftWheel = axleInfo.leftWheel.transform;
			Transform visualRightWheel = axleInfo.rightWheel.transform;
			
			Vector3 position;
			Quaternion rotation;
			axleInfo.leftWheel.GetWorldPose(out position, out rotation);
			visualLeftWheel.transform.rotation = rotation;

			axleInfo.rightWheel.GetWorldPose(out position, out rotation);
			visualRightWheel.transform.rotation = rotation;
		}
	}

	public void switchGear() {
		if (Input.GetAxis ("Switch Pedal") > 0.0f) {
			if (rpm > ((gear - 1) * maxRPM + 0.8f * maxRPM)) {
				if (Input.GetAxis ("Gear " + (gear + 1)) > 0.0f && gear != 5)
					gear++;
			}
			else if (rpm < ((gear - 1) * maxRPM + 0.2f * maxRPM)) {
				if (Input.GetAxis ("Gear " + (gear - 1)) > 0.0f && gear != 0)
					gear--;
			}
		}
	}

	public void lookAround() {
		float viewAngle = 10;
		if (Input.GetAxis ("Camera") > 0.0f)
			playerCamera.transform.Rotate (new Vector3 (0, viewAngle, 0));
		else if (Input.GetAxis ("Camera") < 0.0f)
			playerCamera.transform.Rotate (new Vector3 (0, -viewAngle, 0));
	}

	public void displayInfo() {
		rpm = axleInfos [0].leftWheel.rpm;
		speed = Mathf.Round(rpm / 20.0f);
		motor = Mathf.Round(4 * (rpm - ((gear - 1) * maxRPM)) + 1000.0f);
		if (motor < 200.0f)
			motor = 200.0f;
	}

	public void FixedUpdate()
	{
		lookAround ();
		applyRotation ();
		applyGas ();
		applyBrake ();
		switchGear ();

		displayInfo ();
	}
}