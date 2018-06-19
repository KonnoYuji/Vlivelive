using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MouseRaycastSimulator : MonoBehaviour {

	private static MouseRaycastSimulator instance;
	public static MouseRaycastSimulator Instance{
		
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<MouseRaycastSimulator>();
			}
			return instance;
		}
	}

	public Action<RaycastHit> updateTouchHitEvent;
    public Action updateTouchUnHitEvent;
	
	// Update is called once per frame
	void Update () {

		//Debug.LogFormat("x : {0}, y : {1}", Input.mousePosition.x.ToString(), Input.mousePosition.y.ToString());
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hitInfo;
		bool hit = Physics.Raycast(ray, out hitInfo);
		if(hit)
		{	
			//Debug.LogFormat("hitted {0}", hitInfo.collider.name);
			Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), hitInfo.point);
			//painter.CreateInk(hitInfo.point, painter.transform);
			if(updateTouchHitEvent != null)
        	{
            	updateTouchHitEvent(hitInfo);
        	}
        	
        }
		else
        {
			if(updateTouchUnHitEvent != null)
			{
				updateTouchUnHitEvent();
			}
            
        }	
	}
}
