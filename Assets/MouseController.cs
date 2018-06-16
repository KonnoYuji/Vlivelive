using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	InkPainter Painter;
	// Use this for initialization
	void Start () {
		Painter = FindObjectOfType<InkPainter>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0))
		{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				RaycastHit hitInfo;

				if(Physics.Raycast(ray, out hitInfo))
				{		
					//Debug.Log("Found Something");			
					var paintObject = hitInfo.transform;
					
					if(paintObject != null)
					{
						Painter.CatchRayCastInfo(hitInfo);
					}						
				}
		}
	}
}
