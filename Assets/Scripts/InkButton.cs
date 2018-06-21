using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkButton : MonoBehaviour, IOculusRaycastEventDefinition {

	[SerializeField]
	private InkPainter.InkColor myInk;

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

	public void CatchHittedInfo(RaycastHit info){}

	public void TouchedPad(){}

	public void ClickedPad()
	{
		painter.ChangeCurrentColor(myInk);
	}

	public void UpFlicked(){}

	public void DownFlicked(){}

	public void LeftFlicked(){}

	public void RightFlicked(){}

	public void TriggerEntered(){}

	public void GetUpTouchPad(){}

	public void Gaze(){}

	public void UnGaze(){}	
	
	// public void AttachedEvents()
	// {		
	// 	OculusGoInput.Instance.ClickedPad += CallChangeCurrentColor;
	// }
	// public void DetachedEvents()
	// {		
	// 	OculusGoInput.Instance.ClickedPad -= CallChangeCurrentColor;
	// }

	// private void CallChangeCurrentColor()
	// {
	// 	painter.ChangeCurrentColor(myInk);
	// }

}
