using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VrGrabber;

public class VrgTargetPointer : MonoBehaviour {

	[SerializeField]
	private VrgGrabber myGrabber;

	[SerializeField]
	private GameObject pointer;

	private float offset = 0.025f;

	private void Awake() {
#if !UNITY_EDITOR && UNITY_ANDROID		
		myGrabber.updateTouchHitEvent += ShowPointer; 		
#elif UNITY_EDITOR && UNITY_STANDALONE	
		MouseRaycastSimulator.Instance.updateTouchHitEvent += ShowPointer;
#endif
	}
	
	private void ShowPointer(RaycastHit hitInfo)
	{		
		pointer.transform.position = hitInfo.point + (hitInfo.normal * offset);		
		pointer.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
	}

	private void OnDestroy()
	{
#if !UNITY_EDITOR && UNITY_ANDROID		
		myGrabber.updateTouchHitEvent -= ShowPointer; 		
#elif UNITY_EDITOR && UNITY_STANDALONE	
		MouseRaycastSimulator.Instance.updateTouchHitEvent -= ShowPointer;
#endif
	}
}
