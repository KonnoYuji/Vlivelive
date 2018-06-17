using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MouseController : MonoBehaviour {

	private static MouseController instance;
	public static MouseController Instance{
		
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<MouseController>();
			}
			return instance;
		}
	}
	
	public Action<RaycastHit> updateTouchHitEvent;
    public Action updateTouchUnHitEvent;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0))
		{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				RaycastHit hitInfo;
				bool hit = Physics.Raycast(ray, out hitInfo);
				if(hit)
				{		
					if(hit && updateTouchHitEvent != null)
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
}
