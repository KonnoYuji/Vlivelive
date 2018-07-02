using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;
using System.Linq;
public class VrGrabbableIconElement : MonoBehaviour, IOculusRaycastEventDefinition {

	private float interval = 0f;
	public float Interval
	{
		get
		{
			return interval;
		}		
	}

	[SerializeField]
	protected VrgGrabbable myGrabbable;

	[SerializeField]
	private float sizeChangedTime = 0.5f;
	
	private Vector3 maxScale;

	private float maxRate = 1.2f;

	private Vector3 defaultScale;

	private Coroutine ChangeIconLargerRoutine = null;

	private Coroutine ChangeIconSmallerRoutine = null;

	public bool isWatchedGrabble = false;

	private void Awake()
	{
		if(!myGrabbable)
		{
			myGrabbable = GetComponentInParent<VrgGrabbable>();
		}

		defaultScale = this.transform.localScale;
		maxScale = defaultScale * maxRate;		

		// Debug.LogFormat("defaultScale X : {0} Y : {1} Z : {2}", defaultScale.x, defaultScale.y, defaultScale.z);
		// Debug.LogFormat("maxScale X : {0} Y : {1} Z : {2}", maxScale.x, maxScale.y, maxScale.z);
	}

	// Use this for initialization
	public void Gaze()
	{
		if(ChangeIconSmallerRoutine != null)
		{
			StopCoroutine(ChangeIconSmallerRoutine);
			ChangeIconSmallerRoutine = null;
		}

		if(ChangeIconLargerRoutine != null)
		{
			return;
		}

		if(transform.parent.gameObject.activeSelf)
		{
			ChangeIconLargerRoutine = StartCoroutine(ChangeIconLarger());
			return;
		}
	}

	public void UnGaze()
	{
		myGrabbable.isWatchedFuncPanel = false;

		if(ChangeIconLargerRoutine != null)
		{
			StopCoroutine(ChangeIconLargerRoutine);
			ChangeIconLargerRoutine = null;
		}

		if(ChangeIconSmallerRoutine != null)
		{
			return;
		}	

		if(transform.parent.gameObject.activeSelf)
		{
			ChangeIconSmallerRoutine = StartCoroutine(ChangeIconSmaller());
		}		

		if(!isWatchedGrabble)
		{
			transform.parent.gameObject.SetActive(false);	
						
		}		
	}

	 public void TriggerEntered()
	 {
		 OnClicked();
	 }

	protected IEnumerator ChangeIconLarger()
	{	
		float passedTime = 0;
		var startScale = this.transform.localScale;
		if(startScale == maxScale)
		{
			yield break;
		}

		while(passedTime < sizeChangedTime)
		{
			passedTime += Time.deltaTime;
			var nextScale = Vector3.Lerp(startScale, maxScale, passedTime*2);
			this.GetComponent<RectTransform>().localScale = nextScale;	
			//Debug.LogFormat("Larger Next Scale X : {0} Y : {1} Z : {2}", nextScale.x, nextScale.y, nextScale.z);		
			yield return null;
		}	

		ChangeIconLargerRoutine = null;	 		
	}

	protected IEnumerator ChangeIconSmaller()
	{		
		float passedTime = 0;
		var startScale = this.transform.localScale;

		if(startScale == defaultScale)
		{
			yield break;
		}

		while(passedTime < sizeChangedTime)
		{
			passedTime += Time.deltaTime;
			var nextScale = Vector3.Lerp(startScale, defaultScale, passedTime*2);
			this.GetComponent<RectTransform>().localScale = nextScale;
			//Debug.LogFormat("Smaller Next Scale X : {0} Y : {1} Z : {2}", nextScale.x, nextScale.y, nextScale.z);	

			yield return null;
		}

		ChangeIconSmallerRoutine = null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		this.GetComponent<RectTransform>().localScale = defaultScale;
		isWatchedGrabble = false;
	}
	
	protected virtual void OnClicked()
	{
		
	}

	 public void CatchHittedInfo(RaycastHit info){}

	 public void TouchedPad(){}

	 public void ClickedPad(){}

	 public void UpFlicked(){}

	 public void DownFlicked(){}

	 public void LeftFlicked(){}

	 public void RightFlicked(){}

	 public void GetUpTouchPad(){}

	public void GetNextHittedObj(RaycastHit nextObjInfo)
	{
		var obj = nextObjInfo.collider.gameObject;
        var tempGrabbable = obj.GetComponent<VrgGrabbable>();
        if(tempGrabbable == myGrabbable)
        {
            isWatchedGrabble = true;
        }
		else
		{
			isWatchedGrabble = false;
		}		
	}
}
