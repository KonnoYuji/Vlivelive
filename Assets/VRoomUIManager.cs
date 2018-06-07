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

	[SerializeField]
	private Text Status;

	[SerializeField]
	private Text Error;

	[SerializeField]
	private Text RcmdCnt;

	[SerializeField]
	private bool isDebuged = false;

	private bool initialized = false;

	private void Awake()
	{
		LeaveButton.onClick.AddListener(()=>
		{
			myPos.OffOccupiedstate();
			PhotonManager.Instance.LeaveRoom();
		});
		StartTalkButton.onClick.AddListener(()=>{
			var tempPos = myPos.GetInitialPos();
			PhotonManager.Instance.InstantiateMyChar("unitychan", tempPos, null);
			Camera.main.transform.parent.transform.position = tempPos;
			StartTalkButton.gameObject.SetActive(false);
		});

		if(isDebuged)
		{
			Status.gameObject.SetActive(true);
			Error.gameObject.SetActive(true);
			RcmdCnt.gameObject.SetActive(true);
			PhotonManager.Instance.foundError += GetNetworkError; 
		}
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

		if(isDebuged)
		{
			var currentStats = PhotonNetwork.connectionStateDetailed.ToString(); 
			if(currentStats != Status.text)
			{
				Status.text = currentStats;
			}
			RcmdCnt.text = "RCmdCnt : " + PhotonNetwork.ResentReliableCommands.ToString();
		}
	}

	private void GetNetworkError(string error)
	{
		Error.text = error;
	}

	private void OnDestroy()
	{
		PhotonManager.Instance.foundError -= GetNetworkError;
	}
}
