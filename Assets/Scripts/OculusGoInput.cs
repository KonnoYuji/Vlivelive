using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OculusGoInput : MonoBehaviour {

	static private OculusGoInput _instance;
	static public OculusGoInput Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<OculusGoInput>();
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

	public event InputEvent GetUpTouchPad;

	private bool isClickPad = false;

	private bool isTouched = false;

	private bool isUpFlicked = false;
	private bool isDownFlicked = false;
	private bool isLeftFlicked = false;
	private bool isRightFlicked = false;

	private bool isTrigger = false;

	private bool isGetUpTouchPad = false;

	public float interval = 0.25f;

	void Update () {
#if !UNITY_EDITOR && UNITY_ANDROID
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
		else if(OVRInput.GetUp(OVRInput.Touch.One))
		{
			if(!isGetUpTouchPad)
			{
				StartCoroutine(GetUpTouchPadInteral());
			}
		}
#elif UNITY_EDITOR && UNITY_STANDALONE		
		if (Input.GetKeyDown(KeyCode.Q))
        {
			if(!isClickPad)
			{				
				StartCoroutine(ClickedPadInternal());				
			}
        }
        else if(Input.GetMouseButton(0))
        {
			Debug.Log("Touching is True");
			if(!isTouched)
			{				
				StartCoroutine(TouchedPadInternal());
			}			
        }
        else if(Input.GetMouseButtonDown(1))
        {
            if(!isTrigger)
			{				
				StartCoroutine(TriggerEnteredInternal());
			}                 
        }
		else if(Input.GetKeyDown(KeyCode.W))
		{
			if(!isUpFlicked)
			{
				StartCoroutine(UpFlickedInternal());
			}
		}
		else if(Input.GetKeyDown(KeyCode.S))
		{
			if(!isDownFlicked)
			{				
				StartCoroutine(DownFlickedInternal());
			}
		}
		else if(Input.GetKeyDown(KeyCode.A))
		{
			if(!isLeftFlicked)
			{
				StartCoroutine(LeftFlickedInternal());
			}

		}
		else if(Input.GetKeyDown(KeyCode.D))
		{
			if(!isRightFlicked)
			{
				StartCoroutine(RightFlickedInternal());
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(!isGetUpTouchPad)
			{
				StartCoroutine(GetUpTouchPadInteral());
			}
		}
#endif	
	}
	private IEnumerator ClickedPadInternal()
	{
		if(ClickedPad == null)
		{
			yield break;
		}   
        
		isClickPad = true;
		ClickedPad();	
		
		yield return new WaitForSeconds(interval);
        
		isClickPad = false;
	}
	
	private IEnumerator TouchedPadInternal()
	{
		if(TouchedPad == null)
		{
			//Debug.Log("TouchedPad is Null");
			yield break;
			
		}

		isTouched = true;
		TouchedPad();	
		yield return new WaitForSeconds(interval);

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
		yield return new WaitForSeconds(interval);

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
		yield return new WaitForSeconds(interval);

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
		yield return new WaitForSeconds(interval);

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
		yield return new WaitForSeconds(interval);

		isRightFlicked = false;	
	}

	private IEnumerator GetUpTouchPadInteral()
	{
		if(GetUpTouchPad == null)
		{
			yield break;
		}
		
		isGetUpTouchPad = true;
		GetUpTouchPad();
		yield return new WaitForSeconds(interval);

		isGetUpTouchPad = false; 
	}
	private IEnumerator TriggerEnteredInternal()
	{
		if(TriggerEntered == null)
		{
			yield break;
		}

		isTrigger = true;
		TriggerEntered();
		yield return new WaitForSeconds(interval);

		isTrigger = false;
	}
}
