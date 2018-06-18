using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class InkTestMouseInput : MonoBehaviour {

	private static InkTestMouseInput instance;
	public static InkTestMouseInput Instance{
		
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<InkTestMouseInput>();
			}
			return instance;
		}
	}

	[SerializeField]
	private InkPainter painter;

	private void Awake()
	{
		if( painter == null)
		{
			painter = FindObjectOfType<InkPainter>();
		}
	}	
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0))
		{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				RaycastHit hitInfo;
				bool hit = Physics.Raycast(ray, out hitInfo);
				if(hit)
				{		
					painter.CreateInk(hitInfo.point, painter.transform);
        		}
		}		
	}
}
