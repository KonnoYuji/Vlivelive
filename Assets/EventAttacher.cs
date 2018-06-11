using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventAttacher : MonoBehaviour {

	[SerializeField]
	private Text output;

	[SerializeField]
	private GameObject[] mojis;

	private bool initialized = false;
	// Use this for initialization
	public void AttachEvents()
	{
		if(initialized)
		{
			return;
		}
		Gaze();
		OculusGoInputTest.Instance.ClickedPad += ClickEvent;
		OculusGoInputTest.Instance.TouchedPad += TouchEvent;
		OculusGoInputTest.Instance.UpFlicked += UpEvent;
		OculusGoInputTest.Instance.DownFlicked += DownEvent;
		OculusGoInputTest.Instance.LeftFlicked += LeftEvent;
		OculusGoInputTest.Instance.RightFlicked += RightEvent;
		initialized = true;
	}

	public void DetachEvents()
	{
		if(!initialized)
		{
			return;
		}
		UnGaze();
		OculusGoInputTest.Instance.ClickedPad -= ClickEvent;
		OculusGoInputTest.Instance.TouchedPad -= TouchEvent;
		OculusGoInputTest.Instance.UpFlicked -= UpEvent;
		OculusGoInputTest.Instance.DownFlicked -= DownEvent;
		OculusGoInputTest.Instance.LeftFlicked -= LeftEvent;
		OculusGoInputTest.Instance.RightFlicked -= RightEvent;
		initialized = false;
	}

	private void Gaze()
	{
		for(int i=0; i<mojis.Length; i++)
		{
			if(!mojis[i].activeSelf)
			{
				mojis[i].SetActive(true);
			}
		}
	}

	private void UnGaze()
	{
		for(int i=0; i<mojis.Length; i++)
		{
			if(mojis[i].activeSelf)
			{
				mojis[i].SetActive(false);
			}
		}
	}

	private void ClickEvent()
	{
		output.text += "あ";
	}

	private void TouchEvent()
	{

	}

	private void UpEvent()
	{
		output.text += "い";
	}

	private void DownEvent()
	{
		output.text += "え";
	}

	private void LeftEvent()
	{
		output.text += "お";
	}

	private void RightEvent()
	{
		output.text += "う";
	}
}
