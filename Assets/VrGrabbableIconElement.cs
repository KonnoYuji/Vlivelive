using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;
using System.Collections;
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

		myGrabbable.isWatchedFuncPanel = true;
		Debug.Log("isWatchedFuncPanel true");
	}

	public void UnGaze()
	{
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
			return;
		}		

		myGrabbable.isWatchedFuncPanel = false;
		Debug.Log("isWatchedFuncPanel true");
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

		int largerCalledNum = 0;
		while(passedTime < sizeChangedTime)
		{
			passedTime += Time.deltaTime;
			var nextScale = Vector3.Lerp(startScale, maxScale, passedTime*2);
			this.GetComponent<RectTransform>().localScale = nextScale;	
			//Debug.LogFormat("Larger Next Scale X : {0} Y : {1} Z : {2}", nextScale.x, nextScale.y, nextScale.z);
			largerCalledNum++;
			//Debug.LogFormat("LargerCalledNum : {0}", largerCalledNum);		
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
		int smallerCalledNum = 0;
		while(passedTime < sizeChangedTime)
		{
			passedTime += Time.deltaTime;
			var nextScale = Vector3.Lerp(startScale, defaultScale, passedTime*2);
			this.GetComponent<RectTransform>().localScale = nextScale;
			//Debug.LogFormat("Smaller Next Scale X : {0} Y : {1} Z : {2}", nextScale.x, nextScale.y, nextScale.z);	
			smallerCalledNum++;
			//Debug.LogFormat("SmallerCalledNum : {0}", smallerCalledNum);
			yield return null;
		}

		ChangeIconSmallerRoutine = null;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		this.GetComponent<RectTransform>().localScale = defaultScale;
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

}
