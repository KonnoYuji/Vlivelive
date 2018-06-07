using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRoomInitialPos : MonoBehaviour {

	[SerializeField]
	private GameObject[] symbols;
	
	[SerializeField]
    private Vector3 initialOffsetOfCamera = new Vector3(0, 0, 0);
	
	// Use this for initialization
	private void Awake() {

		if(symbols.Length == 0)
		{
				Destroy(this.gameObject);
				return;
		}	
	}

	private void Update()
	{		
		for(int i=0; i<symbols.Length; i++)
		{
			var symbol = symbols[i].GetComponent<PlayerPosSymbol>();
			var myView = symbol.GetComponent<PhotonView>();

			if(symbol.isOccupied && myView.isMine && symbol.playerId != PhotonNetwork.player.ID)
			{							
				symbol.WriteActiveState(false);
				symbol.playerId = 0;				
			}
		}			
	}	

	public Vector3 GetInitialPos()
	{
		for(int i=0; i<symbols.Length; i++)
		{
			var symbol = symbols[i].GetComponent<PlayerPosSymbol>();

			if(!symbol.isOccupied)
			{
				var myView = symbol.GetComponent<PhotonView>();				

				if(!myView.isMine)
				{
					symbol.ChaneOwnerShip(PhotonNetwork.player);
				}
				
				symbol.WriteActiveState(true);
				symbol.WritePlayerId(PhotonNetwork.player.ID);
				
				return symbol.transform.position;
			}
		}

		return new Vector3(0, 0, 0);
	}


	public void OffOccupiedstate()
	{
		for(int i=0; i<symbols.Length; i++)
		{
			var symbol = symbols[i].GetComponent<PlayerPosSymbol>();

			if(symbol.isOccupied)
			{
				var myView = symbol.GetComponent<PhotonView>();				

				if(myView.isMine)
				{					
					symbol.WriteActiveState(false);
				}				
			}
		}
	}

	public Vector3 GetCameraInitialOffset()
	{
		return initialOffsetOfCamera;
	}
}
