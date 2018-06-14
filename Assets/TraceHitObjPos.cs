using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;
using Es.InkPainter;

public class TraceHitObjPos : MonoBehaviour {

	[SerializeField]
	private VrgGrabber hitSource;
	
	[SerializeField]
	private Brush brush = null;

	private void Awake()
	{
		hitSource.updateTouchHitEvent += TakeCurrentRaycast;
	}
	// Update is called once per frame
	private void TakeCurrentRaycast(RaycastHit hit)
	{		
		transform.position = hit.point + hitSource.transform.forward * 0.1f;

		var canvas = hit.collider.GetComponent<InkCanvas>();
		if(canvas != null)					
		{
			//Debug.Log("Canvas is not null");
			canvas.Paint(brush, hit.point);
		}		
	}
}
