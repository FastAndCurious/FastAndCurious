/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor; // is this wheel attached to motor?
	public bool steering; // does this wheel apply steer angle?
}

public class SimpleCarController : MonoBehaviour {

	public List<AxleInfo> axleInfos; // the information about each individual axle
	public float maxMotorTorque; // maximum torque the motor can apply to wheel
	public float maxSteeringAngle; // maximum steer angle the wheel can have
	[Range(-1,5)]
	public int gear = 1;

	// Debug info
	public float rpm, speed;

	public void applyGas(float gas) {
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.motor) {
				if(rpm < 500.0f * gear) {
					axleInfo.leftWheel.motorTorque = gas;
					axleInfo.rightWheel.motorTorque = gas;
				}
				else {
					axleInfo.leftWheel.motorTorque = 0;
					axleInfo.rightWheel.motorTorque = 0;
				}
			}
		}
	}

	public void applyBrake(float brake) {
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.motor) {
				axleInfo.leftWheel.brakeTorque = brake;
				axleInfo.rightWheel.brakeTorque = brake;
			}
		}
	}

	public void applyRotation(float steering)
	{		
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

	public void switchGearUp() {
		if (Input.GetAxis ("Gear " + (gear + 1)) > 0.0f && gear != 5)
			gear++;
	}

	public void switchGearDown() {
		if (Input.GetAxis ("Gear " + (gear - 1)) > 0.0f && gear != 0)
			gear--;
	}

	public void FixedUpdate()
	{
		float motor = maxMotorTorque * Input.GetAxis ("Gas Pedal");
		float steering = maxSteeringAngle * Input.GetAxis("Wheel");
		float brake = maxMotorTorque * Input.GetAxis ("Brake Pedal");

		applyRotation (steering);
		applyGas (motor);
		applyBrake (brake);

		if (Input.GetAxis ("Switch Pedal") > 0.0f) {
			if ( rpm > ((gear-1)*500 + 425) )
				switchGearUp();
			else if ( rpm < ((gear-1)*500 + 150) )
				switchGearDown();
		}

		rpm = axleInfos [0].leftWheel.rpm;
		speed = rpm / 20;
	}
}*/