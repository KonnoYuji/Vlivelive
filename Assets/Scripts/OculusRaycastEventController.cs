using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;

//OculusGoInputとRaycastでヒットしたObj間で、OculusEventを操作するためのコントローラー
public class OculusRaycastEventController : MonoBehaviour {

	//RayCastソース
	[SerializeField]
	private VrgGrabber hitObjGrabber;

    private Transform currentTarget = null;                	

	private void Start()
	{
#if !UNITY_EDITOR && UNITY_ANDROID		
		if(hitObjGrabber == null)
		{
			hitObjGrabber = FindObjectOfType<VrgGrabber>();	
		}

		//Raycastソース取得時のイベントフック
		hitObjGrabber.updateTouchHitEvent += CatchRayCastInfo;
		hitObjGrabber.updateTouchUnHitEvent += DetachEvent;
#elif UNITY_EDITOR && UNITY_STANDALONE
		MouseRaycastSimulator.Instance.updateTouchHitEvent += CatchRayCastInfo;
		MouseRaycastSimulator.Instance.updateTouchUnHitEvent += DetachEvent;
#endif		
	}

	public void CatchRayCastInfo(RaycastHit obj)
	{
		if(currentTarget != null)
		{	
			if(currentTarget != obj.collider.transform)
			{				
				DetachEvent();
			}
			else
			{
				//同じオブジェクトをキャストし続けてるときはキャスト情報を、オブジェクトに渡す
				var tempEventDefinition = currentTarget.transform.GetComponent(typeof(IOculusRaycastEventDefinition)) as IOculusRaycastEventDefinition;
				if(tempEventDefinition != null)
				{					
					tempEventDefinition.Gaze();
					tempEventDefinition.CatchHittedInfo(obj);					
				}
				return;
			}
		}

		currentTarget = obj.collider.transform;
		//今のフレームのObjectがフリックイベントを保つ場合にAttachする
		AttachEvent();
	}

	public void AttachEvent()
	{
		var eventDefinition = currentTarget.transform.GetComponent(typeof(IOculusRaycastEventDefinition)) as IOculusRaycastEventDefinition;
		if(eventDefinition != null)
		{										
			OculusGoInput.Instance.TouchedPad += eventDefinition.TouchedPad;
	   		OculusGoInput.Instance.ClickedPad += eventDefinition.ClickedPad;
	   		OculusGoInput.Instance.UpFlicked += eventDefinition.UpFlicked;
	   		OculusGoInput.Instance.DownFlicked += eventDefinition.DownFlicked;
	   		OculusGoInput.Instance.LeftFlicked += eventDefinition.LeftFlicked;
	   		OculusGoInput.Instance.RightFlicked += eventDefinition.RightFlicked;
	   		OculusGoInput.Instance.TriggerEntered += eventDefinition.TriggerEntered;
	   		OculusGoInput.Instance.GetUpTouchPad += eventDefinition.GetUpTouchPad;
			OculusGoInput.Instance.interval = eventDefinition.Interval;						
		}
	}

	public void DetachEvent()
	{
		if(currentTarget == null)
		{
			return;
		}
		
		var eventDefinition = currentTarget.GetComponent(typeof(IOculusRaycastEventDefinition)) as IOculusRaycastEventDefinition;
		if(eventDefinition != null)
		{
			eventDefinition.UnGaze();												
			OculusGoInput.Instance.TouchedPad -= eventDefinition.TouchedPad;
	   		OculusGoInput.Instance.ClickedPad -= eventDefinition.ClickedPad;
	   		OculusGoInput.Instance.UpFlicked -= eventDefinition.UpFlicked;
	   		OculusGoInput.Instance.DownFlicked -= eventDefinition.DownFlicked;
	   		OculusGoInput.Instance.LeftFlicked -= eventDefinition.LeftFlicked;
	   		OculusGoInput.Instance.RightFlicked -= eventDefinition.RightFlicked;
	   		OculusGoInput.Instance.TriggerEntered -= eventDefinition.TriggerEntered;
	   		OculusGoInput.Instance.GetUpTouchPad -= eventDefinition.GetUpTouchPad;				
			OculusGoInput.Instance.interval = 0;		
		}

		currentTarget = null;
	}

	public void OnDestroy()
	{
#if !UNITY_EDITOR && UNITY_ANDROID		
		if(hitObjGrabber != null)
		{
			hitObjGrabber.updateTouchHitEvent -= CatchRayCastInfo;
			hitObjGrabber.updateTouchUnHitEvent -= DetachEvent;
		}

#elif UNITY_EDITOR && UNITY_STANDALONE
		MouseRaycastSimulator.Instance.updateTouchHitEvent -= CatchRayCastInfo;
		MouseRaycastSimulator.Instance.updateTouchUnHitEvent -= DetachEvent;
#endif	
	}
}
