using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRoomUIManager : MonoBehaviour {

	[SerializeField]
	private Button StartTalkButton;

	[SerializeField]
	private Button LeaveButton;

	[SerializeField]
	private VRoomInitialPos myPos;

	private bool initialized = false;

	private void Awake()
	{
		LeaveButton.onClick.AddListener(PhotonManager.Instance.LeaveRoom);
		StartTalkButton.onClick.AddListener(()=>{
			var tempPos = myPos.GetInitialPos();
			PhotonManager.Instance.InstantiateMyChar("unitychan", tempPos, null);
			Camera.main.transform.parent.transform.position = tempPos;
			StartTalkButton.gameObject.SetActive(false);
		});
	}

	private void Update()
	{
		if(!initialized)
		{
			if(PhotonNetwork.inRoom)
			{
				StartTalkButton.interactable = true;
				initialized = true;
			}
		}
	}
}
