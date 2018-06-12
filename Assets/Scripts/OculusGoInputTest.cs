using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OculusGoInputTest : MonoBehaviour {

	static private OculusGoInputTest _instance;
	static public OculusGoInputTest Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<OculusGoInputTest>();
			}
			return _instance;
		}
	}

	public delegate void InputEvent();

	public event InputEvent TouchedPad;

	public event InputEvent ClickedPad;

	public event InputEvent UpFlicked;

	public event InputEvent DownFlicked;

	public event InputEvent LeftFlicked;

	public event InputEvent RightFlicked;

	public event InputEvent TriggerEntered;

	private bool isClickPad = false;

	private bool isTouched = false;

	private bool isUpFlicked = false;
	private bool isDownFlicked = false;
	private bool isLeftFlicked = false;
	private bool isRightFlicked = false;

	private bool isTrigger = false;

	void Update () {

        if (OVRInput.Get(OVRInput.Button.One))
        {
			if(!isClickPad)
			{
				StartCoroutine(ClickedPadInternal());				
			}
        }
        else if(OVRInput.Get(OVRInput.Touch.One))
        {
			if(!isTouched)
			{
				StartCoroutine(TouchedPadInternal());
			}			
        }
        else if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if(!isTrigger)
			{
				StartCoroutine(TriggerEnteredInternal());
			}                 
        }
		else if(OVRInput.Get(OVRInput.Button.Up))
		{
			if(!isUpFlicked)
			{
				StartCoroutine(UpFlickedInternal());
			}
		}
		else if(OVRInput.Get(OVRInput.Button.Down))
		{
			if(!isDownFlicked)
			{
				StartCoroutine(DownFlickedInternal());
			}
		}
		else if(OVRInput.Get(OVRInput.Button.Left))
		{
			if(!isLeftFlicked)
			{
				StartCoroutine(LeftFlickedInternal());
			}

		}
		else if(OVRInput.Get(OVRInput.Button.Right))
		{
			if(!isRightFlicked)
			{
				StartCoroutine(RightFlickedInternal());
			}
		}
	}

	private IEnumerator ClickedPadInternal()
	{
		if(ClickedPad == null)
		{
			yield break;
		}   
        
		isClickPad = true;
		ClickedPad();	
		
		yield return new WaitForSeconds(0.25f);
        
		isClickPad = false;
	}
	
	private IEnumerator TouchedPadInternal()
	{
		if(TouchedPad == null)
		{
			yield break;
			
		}

		isTouched = true;
		TouchedPad();	
		yield return new WaitForSeconds(0.25f);

		isTouched = false;
	}

	private IEnumerator UpFlickedInternal()
	{
		if(UpFlicked == null)
		{
			yield break;
		}

		isUpFlicked = true;
		UpFlicked();		
		yield return new WaitForSeconds(0.25f);

		isUpFlicked = false;
	}

	private IEnumerator DownFlickedInternal()
	{
		if(DownFlicked == null)
		{
			yield break;
		}

		isDownFlicked = true;
		DownFlicked();		
		yield return new WaitForSeconds(0.25f);

		isDownFlicked = false;
	}

	private IEnumerator LeftFlickedInternal()
	{
		if(LeftFlicked == null)
		{
			yield break;
		}

		isLeftFlicked = true;
		LeftFlicked();
		yield return new WaitForSeconds(0.25f);

		isLeftFlicked = false;
	}

	private IEnumerator RightFlickedInternal()
	{
		if(RightFlicked == null)
		{
			yield break;
		}

		isRightFlicked = true;		
		RightFlicked();		
		yield return new WaitForSeconds(0.25f);

		isRightFlicked = false;	
	}

	private IEnumerator TriggerEnteredInternal()
	{
		if(TriggerEntered == null)
		{
			yield break;
		}

		isTrigger = true;
		TriggerEntered();
		yield return new WaitForSeconds(0.25f);

		isTrigger = false;
	}
}
