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
		//Debug.Log("Called deactive from OnClicked");
		me.parent.gameObject.SetActive(false);
	}
}
