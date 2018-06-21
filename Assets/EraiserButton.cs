using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraiserButton : MonoBehaviour, IOculusRaycastEventDefinition{

	// Use this for initialization
	[SerializeField]
	private InkPainter painter;

	private float interval = 0f;
	public float Interval
	{
		get
		{
			return interval;
		}		
	}
	private void Awake()
	{
		if(painter == null)
		{
			painter = FindObjectOfType<InkPainter>();
		}
	}

	public void CatchHittedInfo(RaycastHit info)
	{

	}

	public void TouchedPad(){}

	public void ClickedPad()
	{
		painter.IsErasing = true;			
	}

	public void UpFlicked(){}

	public void DownFlicked(){}

	public void LeftFlicked(){}

	public void RightFlicked(){}

	public void TriggerEntered(){}

	public void GetUpTouchPad(){}

	public void Gaze(){}

	public void UnGaze(){}
	// private void CallChangeIsErasing()
	// {
	// 	Debug.Log("IsErasing is Changed");
	// 	painter.IsErasing = !painter.IsErasing;
	// }

    // public void AttachedEvents()
	// {
	// 	Debug.Log("EraiserButton is AttachedEvents");
	// 	OculusGoInput.Instance.ClickedPad += CallChangeIsErasing;
	// }
	// public void DetachedEvents()
	// {
	// 	Debug.Log("EraiserButton is DetachedEvents");
	// 	OculusGoInput.Instance.ClickedPad -= CallChangeIsErasing;
	// }
}
