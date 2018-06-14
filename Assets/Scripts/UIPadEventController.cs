using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;

public class UIPadEventController : MonoBehaviour {

	[SerializeField]
	private VrgGrabber hitObjGrabber;

    private Transform currentUITarget = null;                	

	private void Start()
	{		
		if(hitObjGrabber == null)
		{
			hitObjGrabber = GetComponent<VrgGrabber>();			
		}
		hitObjGrabber.updateTouchHitEvent += AttachUIEventToController;
		hitObjGrabber.updateTouchUnHitEvent += DetachUIEventFromController;
	}

	public void AttachUIEventToController(RaycastHit obj)
	{
		if(currentUITarget != null)
		{	
			if(currentUITarget != obj.collider.transform)
			{
				DetachUIEventFromController();
			}
			else
			{
				return;
			}
		}

		currentUITarget = obj.collider.transform;

		//今のフレームのObjectがフリックイベントを保つ場合にAttachする
		var hitEventAttacher = currentUITarget.transform.GetComponent(typeof(IEventAttacher)) as IEventAttacher;
		if(hitEventAttacher != null)
		{
			//Debug.Log("Called AttachEvent");
			hitEventAttacher.AttachEvents();					
		}
	}

	public void DetachUIEventFromController()
	{
		if(currentUITarget == null)
		{
			return;
		}

		var targetUIEventAttacher = currentUITarget.GetComponent(typeof(IEventAttacher)) as IEventAttacher;
		if(targetUIEventAttacher != null)
		{							
			targetUIEventAttacher.DetachEvents();						
		}

		currentUITarget = null;
	}

	public void OnDestroy()
	{
		if(hitObjGrabber != null)
		{
			hitObjGrabber.updateTouchHitEvent -= AttachUIEventToController;
			hitObjGrabber.updateTouchUnHitEvent -= DetachUIEventFromController;
		}
	}
}
