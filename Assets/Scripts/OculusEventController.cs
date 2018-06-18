using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;

//OculusGoInputとRaycastでヒットしたObj間で、OculusEventを操作するためのコントローラー
public class OculusEventController : MonoBehaviour {

	//RayCastソース
	[SerializeField]
	private VrgGrabber hitObjGrabber;

    private Transform currentTarget = null;                	

	private void Start()
	{
		if(hitObjGrabber == null)
		{
			hitObjGrabber = FindObjectOfType<VrgGrabber>();	
		}

		//Raycastソース取得時のイベントフック
		hitObjGrabber.updateTouchHitEvent += CatchRayCastInfo;
		hitObjGrabber.updateTouchUnHitEvent += DetachEvent;
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
				var tempEventDefinition = currentTarget.transform.GetComponent(typeof(IOculusEventDefinition)) as IOculusEventDefinition;
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
		var eventDefinition = currentTarget.transform.GetComponent(typeof(IOculusEventDefinition)) as IOculusEventDefinition;
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
		}
	}

	public void DetachEvent()
	{
		if(currentTarget == null)
		{
			return;
		}
		
		var eventDefinition = currentTarget.GetComponent(typeof(IOculusEventDefinition)) as IOculusEventDefinition;
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
		}

		currentTarget = null;
	}

	public void OnDestroy()
	{
		if(hitObjGrabber != null)
		{
			hitObjGrabber.updateTouchHitEvent -= AttachEvent;
			hitObjGrabber.updateTouchUnHitEvent -= DetachEvent;
		}
	}
}
