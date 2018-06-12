using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPadEventAttacher : MonoBehaviour, IEventAttacher {

	[SerializeField]
	private Text output;

	[SerializeField]
	private GameObject[] mojis;

	[SerializeField]
	private string[] arrangedInputMojis; // 0 : Clicked, 1 : leftFlicked, 2: UpFlicked, 3 : RightFlicked, 4 : DownFlicked 

	private bool initialized = false;
	
	[SerializeField]
	private bool acceptableAroundMojis = false;

	[SerializeField]
	private Image myImage;
	private void Awake()
	{
		if(mojis == null || mojis.Length < 4 || arrangedInputMojis == null || arrangedInputMojis.Length < 5)
		{
			Destroy(this);
			Debug.Log("Not mmatched terms to use TextPad");
		}

		if(myImage == null)
		{
			myImage = GetComponent<Image>();
		}
	}

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
		myImage.color = new Color(0.5f, 1.0f, 1.0f);
		if(!acceptableAroundMojis)
		{
			return;
		}
		transform.SetAsLastSibling();
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
		myImage.color = new Color(1.0f, 1.0f, 1.0f);
		if(!acceptableAroundMojis)
		{
			return;
		}
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
		output.text += arrangedInputMojis[0];
	}

	private void TouchEvent()
	{

	}

	private void LeftEvent()
	{
		output.text += arrangedInputMojis[1];
	}

	private void UpEvent()
	{
		output.text +=  arrangedInputMojis[2];
	}

	private void RightEvent()
	{
		output.text += arrangedInputMojis[3];
	}
	
	private void DownEvent()
	{
		output.text += arrangedInputMojis[4];
	}



}
