using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class OculusGoControllerInfo : MonoBehaviour {

	private StringBuilder data;

	[SerializeField]
	private Text uiText;
	void Update () {

		OVRInput.Controller activeController = OVRInput.GetActiveController();

		data.Length = 0;
		byte recenterCount = OVRInput.GetControllerRecenterCount();
		data.AppendFormat("RecenterCount: {0}\n", recenterCount);

		byte battery = OVRInput.GetControllerBatteryPercentRemaining();
		data.AppendFormat("Battery: {0}\n", battery);

		float framerate = OVRPlugin.GetAppFramerate();
		data.AppendFormat("Framerate: {0:F2}\n", framerate);

		string activeControllerName = activeController.ToString();
		data.AppendFormat("Active: {0}\n", activeControllerName);

		string connectedControllerNames = OVRInput.GetConnectedControllers().ToString();
		data.AppendFormat("Connected: {0}\n", connectedControllerNames);


		Quaternion rot = OVRInput.GetLocalControllerRotation(activeController);
		data.AppendFormat("Orientation: ({0:F2}, {1:F2}, {2:F2}, {3:F2})\n", rot.x, rot.y, rot.z, rot.w);

		Vector3 angVel = OVRInput.GetLocalControllerAngularVelocity(activeController);
		data.AppendFormat("AngVel: ({0:F2}, {1:F2}, {2:F2})\n", angVel.x, angVel.y, angVel.z);

		Vector3 angAcc = OVRInput.GetLocalControllerAngularAcceleration(activeController);
		data.AppendFormat("AngAcc: ({0:F2}, {1:F2}, {2:F2})\n", angAcc.x, angAcc.y, angAcc.z);

		Vector3 pos = OVRInput.GetLocalControllerPosition(activeController);
		data.AppendFormat("Position: ({0:F2}, {1:F2}, {2:F2})\n", pos.x, pos.y, pos.z);

		Vector3 vel = OVRInput.GetLocalControllerVelocity(activeController);
		data.AppendFormat("Vel: ({0:F2}, {1:F2}, {2:F2})\n", vel.x, vel.y, vel.z);

		Vector3 acc = OVRInput.GetLocalControllerAcceleration(activeController);
		data.AppendFormat("Acc: ({0:F2}, {1:F2}, {2:F2})\n", acc.x, acc.y, acc.z);

		Vector2 primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		data.AppendFormat("PrimaryTouchpad: ({0:F2}, {1:F2})\n", primaryTouchpad.x, primaryTouchpad.y);

		Vector2 secondaryTouchpad = OVRInput.Get(OVRInput.Axis2D.SecondaryTouchpad);
		data.AppendFormat("SecondaryTouchpad: ({0:F2}, {1:F2})\n", secondaryTouchpad.x, secondaryTouchpad.y);

		float indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
		data.AppendFormat("PrimaryIndexTriggerAxis1D: ({0:F2})\n", indexTrigger);

		float handTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
		data.AppendFormat("PrimaryHandTriggerAxis1D: ({0:F2})\n", handTrigger);

		if (uiText != null)
		{
			uiText.text = data.ToString();
		}		
	}
}
