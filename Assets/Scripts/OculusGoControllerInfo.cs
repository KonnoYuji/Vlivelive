using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class OculusGoControllerInfo : MonoBehaviour {

	private StringBuilder data = new StringBuilder();

	private string rotVel;

	[SerializeField]
	private Text uiText;

	[SerializeField]
	private Text rotVelocityText;

	private bool isRunning = false;

	private Vector2 primaryTouchpad;

	public Vector2 PrimaryTouchpad
	{
		get
		{
			return primaryTouchpad;
		}
	}

	private bool isHighAcceraration = false;

	private bool quickReel = false;
	
	private bool quickStretch = false;

	private float passedTime = 0;
	void Update () {

		primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
		data.Length = 0;	
		data.AppendFormat("PrimaryTouchpad: ({0:F2}, {1:F2})\n", primaryTouchpad.x, primaryTouchpad.y);

		if(!isRunning)
		{
			StartCoroutine(GetControllerInfo());
		}			
	}


	private IEnumerator GetControllerInfo()
	{	
		isRunning = true;
		
		OVRInput.Controller activeController = OVRInput.GetActiveController();
		// Quaternion rot = OVRInput.GetLocalControllerRotation(activeController);
		// data.AppendFormat("Orientation: ({0:F2}, {1:F2}, {2:F2}, {3:F2})\n", rot.x, rot.y, rot.z, rot.w);

		Vector3 angVel = OVRInput.GetLocalControllerAngularVelocity(activeController);
		data.AppendFormat("AngVel: ({0:F2}, {1:F2}, {2:F2})\n", angVel.x, angVel.y, angVel.z);
		
		Vector3 angAcc = OVRInput.GetLocalControllerAngularAcceleration(activeController);
		data.AppendFormat("AngAcc: ({0:F2}, {1:F2}, {2:F2})\n", angAcc.x, angAcc.y, angAcc.z);
		
		if(angAcc.x > 100 && !isHighAcceraration)
		{
			rotVel += "AngAcc over 100 : " + angAcc.x + '\n';		
			isHighAcceraration = true;
		}

		if(isHighAcceraration)
		{
			StartCoroutine(CheckQuickReelMotion());
			StartCoroutine(CheckQuickStretchMotion());
		}
		
		// if(angAcc.x > 150 && ( Mathf.Abs(rot.x) <= 0.05f && Mathf.Abs(rot.x) >= 0))
		// {
		// 	rotVel += "AngAcc over 100 : " + angAcc.x + '\n';
		// 	rotVel += "Horizon\n";			
		// }
		// else if(angAcc.x > 150 && ( Mathf.Abs(rot.x) <= 0.55f && Mathf.Abs(rot.x) >= 0.45f))
		// {
		// 	rotVel += "AngAcc over 100 : " + angAcc.x + '\n';
		// 	rotVel += "Vertical\n";			
		// }

		// Vector3 pos = OVRInput.GetLocalControllerPosition(activeController);
		// data.AppendFormat("Position: ({0:F2}, {1:F2}, {2:F2})\n", pos.x, pos.y, pos.z);

		// Vector3 vel = OVRInput.GetLocalControllerVelocity(activeController);
		// data.AppendFormat("Vel: ({0:F2}, {1:F2}, {2:F2})\n", vel.x, vel.y, vel.z);

		// Vector3 acc = OVRInput.GetLocalControllerAcceleration(activeController);
		// data.AppendFormat("Acc: ({0:F2}, {1:F2}, {2:F2})\n", acc.x, acc.y, acc.z);

		// Vector2 secondaryTouchpad = OVRInput.Get(OVRInput.Axis2D.SecondaryTouchpad);
		// data.AppendFormat("SecondaryTouchpad: ({0:F2}, {1:F2})\n", secondaryTouchpad.x, secondaryTouchpad.y);

		// float indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
		// data.AppendFormat("PrimaryIndexTriggerAxis1D: ({0:F2})\n", indexTrigger);

		// float handTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
		// data.AppendFormat("PrimaryHandTriggerAxis1D: ({0:F2})\n", handTrigger);

		if (uiText != null)
		{
			uiText.text = data.ToString();
		}

		if(rotVelocityText)
		{
			rotVelocityText.text = rotVel;
		}
		//yield return new WaitForSeconds(0.25f);
		yield return null;
		isRunning = false;		
	}

	private IEnumerator CheckQuickReelMotion()
	{
		yield return new WaitForSeconds(0.25f);

		var ctrForward = this.transform.forward.normalized;
		data.AppendFormat("ctrForward: ({0:F2}, {1:F2}, {2:F2})\n", ctrForward.x, ctrForward.y, ctrForward.z);
		var dot = Vector3.Dot(ctrForward, Vector3.up);
		var sita = Mathf.Acos(dot) * (180 / 3.14f);

		if((sita <= 20 && sita >= 0))
		{
			quickStretch = false;
			quickReel  = true;
			Debug.Log("QuickReel is true");
		}

		isHighAcceraration = false;		
	}

	private IEnumerator CheckQuickStretchMotion()
	{
		yield return new WaitForSeconds(0.25f);

		var ctrForward = this.transform.forward.normalized;
		data.AppendFormat("ctrForward: ({0:F2}, {1:F2}, {2:F2})\n", ctrForward.x, ctrForward.y, ctrForward.z);
		var dot = Vector3.Dot(ctrForward, Vector3.up);
		var sita = Mathf.Acos(dot) * (180 / 3.14f);

		if(sita >= 70 && 110 >= sita)
		{			
			quickReel  = false;
			quickStretch = true;
			Debug.Log("QuickStretch is true");
		}

		isHighAcceraration = false;
	}
}
