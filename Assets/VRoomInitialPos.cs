using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRoomInitialPos : MonoBehaviour {

	[SerializeField]
	private GameObject[] symbols;
	
	[SerializeField]
    private Vector3 initialYAxisOffsetOfCamera = new Vector3(0, 0, 0);
	
	// Use this for initialization
	private void Awake() {

		if(symbols.Length == 0)
		{
				Destroy(this.gameObject);
				return;
		}	
	}

	public Vector3 GetInitialPos()
	{
		for(int i=0; i<symbols.Length; i++)
		{
			if(symbols[i].activeSelf)
			{
				symbols[i].SetActive(false);
				return symbols[i].transform.position;
			}
		}

		return new Vector3(0, 0, 0) + initialYAxisOffsetOfCamera;
	}
}
