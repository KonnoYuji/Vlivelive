using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VWorldUIManager : MonoBehaviour {

	[SerializeField]
	private Button MainButton;

	[SerializeField]
	private Button VipButton;

	[SerializeField]
	private Button Audience;

	[SerializeField]
	private Button LeaveRoom;

	[SerializeField]
	private VWorldInitialPos myPos;

	[SerializeField]
	private GameObject PlayerSelection;

	private bool initialized = false; 

	private void Awake()
	{					
		MainButton.onClick.AddListener(()=>{
			PhotonManager.Instance.InstantiateMyChar("unitychan", myPos.MyCharOffset(VWorldInitialPos.PlayerStyle.Main), null);
			Camera.main.transform.parent.transform.position = myPos.MyCharOffset(VWorldInitialPos.PlayerStyle.Main);
			PlayerSelection.SetActive(false);
		});

		VipButton.onClick.AddListener(()=>{
			PhotonManager.Instance.InstantiateMyChar("VipMan", myPos.MyCharOffset(VWorldInitialPos.PlayerStyle.Vip), null);
			Camera.main.transform.parent.transform.position = myPos.MyCharOffset(VWorldInitialPos.PlayerStyle.Vip);
			PlayerSelection.SetActive(false);
		});

		Audience.onClick.AddListener(()=>{
			PhotonManager.Instance.InstantiateMyChar("asobi_chan_b_yellow", myPos.MyCharOffset(VWorldInitialPos.PlayerStyle.Audience), null);
			Camera.main.transform.parent.transform.position = myPos.MyCharOffset(VWorldInitialPos.PlayerStyle.Audience);
			PlayerSelection.SetActive(false);
		});

		LeaveRoom.onClick.AddListener(PhotonManager.Instance.LeaveRoom);
	}

	private void Update()
	{
		if(!initialized)
		{
			if(PhotonNetwork.inRoom)
			{
				MainButton.interactable = true;
				VipButton.interactable = true;
				Audience.interactable = true;
				initialized = true;
			}
		}
	}
}
