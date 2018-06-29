using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;
using System.Linq;
public class VrGrabbableGrabIcon : VrGrabbableIconElement {
	protected override void OnClicked()
	{
		if(!myGrabbable)
		{
			Destroy(this.gameObject);
		}

		var Grabber = FindObjectOfType<VrgGrabber>();
		if(!Grabber)
		{
			return;
		}

		Grabber.IsHolding = true;
		var me = this.GetComponent<RectTransform>();
		Debug.Log("Called deactive from OnClicked");
		me.parent.gameObject.SetActive(false);
	}

	// private void Update()
	// {
	// 	if(myGrabbable.grabbers != null && myGrabbable.grabbers.Count != 0)
	// 	{
	// 		var me = this.GetComponent<RectTransform>();			
	// 		var grabber = myGrabbable.grabbers.FirstOrDefault();
	// 		if(grabber.IsHolding && me.parent.gameObject.activeSelf)
	// 		{
	// 			Debug.Log("Called deactive from Update");
	// 			me.parent.gameObject.SetActive(false);
	// 		}
	// 	}
	// }

}
