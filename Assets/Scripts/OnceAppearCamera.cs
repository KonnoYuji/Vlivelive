using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnceAppearCamera : MonoBehaviour {

	// Use this for initialization
	private void Awake()
	{
		OculusGoInputTest.Instance.TouchedPad += RegisterOnceAppear;
	}

	private void RegisterOnceAppear()
	{
		var forward = Camera.main.transform.forward;
		this.transform.position = forward + Camera.main.transform.position;
	}
}
