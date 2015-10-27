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

public class SimpleCarController : MonoBehaviour {

	public List<AxleInfo> axleInfos; // the information about each individual axle
	public float maxMotorTorque; // maximum torque the motor can apply to wheel
	public float maxSteeringAngle; // maximum steer angle the wheel can have
	[Range(-1,5)]
	public int gear = 0;

	public void FixedUpdate()
	{
		float motor = maxMotorTorque * Input.GetAxis("Gas Pedal");
		float steering = maxSteeringAngle * Input.GetAxis("Wheel");

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor) {
					axleInfo.leftWheel.motorTorque = motor * gear;
					axleInfo.rightWheel.motorTorque = motor * gear;
				
			}
		}
	}
}