using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InkBehaviour : MonoBehaviour, IOculusRaycastEventDefinition {

	// Use this for initialization	private InkPainter painter;
	[HideInInspector]
	public Vector3 myCenter;

	private InkPainter painter;

	private bool enableErased = false;

	public Action<Vector3> inkDestroyEvent;

	private bool initialied = false;

	//1フレーム分程度 (1/60 = 0.016f)
	private float interval = 0.016f;
	public float Interval
	{
		get
		{
			return interval;
		}		
	}

	private void Awake()
	{
		StartCoroutine(GetInkPainter());
	}

	private IEnumerator GetInkPainter()
	{
		//Whiteboardが親になるまで待つ		
		yield return null;
		if(painter == null)
		{
			painter = GetComponentInParent<InkPainter>();
			if(painter == null)
			{
				//Debug.Log("Painter null");
			}
			else
			{
				initialied = true;
			}
		}		
	}

	public void CatchHittedInfo(RaycastHit info)
	{
		if(!enableErased)
		{
			return;
		}
		//Debug.Log("enableEraised true");
		DestroyMyself();
	}

	public void TouchedPad()
	{
		if(!initialied)
		{
			return;
		}

		if(!painter.IsErasing)
		{
			//Debug.Log("IsErasing false");
			return;
		}

		enableErased = true;
	}

	public void ClickedPad()
	{
	}

	public void UpFlicked(){}

	public void DownFlicked(){}

	public void LeftFlicked(){}

	public void RightFlicked(){}

	public void TriggerEntered(){}

	public void GetUpTouchPad()
	{
		if(enableErased)
		{
			enableErased = false;
		}
	}

	public void Gaze(){}

	public void UnGaze()
	{

	}

	public void GetNextHittedObj(RaycastHit nextObjInfo){}
	
	private void DestroyMyself()
	{		
		//OculusGoInput.Instance.TouchedPad -= DestroyMyself;

		if(!painter.IsErasing)
		{
			return;
		}

		if(inkDestroyEvent != null)
		{
			inkDestroyEvent(myCenter);
		}
		Destroy(this.gameObject);
	}

	// public void AttachedEvents()
	// {
	// 	// if(!painter.IsErasing)
	// 	// {
	// 	// 	return;
	// 	// }
	// 	// OculusGoInput.Instance.TouchedPad += DestroyMyself;
	// }
	// public void DetachedEvents()
	// {
	// 	// if(!painter.IsErasing)
	// 	// {
	// 	// 	return;
	// 	// }
	// 	// OculusGoInput.Instance.TouchedPad -= DestroyMyself;
	// }
}
