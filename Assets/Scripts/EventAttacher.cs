using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;
using System;

public class EventAttacher : MonoBehaviour {

	[SerializeField]
	private VrgGrabber hitObjGrabber;

    private Transform currentTarget = null;                	

	private void Start()
	{		
#if !UNITY_EDITOR && UNITY_ANDROID				
		if(hitObjGrabber == null)
		{
			hitObjGrabber = GetComponent<VrgGrabber>();			
		}
		hitObjGrabber.updateTouchHitEvent += AttachEvent;
		hitObjGrabber.updateTouchUnHitEvent += DetachEvent;
#elif UNITY_EDITOR && UNITY_STANDALONE
		MouseRaycastSimulator.Instance.updateTouchHitEvent += AttachEvent;
		MouseRaycastSimulator.Instance.updateTouchUnHitEvent += DetachEvent;
#endif		
	}

	public void AttachEvent(RaycastHit obj)
	{
		if(currentTarget != null)
		{	
			if(currentTarget != obj.collider.transform)
			{
				DetachEvent();
			}
			else
			{
				var tempEventDefinition = currentTarget.transform.GetComponent(typeof(IEventDefinition)) as IEventDefinition;
				if(tempEventDefinition != null)
				{					
					tempEventDefinition.CatchHittedInfo(obj);					
				}
				return;
			}
		}

		currentTarget = obj.collider.transform;
		//今のフレームのObjectがフリックイベントを保つ場合にAttachする
		var eventDefinition = currentTarget.transform.GetComponent(typeof(IEventDefinition)) as IEventDefinition;
		if(eventDefinition != null)
		{
			eventDefinition.AttachedEvents();				
		}
	}

	public void DetachEvent()
	{
		if(currentTarget == null)
		{
			return;
		}

		var eventDefinition = currentTarget.GetComponent(typeof(IEventDefinition)) as IEventDefinition;
		if(eventDefinition != null)
		{							
			eventDefinition.DetachedEvents();						
		}

		currentTarget = null;
	}

	public void OnDestroy()
	{
#if !UNITY_EDITOR && UNITY_ANDROID		
		if(hitObjGrabber != null)
		{
			hitObjGrabber.updateTouchHitEvent -= AttachEvent;
			hitObjGrabber.updateTouchUnHitEvent -= DetachEvent;
		}
#elif UNITY_EDITOR && UNITY_STANDALONE
		MouseRaycastSimulator.Instance.updateTouchHitEvent += AttachEvent;
		MouseRaycastSimulator.Instance.updateTouchUnHitEvent += DetachEvent;
#endif		
	}
}
