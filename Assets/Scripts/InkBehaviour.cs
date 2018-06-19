using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InkBehaviour : MonoBehaviour, IOculusEventDefinition {

	// Use this for initialization	private InkPainter painter;
	[HideInInspector]
	public Vector3 myCenter;

	private InkPainter painter;

	private bool enableErased = false;

	public Action<Vector3> inkDestroyEvent;

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
			painter = FindObjectOfType<InkPainter>();
		}
	}

	public void CatchHittedInfo(RaycastHit info)
	{
		if(!enableErased)
		{
			return;
		}
		DestroyMyself();
	}

	public void TouchedPad()
	{
		if(!painter.IsErasing)
		{
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
	private void DestroyMyself()
	{		
		//OculusGoInput.Instance.TouchedPad -= DestroyMyself;

		if(!painter.IsErasing)
		{
			return;
		}

		//Debug.Log("DestroyMyself");	
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
