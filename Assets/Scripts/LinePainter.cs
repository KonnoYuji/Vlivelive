using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;
using VrGrabber;

public class LinePainter : MonoBehaviour {

	[SerializeField]
	private Brush brush = null;

	[SerializeField]
	private VrgGrabber grabber;

	private void Awake()
	{
		if(grabber == null)
		{
			grabber = FindObjectOfType<VrgGrabber>();
			if(!grabber)
			{
				Destroy(this);
				return;
			}
		}	
		grabber.updateTouchHitEvent += DrawOnCanvas;
	}

	private void DrawOnCanvas(RaycastHit hit)
	{	
		var canvas = hit.collider.GetComponent<InkCanvas>();
		if(canvas != null)
		{		
			//Debug.LogFormat("hitPos X : {0}; Y : {1} ; Z : {2}", hit.point.x, hit.point.y, hit.point.z);
			canvas.Paint(brush, hit.point);
		}
	}

	private void OnDestroy()
	{
		grabber.updateTouchHitEvent -= DrawOnCanvas;	
	}
}
