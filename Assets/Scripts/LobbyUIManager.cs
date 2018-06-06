using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour {

	[SerializeField]
	private Button ConnectToPhotonButton;

	[SerializeField]
	private Button CreateVWorldButton;

	[SerializeField]
	private Button JoinVWorldButton;

	[SerializeField]
	private Button CreateVRoomButton;

	[SerializeField]
	private Button JoinVRoomButton;

	[SerializeField]
	private Button DisconectFromPhotonButton;	

	[SerializeField]
	private Text Error;

	[SerializeField]
	private Text Status;

	private void Awake() {
						
		ConnectToPhotonButton.onClick.AddListener(()=>
		{
			CreateVWorldButton.interactable = true;
			CreateVRoomButton.interactable = true;
			PhotonManager.Instance.ConnectPhoton();
			ConnectToPhotonButton.interactable = false;			
		});

		CreateVWorldButton.onClick.AddListener(PhotonManager.Instance.CreateVWorld);
		CreateVRoomButton.onClick.AddListener(PhotonManager.Instance.CreateVRoom);
		DisconectFromPhotonButton.onClick.AddListener(PhotonManager.Instance.DisconnectFromPhoton);

		PhotonManager.Instance.foundVWorldRoom += RunExistedVWorldCb;
		PhotonManager.Instance.foundVRoomRoom += RunExistedVRoomCb;

		if(PhotonNetwork.connected)
		{
			CreateVWorldButton.interactable = true;
			CreateVRoomButton.interactable = true;			
			ConnectToPhotonButton.interactable = false;	
		}
	}

	private void Update()
	{
		var currentStats = PhotonNetwork.connectionStateDetailed.ToString(); 
		if(currentStats != Status.text)
		{
			Status.text = currentStats;
		}
	}
	private void RunExistedVWorldCb(string roomName)
	{
		if(roomName == null || roomName.Length == 0)
		{
			return;
		}

		JoinVWorldButton.interactable = true;
		CreateVWorldButton.interactable = false;
		JoinVWorldButton.onClick.AddListener(()=>{
			PhotonManager.Instance.CallJoinRoom(roomName);});
	}	

	private void RunExistedVRoomCb(string roomName)
	{
		if(roomName == null || roomName.Length == 0)
		{
			return;
		}

		JoinVRoomButton.interactable = true;
		CreateVRoomButton.interactable = false;
		JoinVRoomButton.onClick.AddListener(()=>{
		PhotonManager.Instance.CallJoinRoom(roomName);});
	}

	private void RunErrorText(string cause)
	{
		if(cause == null || cause.Length == 0)
		{
			return;
		}
		
		Error.text = cause;

		CreateVWorldButton.interactable = false;
		JoinVWorldButton.interactable = false;
		CreateVRoomButton.interactable = false;
		JoinVRoomButton.interactable = false;
		ConnectToPhotonButton.interactable = true;
	}

	private void OnDestroy()
	{
		PhotonManager.Instance.foundVWorldRoom -= RunExistedVWorldCb;
		PhotonManager.Instance.foundVRoomRoom -= RunExistedVRoomCb;
	}
}
